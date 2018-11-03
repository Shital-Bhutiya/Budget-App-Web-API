using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Description;
using BugetApp.Models;
using BugetApp.Models.Classes;
using Microsoft.AspNet.Identity;

namespace BugetApp.Controllers
{
    [RoutePrefix("api/HouseHolds")]
    public class HouseHoldsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/HouseHolds
        [Route("")]
        public ICollection<string> GetHouseHolds()
        {
            return db.HouseHolds.Select(p => p.Name).ToList();
        }

        // GET: api/HouseHolds/5
        [ResponseType(typeof(HouseHolds))]
        [Route("GetHouseHolds")]
        public async Task<IHttpActionResult> GetHouseHolds(int id)
        {
            HouseHolds houseHolds = await db.HouseHolds.FindAsync(id);
            if (houseHolds == null)
            {
                return NotFound();
            }

            return Ok(houseHolds);
        }

        // PUT: api/HouseHolds/5
        [ResponseType(typeof(void))]
        [Route("PutHouseHolds")]

        public async Task<IHttpActionResult> PutHouseHolds(int id, HouseHolds houseHolds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != houseHolds.Id)
            {
                return BadRequest();
            }
            var household = db.HouseHolds.Where(p => p.Id == houseHolds.Id).FirstOrDefault();
            household.CreatorId = User.Identity.GetUserId();
            household.Name = houseHolds.Name;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseHoldsExists(id))
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
        [HttpPost]
        [Route("InvitedUserViewList")]
        public List<string> InvitedUserViewList(int id)
        {
            var householdId = db.HouseHolds.Where(p => p.Id == id).Select(p => p.Id).FirstOrDefault();
            if (householdId == 0)
            {
                var error = "this ID is not found";
                List<string> myList = new List<string> { error };

                return myList;
            }
            var invitedUsers = db.HouseHoldInvites.Where(p => p.HouseHoldId == householdId).Select(prop => prop.InvitedUser.UserName).ToList();

            return invitedUsers;
        }
        [HttpPost]
        [Route("InviteUser")]
        public string InviteUser(int id, string email)
        {
            var household = db.HouseHolds.Where(p => p.Id == id).FirstOrDefault();
            var user = db.Users.Where(p => p.UserName == email).FirstOrDefault();
            if (user == null)
            {
                return "user is not exist";
            }
            if (household == null)
            {
                return "household is not exist";
            }
            if (household.CreatorId == User.Identity.GetUserId() && !household.HouseHoldInvites.Any(p => p.InvitedUser.Email == email))
            {
                var personalEmailService = new PersonalEmailService();
                var mailMessage = new MailMessage(
                   WebConfigurationManager.AppSettings["emailto"],
                  email
                  );
                mailMessage.Body = "Your invited to" + household.Name + " House";
                mailMessage.Subject = "Invitaton of household";
                mailMessage.IsBodyHtml = true;
                personalEmailService.Send(mailMessage);
                var invites = new HouseHoldInvites();
                invites.HouseHoldId = household.Id;
                invites.InvitedUserId = user.Id;
                db.HouseHoldInvites.Add(invites);
                household.HouseHoldInvites.Add(invites);
                db.SaveChanges();
                return "Successfully invited";
            }
            else
            {
                return "This user has been alerady invited";
            }
        }
        [HttpPost]
        [Route("JoinHouseHold")]
        public string JoinHouseHold(int id, string email)
        {
            var household = db.HouseHolds.Where(p => p.Id == id).FirstOrDefault();
            var user = db.Users.Where(p => p.UserName == email).FirstOrDefault();
            if (user == null)
            {
                return "Umm looks like you don't have account. plz register first.";
            }
            if (household != null)
            {
                if (!household.JoinedUsers.Any(p => p.Email == email))
                {
                    var houseHoldInvites = db.HouseHoldInvites.Where(p => p.InvitedUser.Email == email).FirstOrDefault();
                    db.HouseHoldInvites.Remove(houseHoldInvites);
                    household.JoinedUsers.Add(user);
                    db.SaveChanges();
                    return "Successfully Joined";
                }
                else
                {
                    return "This user has been alerady Joined";
                }
            }
            else
            {
                return "household is not exist";
            }
        }
        [HttpPost]
        [Route("LeaveHouseHold")]
        public string LeaveHouseHold(int id, string email)
        {
            var household = db.HouseHolds.Where(p => p.Id == id).FirstOrDefault();
            var user = db.Users.Where(p => p.UserName == email).FirstOrDefault();
            if (user == null)
            {
                return "This user is not exist in our database";
            }
            if (household != null)
            {
                if (household.JoinedUsers.Any(p => p.Email == email))
                {
                    household.JoinedUsers.Remove(user);
                    db.SaveChanges();
                    return "Successfully Leaved";
                }
                else
                {
                    return "There is no such user in this household";
                }
            }
            else
            {
                return "household is not exist";
            }
        }

        // POST: api/HouseHolds
        [ResponseType(typeof(HouseHolds))]
        [Route("PostHouseHolds")]
        public async Task<IHttpActionResult> PostHouseHolds(HouseHolds houseHolds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (db.HouseHolds.Any(p => p.Name == houseHolds.Name))
            {
                return Ok("Plz rename it because we have one already with this name");
            }
            houseHolds.CreatorId = User.Identity.GetUserId();
            db.HouseHolds.Add(houseHolds);
            await db.SaveChangesAsync();
            return Ok("Successfully created");
        }

        // DELETE: api/HouseHolds/5
        [ResponseType(typeof(HouseHolds))]
        [Route("DeleteHouseHolds")]
        public async Task<IHttpActionResult> DeleteHouseHolds(int id)
        {
            HouseHolds houseHolds = await db.HouseHolds.FindAsync(id);
            if (houseHolds == null)
            {
                return NotFound();
            }

            db.HouseHolds.Remove(houseHolds);
            await db.SaveChangesAsync();

            return Ok("Deleted Successfully");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HouseHoldsExists(int id)
        {
            return db.HouseHolds.Count(e => e.Id == id) > 0;
        }
    }
}