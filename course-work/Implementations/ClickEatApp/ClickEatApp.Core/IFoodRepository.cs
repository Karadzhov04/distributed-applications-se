using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core
{
    public interface IFoodRepository : IRepository<Food>
    {
        Task<List<Food>> GetFoodsByRestaurantIdAsync(int restaurantId);
    }
}
