using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BugetApp.Models;
using BugetApp.Models.Classes;
using BugetApp.Models.ViewModel;
using Microsoft.AspNet.Identity;

namespace BugetApp.Controllers
{
    public class AccountsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Accounts
        public IQueryable<Account> GetAccounts()
        {
            return db.Accounts;
        }

        // GET: api/Accounts/5
        [ResponseType(typeof(Account))]
        public IHttpActionResult GetAccount(int id)
        {
            var accountsHouseHoldViewModel = new AccountsHouseHoldViewModel();
            var household = db.HouseHolds.Where(p => p.Id == id).FirstOrDefault();
            if (household == null)
            {
                return Ok("Household is not exist");
            }
            if (!household.JoinedUsers.Any(p => p.Id == User.Identity.GetUserId() || household.CreatorId != User.Identity.GetUserId()))
            {
                return Ok("You are not authorized to get Account");
            }

            accountsHouseHoldViewModel.HouseholdName = household.Name;

            accountsHouseHoldViewModel.Accounts = db.Accounts.Where(p => p.HouseholdId == household.Id).Select(i => new AccountsViewModel { Balance = i.Balance, Name = i.Name }).ToList();
            
            return Ok(accountsHouseHoldViewModel);
        }

        // PUT: api/Accounts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAccount(int id, Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbAccount = db.Accounts.FirstOrDefault(p => p.Id == id);
            if (dbAccount == null)
            {
                return Ok("Thre is no Account with this id");
            }
            if ((!dbAccount.Household.JoinedUsers.Any(p => p.Id == User.Identity.GetUserId())) || (dbAccount.Household.CreatorId != User.Identity.GetUserId()))
            {
                return BadRequest("You are not Authorize");
            }
            if (id != account.Id)
            {
                return BadRequest();
            }
            if (account.Name != null)
            {
                dbAccount.Name = account.Name;
            }
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Accounts
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> PostAccount(Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!db.HouseHolds.Any(p => p.Id == account.HouseholdId))
            {
                return Ok("we don't have any household of that id that you gave us");
            }
            var dbAccount = db.Accounts.Where(p => p.HouseholdId == account.HouseholdId).Select(p => p.Name).ToList();
            if (dbAccount.Contains(account.Name))
            {
                return Ok("We have same category with this name on this household");
            }
            db.Accounts.Add(account);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = account.Id }, account);
        }

        // DELETE: api/Accounts/5
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> DeleteAccount(int id)
        {
            Account account = db.Accounts.FirstOrDefault(p => p.Id == id);
            if ((account.Household.JoinedUsers.Any(p => p.Id == User.Identity.GetUserId())) || (account.Household.CreatorId != User.Identity.GetUserId()))
            {
                return BadRequest("You are not Authorize to delete this account");
            }
            if (account == null)
            {
                return BadRequest("Account not found");
            }

            db.Accounts.Remove(account);
            await db.SaveChangesAsync();

            return Ok("Successfully deleted this account");
        }
        public IHttpActionResult UpdateBalance(int id)
        {
            Account account = db.Accounts.FirstOrDefault(p => p.Id == id);
            if(account == null)
            {
                return BadRequest("Account is not exist");
            }
            decimal totalAmmount = 0;
            var Transactions = db.Transactions.Where(p => p.AccountId == account.Id).ToList();
            foreach (var transaction in Transactions)
            {
                if (!transaction.IsVoided)
                {
                    totalAmmount += transaction.Ammount;
                }
            }
            return Ok(totalAmmount);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountExists(int id)
        {
            return db.Accounts.Count(e => e.Id == id) > 0;
        }
    }
}