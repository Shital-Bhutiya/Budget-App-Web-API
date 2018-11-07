﻿using System;
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
    public class CategoriesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Categories
        public async Task<IHttpActionResult> GetCategories()
        {
            var categoryHouseholdViewModel = new CategoryHouseholdViewModel();
            categoryHouseholdViewModel.AllCategory = db.Categories.Select(i => new AllCategoryViewModel { Name = i.Name, Id = i.Id, HouseHoldName = i.Household.Name }).ToList();
            return Ok(categoryHouseholdViewModel.AllCategory);
        }

        // GET: api/Categories/5
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> GetCategory(int id)
        {
            var categoryHouseholdViewModel = new CategoryHouseholdViewModel();
            var household = db.HouseHolds.Where(p => p.Id == id).FirstOrDefault();
            if (household == null)
            {
                return Ok("Household is not exist");
            }
            if (!household.JoinedUsers.Any(p => p.Id == User.Identity.GetUserId() || household.CreatorId != User.Identity.GetUserId()))
            {
                return Ok("You are not authorized to get categories");
            }

            categoryHouseholdViewModel.HouseholdName = household.Name;

            categoryHouseholdViewModel.IndividualCategories = db.Categories
                                .Where(p => p.HouseholdId == household.Id)
                                .Select(i => new IndividualCategoryViewModel { Name = i.Name, Id = i.Id })
                                .ToList();
            if (categoryHouseholdViewModel.IndividualCategories == null)
            {
                return NotFound();
            }
            return Ok(categoryHouseholdViewModel);
        }

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCategory(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbCategory = db.Categories.FirstOrDefault(p => p.Id == id);
            if (dbCategory == null)
            {
                return Ok("Thre is no category with this id");
            }
            if ((!dbCategory.Household.JoinedUsers.Any(p=>p.Id == User.Identity.GetUserId())) || (dbCategory.Household.CreatorId != User.Identity.GetUserId()))
            {
                return BadRequest("You are not Authorize");
            }
            if (id != category.Id)
            {
                return BadRequest();
            }
            
            if(category.Name != null)
            {
                dbCategory.Name = category.Name;
            }
            try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok("Successfully Updated");
            }

        // POST: api/Categories
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> PostCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!db.Categories.Any(p => p.HouseholdId == category.HouseholdId))
            {
                return Ok("we don't have any household of that id that you gave us");
            }
            var categories = db.Categories.Where(p => p.HouseholdId == category.HouseholdId).Select(p => p.Name).ToList();
            if (categories.Contains(category.Name))
            {
                return Ok("We have same category with this name on this household");
            }
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            Category category = db.Categories.FirstOrDefault(p => p.Id == id);
            if ((category.Household.JoinedUsers.Any(p => p.Id == User.Identity.GetUserId())) || (category.Household.CreatorId != User.Identity.GetUserId()))
            {
                return BadRequest("You are not Authorize to delete");
            }
            if (category == null)
            {
                return BadRequest("Category not found");
            }

            db.Categories.Remove(category);
            await db.SaveChangesAsync();

            return Ok("Successfully removed");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(int id)
        {
            return db.Categories.Count(e => e.Id == id) > 0;
        }
    }
}