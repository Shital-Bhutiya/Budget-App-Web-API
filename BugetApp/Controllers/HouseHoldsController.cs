using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Description;
using BugetApp.Models;
using BugetApp.Models.Classes;
using BugetApp.Models.ViewModel;
using Microsoft.AspNet.Identity;

namespace BugetApp.Controllers
{
    [RoutePrefix("api/HouseHolds")]
    [Authorize]
    public class HouseHoldsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// Get All household
        /// </summary>
        /// <returns></returns>
        // GET: api/HouseHolds
        [Route("")]
        public IHttpActionResult GetHouseHolds()
        {
            var userId = User.Identity.GetUserId();
            var HouseHolds = db.HouseHolds.Where(p=>p.CreatorId == userId)                
                .Select(p => new MyHoseholdViewModel
                {
                    Id = p.Id,
                    Name=p.Name,
                    Creator = p.Creator.Email,
                    Members = p.JoinedUsers.Select(t => new MyHouseHoldMembersVieModel
                    {
                        Email = t.Email
                    }).ToList()
                })
                .ToList();
            return Ok(HouseHolds);
        }
        [Route("GetMyHouseHolds")]
        public IHttpActionResult GetMyHouseHolds()
        {
            var userID = User.Identity.GetUserId();
            var HouseHolds = db.HouseHolds.Where(p=>p.CreatorId == userID)
                .Select(p => new MyHoseholdViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Creator = p.Creator.Email
                })
                .ToList();
            return Ok(HouseHolds);
        }
        // GET: api/HouseHolds/5
        /// <summary>
        /// Get Specific HouseHold
        /// </summary>
        /// <param name="id">Id of HouseHold</param>
        /// <returns></returns>
        [ResponseType(typeof(HouseHolds))]
        [Route("GetHouseHolds")]
        public async Task<IHttpActionResult> GetHouseHolds(int id)
        {
            HouseHolds houseHolds = await db.HouseHolds.FindAsync(id);
            if (houseHolds == null)
            {
                return NotFound();
            }
            var houseHoldViewModel = new HouseHoldViewModel();
            houseHoldViewModel.Name = houseHolds.Name;
            houseHoldViewModel.Creator = houseHolds.Creator.Email;
            houseHoldViewModel.Members = houseHolds.JoinedUsers.Select(p=>p.Email).ToList();
            return Ok(houseHoldViewModel);
        }
        /// <summary>
        /// Edit HouseHold
        /// </summary>
        /// <param name="id">Id of Household</param>
        /// <param name="houseHolds">Information of HouseHold</param>
        /// <returns></returns>
        // PUT: api/HouseHolds/5
        [ResponseType(typeof(void))]
        [Route("PutHouseHolds")]
        public async Task<IHttpActionResult> PutHouseHolds(int id,HouseHolds houseHolds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var household = db.HouseHolds.Where(p => p.Id == houseHolds.Id).FirstOrDefault();
            if(household == null)
            {
                return Ok("HouseHold is not exist");
            }
            if (string.IsNullOrWhiteSpace(houseHolds.Name))
            {
                return BadRequest("Name Must Required");
            }
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
            return Ok("Success fully edited");
        }
        /// <summary>
        /// View Invited User Of household
        /// </summary>
        /// <param name="id">id of household</param>
        /// <returns></returns>
        [Route("InvitedUserViewList")]
        public IHttpActionResult InvitedUserViewList()
        {
            var userId = User.Identity.GetUserId();
            var invitedUsers = db.HouseHoldInvites.Where(p => p.InvitedUserId==userId).Select(p => new MyHoseholdViewModel
            {
                Id = p.HouseHold.Id,
                Name = p.HouseHold.Name,
                Creator = p.HouseHold.Creator.Email
            }).ToList();
            if (invitedUsers == null)
            {
                var error = "this ID is not found";
                List<string> myList = new List<string> { error };

                return Ok(myList);
            }

            return Ok(invitedUsers);
        }
        [HttpPost]
        [Route("JoinedHouseHold")]
        public IHttpActionResult JoinedHouseHold()
        {
            var userID = User.Identity.GetUserId();
            var houseHoldIds = db.HouseHolds.Where(p => p.JoinedUsers.Any(t => t.Id == userID))
                                .Select(p => new MyHoseholdViewModel
                                {
                                    Id = p.Id,
                                    Name = p.Name
                                })
                .ToList();
            return Ok(houseHoldIds);
        }
        //}
        /// <summary>
        /// Invite a user on household
        /// </summary>
        /// <param name="id">id of household</param>
        /// <param name="email">Email of user</param>
        /// <returns></returns>
        [HttpPost]
        [Route("InviteUser")]
        public IHttpActionResult InviteUser(int id, string email)
        {
            var household = db.HouseHolds.Where(p => p.Id == id).FirstOrDefault();
            var user = db.Users.Where(p => p.UserName == email).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("user is not exist");
            }
            if (household == null)
            {
                return BadRequest("household is not exist");
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
                return Ok("Successfully invited");
            }
            else
            {
                return Ok("This user has been alerady invited");
            }
        }

        /// <summary>
        /// Join the household
        /// </summary>
        /// <param name="id">id of household</param>
        /// <returns></returns>
        [HttpPost]
        [Route("JoinHouseHold")]
        public IHttpActionResult JoinHouseHold(int id)
        {
            var household = db.HouseHolds.Where(p => p.Id == id).FirstOrDefault();
            var userID = User.Identity.GetUserId();
            var user = db.Users.Where(p => p.Id == userID).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("Umm looks like you don't have account. plz register first.");
            }
            if (household != null)
            {
                if (!household.JoinedUsers.Any(p => p.Email == user.Email))
                {
                    var houseHoldInvites = db.HouseHoldInvites.Where(p => p.InvitedUser.Email == user.Email).FirstOrDefault();
                    db.HouseHoldInvites.Remove(houseHoldInvites);
                    household.JoinedUsers.Add(user);
                    db.SaveChanges();
                    return Ok("Successfully Joined");
                }
                else
                {
                    return Ok("This user has been alerady Joined");
                }
            }
            else
            {
                return Ok("household is not exist");
            }
        }
        /// <summary>
        /// Leave a household
        /// </summary>
        /// <param name="id">Id of household</param>
        /// <returns></returns>
        [HttpPost]
        [Route("LeaveHouseHold")]
        public IHttpActionResult LeaveHouseHold(int id)
        {
            var household = db.HouseHolds.Where(p => p.Id == id).FirstOrDefault();
            var userID = User.Identity.GetUserId();
            var user = db.Users.Where(p => p.Id ==userID).FirstOrDefault();
            if (user == null)
            {
                return Ok("This user is not exist in our database");
            }
            if (household != null)
            {
                if (household.JoinedUsers.Any(p => p.Email == user.Email))
                {
                    household.JoinedUsers.Remove(user);
                    db.SaveChanges();
                    return Ok("Successfully Leaved");
                }
                else
                {
                    return Ok("There is no such user in this household");
                }
            }
            else
            {
                return Ok("household is not exist");
            }
        }
        /// <summary>
        /// Create HouseHold
        /// </summary>
        /// <param name="houseHolds">information of houseold</param>
        /// <returns></returns>
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
        /// <summary>
        /// Delete Household
        /// </summary>
        /// <param name="id">Id of houseold</param>
        /// <returns></returns>
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