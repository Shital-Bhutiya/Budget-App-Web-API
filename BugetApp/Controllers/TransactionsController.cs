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
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Transactions
        public IQueryable<Transaction> GetTransactions()
        {
            return db.Transactions;
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> GetTransaction(int id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

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
            dbTransaction.Description = transaction.Description;
            dbTransaction.Date = transaction.Date;
            dbTransaction.Ammount = transaction.Ammount;
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
                return BadRequest("Account id not found");
            }
            if (!db.Categories.Any(p => p.Id == transaction.CategoryId))
            {
                return BadRequest("Category not found");
            }

            transaction.IsVoided = false;
            transaction.EnteredById = User.Identity.GetUserId();
            db.Transactions.Add(transaction);
            await db.SaveChangesAsync();
            UpdateBalance(transaction);
            return Ok("Successfully created transaction");
        }

        public IHttpActionResult VoidTransction(int id)
        {
            var transaction = db.Transactions.Include(p => p.Account).FirstOrDefault(p => p.Id == id);
            if (transaction == null)
            {
                return BadRequest("Transaction is not exist");
            }
            transaction.IsVoided = true;
            UpdateBalance(transaction);
            return Ok();
        }

        private void UpdateBalance(Transaction transaction)
        {
            Transaction dbtransaction =  db.Transactions.Where(p=>p.Id == transaction.Id).Include(p=>p.Account).FirstOrDefault();

            dbtransaction.Account.Balance += transaction.Ammount;
            db.SaveChanges();
        }
        // DELETE: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> DeleteTransaction(int id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();

            return Ok(transaction);
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