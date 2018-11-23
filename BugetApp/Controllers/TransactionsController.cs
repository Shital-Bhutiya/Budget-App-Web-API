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
    [Authorize]
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IHttpActionResult GetTransaction(int id)
        {
            var Transactions = db.Transactions.Where(p => p.Account.HouseholdId == id).Select(a=> new TransactionViewModel{ Category = a.Category.Name, Account = a.Account.Name,Id =a.Id,Description=a.Description,Date =a.Date, Amount = a.Amount, IsVoided = a.IsVoided }).ToList();
            if(Transactions.Count == 0)
            {
                return Ok("No Transaction Found");
            }
            return Ok(Transactions);
        }
       /// <summary>
       /// Edit Transaction
       /// </summary>
       /// <param name="id">Id Of Transaction</param>
       /// <param name="transaction">Information to update</param>
       /// <returns></returns>
        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTransaction(int id, TransactionEditingViewModel transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Transaction dbTransaction = await db.Transactions.FindAsync(id);
            if (dbTransaction == null)
            {
                return BadRequest("Transaction is not exist");
            }
            if (!db.Categories.Any(p => p.Id == transaction.CategoryId))
            {
                return BadRequest("Category not found");
            }
            var HouseHold = db.Categories.Where(p => p.Id == transaction.CategoryId).Select(p => p.Household).FirstOrDefault();
            if (!HouseHold.JoinedUsers.Any(p => p.Id == User.Identity.GetUserId() || HouseHold.CreatorId != User.Identity.GetUserId()))
            {
                return Ok("You are not authorized to Edit Transaction");
            }
            dbTransaction.Description = transaction.Description;
            dbTransaction.Date = transaction.Date;
            dbTransaction.Amount = transaction.Amount;
            dbTransaction.CategoryId = transaction.CategoryId;
            UpdateBalance(dbTransaction);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
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
        /// <summary>
        /// Create A transaction
        /// </summary>
        /// <param name="transaction">Transaction Infromation</param>
        /// <returns></returns>
        // POST: api/Transactions
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> PostTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!db.Accounts.Any(p => p.Id == transaction.AccountId))
            {
                return Ok("Account id not found");
            }
            if (!db.Categories.Any(p => p.Id == transaction.CategoryId))
            {
                return Ok("Category not found");
            }
            var HouseHold = db.Categories.Where(p => p.Id == transaction.CategoryId).Select(p => p.Household).FirstOrDefault();
            if (!(HouseHold.JoinedUsers.Any(p => p.Id == User.Identity.GetUserId())) && HouseHold.CreatorId != User.Identity.GetUserId())
            {
                return Ok("You are not authorized to Create Transaction");
            }
            transaction.IsVoided = false;
            transaction.EnteredById = User.Identity.GetUserId();
            db.Transactions.Add(transaction);
            await db.SaveChangesAsync();
            UpdateBalance(transaction);
            return Ok("Successfully created transaction");
        }
        /// <summary>
        /// Void A transaction
        /// </summary>
        /// <param name="id">id of transaction</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult VoidTransction(int id)
        {
            var transaction = db.Transactions.Include(p => p.Account).FirstOrDefault(p => p.Id == id);
            if (transaction == null)
            {
                return Ok("Transaction is not exist");
            }
            var HouseHold = db.Categories.Where(p => p.Id == transaction.CategoryId).Select(p => p.Household).FirstOrDefault();
            if (!HouseHold.JoinedUsers.Any(p => p.Id == User.Identity.GetUserId()) && HouseHold.CreatorId != User.Identity.GetUserId())
            {
                return Ok("You are not authorized to Void Transaction");
            }
            transaction.IsVoided = true;
            UpdateBalance(transaction);
            return Ok("Voided Successfully");
        }

        private void UpdateBalance(Transaction transaction)
        {
            Transaction dbtransaction = db.Transactions.Where(p => p.Id == transaction.Id).Include(p => p.Account).FirstOrDefault();
            if(transaction.Amount > 0)
            {
                dbtransaction.Account.Balance -= transaction.Amount;
            }
            else
            {
                dbtransaction.Account.Balance += transaction.Amount;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// Delete A transaction
        /// </summary>
        /// <param name="id">Id of Transaction</param>
        /// <returns></returns>
        // DELETE: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> DeleteTransaction(int id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            var HouseHold = db.Categories.Where(p => p.Id == transaction.CategoryId).Select(p => p.Household).FirstOrDefault();

            if (!HouseHold.JoinedUsers.Any(p => p.Id == User.Identity.GetUserId() || HouseHold.CreatorId != User.Identity.GetUserId()))
            {
                return Ok("You are not authorized to Void Transaction");
            }
            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();

            return Ok("Successfully Deleted Transaction");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionExists(int id)
        {
            return db.Transactions.Count(e => e.Id == id) > 0;
        }
    }
}