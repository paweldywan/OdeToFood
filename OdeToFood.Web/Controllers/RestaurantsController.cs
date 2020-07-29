using OdeToFood.Data.Models;
using OdeToFood.Data.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OdeToFood.Web.Controllers
{
    public class RestaurantsController : Controller
    {
        private readonly IRestaurantData db;

        public RestaurantsController(IRestaurantData db)
        {
            this.db = db;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = db.GetAll();

            return View(model);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var model = db.Get(id);

            if (model == null)
            {
                return View("NotFound");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                db.Add(restaurant);

                return RedirectToAction("Details", new { id = restaurant.Id });
            }

            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = db.Get(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                //var restaurantToUpdate = db.Get(restaurant.Id);

                //if(restaurantToUpdate == null)
                //{
                //    ModelState.AddModelError(string.Empty, "Unable to save changes. The restaurant was deleted by another user.");

                //    return View(restaurant);
                //}

                try
                {
                    db.Update(restaurant);

                    TempData["Message"] = "You have saved the restaurant!";

                    return RedirectToAction("Details", new { id = restaurant.Id });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();

                    var clientEntry = entry.CurrentValues; //(Restaurant)entry.Entity;
                    //var clientValues = (Restaurant)entry.Entity;

                    var databaseEntry = entry.GetDatabaseValues();
                    //var databaseValues = (Restaurant)databaseEntry.ToObject();

                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The object was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Restaurant)databaseEntry.ToObject();

                        foreach (var property in clientEntry.PropertyNames)
                        {
                            var databaseValue = databaseEntry[property];

                            var clientValue = clientEntry[property];

                            if (clientValue != databaseValue && !clientValue.Equals(databaseValue))
                            {
                                ModelState.AddModelError(property, "Current value: " + databaseValue);
                            }
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");

                        restaurant.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(restaurant);
        }

        [HttpGet]
        public ActionResult Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = db.Get(id.Value);

            if (model == null)
            {
                //return View("NotFound");
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }

                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Restaurant restaurant /*int id, FormCollection form*/)
        {
            //_ = form;
            try
            {
                db.Delete(restaurant);

                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, restaurant.Id });
            }
            catch (DataException)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");

                //var entry = new Restaurant();

                //TryUpdateModel(entry);

                return View(restaurant);
            }
        }
    }
}