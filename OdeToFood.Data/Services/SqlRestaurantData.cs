using OdeToFood.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdeToFood.Data.Services
{
    public class SqlRestaurantData : IRestaurantData
    {
        private readonly OdeToFoodDbContext db;

        public SqlRestaurantData(OdeToFoodDbContext db)
        {
            this.db = db;
        }

        public void Add(Restaurant restaurant)
        {
            db.Restaurants.Add(restaurant);

            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var restaurant = db.Restaurants.Find(id);

            db.Restaurants.Remove(restaurant);

            db.SaveChanges();
        }

        public void Delete(Restaurant restaurant)
        {
            db.Entry(restaurant).State = EntityState.Deleted;

            db.SaveChanges();
        }

        public bool Delete(Restaurant restaurant, Action<string, string> writeError)
        {
            db.Entry(restaurant).State = EntityState.Deleted;

            bool result = false;

            try
            {
                db.SaveChanges();

                result = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();

                var databaseEntry = entry.GetDatabaseValues();

                if (databaseEntry == null)
                {
                    result = true;
                }
                else
                {
                    writeError(string.Empty, "The record you attempted to delete "
                        + "was modified by another user after you got the original values. "
                        + "The delete operation was canceled and the current values in the "
                        + "database have been displayed. If you still want to delete this "
                        + "record, click the Delete button again. Otherwise "
                        + "click the Back to List hyperlink.");

                    entry.Reload();
                }
            }
            catch (DataException)
            {
                writeError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
            }

            return result;
        }

        public Restaurant Get(int id)
        {
            return db.Restaurants.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<Restaurant> GetAll()
        {
            return from r in db.Restaurants
                   orderby r.Name
                   select r;
        }

        public bool Update(Restaurant restaurant, Action<string, string> writeError)
        {
            db.Entry(restaurant).State = EntityState.Modified;

            int rowsAffected = 0;

            try
            {
                rowsAffected = db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();

                var clientEntry = entry.CurrentValues;

                var databaseEntry = entry.GetDatabaseValues();

                if (databaseEntry == null)
                {
                    writeError(string.Empty, "Unable to save changes. The object was deleted by another user.");
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
                            writeError(property, "Current value: " + databaseValue);
                        }
                    }

                    writeError(string.Empty, "The record you attempted to edit "
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
                writeError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return rowsAffected > 0;
        }
    }
}
