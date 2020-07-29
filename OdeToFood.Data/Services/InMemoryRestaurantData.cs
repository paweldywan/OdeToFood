using OdeToFood.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OdeToFood.Data.Services
{
    public class InMemoryRestaurantData : IRestaurantData
    {
        readonly List<Restaurant> restaurants;

        public InMemoryRestaurantData()
        {
            restaurants = new List<Restaurant>
            {
                new Restaurant { Id = 1, Name = "Scott's Pizza", Cuisine = CuisineType.Italian },
                new Restaurant { Id = 2, Name = "Tersiguels", Cuisine = CuisineType.French },
                new Restaurant { Id = 3, Name = "Mango Grove", Cuisine = CuisineType.Indian }
            };
        }

        public void Add(Restaurant restaurant)
        {
            restaurants.Add(restaurant);

            restaurant.Id = restaurants.Max(r => r.Id) + 1;
        }

        public bool Update(Restaurant restaurant, Action<string, string> writeError)
        {
            bool success = false;

            var existing = Get(restaurant.Id);

            if (existing != null)
            {
                existing.Name = restaurant.Name;

                existing.Cuisine = restaurant.Cuisine;

                success = true;
            }
            else
            {
                writeError(string.Empty, "Unable to save changes. The object was deleted by another user.");
            }

            return success;
        }

        public Restaurant Get(int id)
        {
            return restaurants.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<Restaurant> GetAll()
        {
            return restaurants.OrderBy(r => r.Name);
        }

        public void Delete(int id)
        {
            var restaurant = Get(id);

            if (restaurant != null)
            {
                restaurants.Remove(restaurant);
            }
        }

        public void Delete(Restaurant restaurant)
        {
            restaurants.Remove(restaurant);
        }

        public bool Delete(Restaurant restaurant, Action<string, string> writeError)
        {
            bool result = false;

            if (restaurants.Contains(restaurant))
            {
                restaurants.Remove(restaurant);

                result = true;
            }
            else
            {
                writeError(string.Empty, "Unable to delete object. The object was deleted by another user.");
            }

            return result;
        }
    }
}
