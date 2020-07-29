using OdeToFood.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdeToFood.Data.Services
{
    public interface IRestaurantData
    {
        IEnumerable<Restaurant> GetAll();

        Restaurant Get(int id);

        void Add(Restaurant restaurant);

        bool Update(Restaurant restaurant, Action<string, string> writeError);

        void Delete(int id);

        void Delete(Restaurant restaurant);

        bool Delete(Restaurant restaurant, Action<string, string> writeError);
    }
}
