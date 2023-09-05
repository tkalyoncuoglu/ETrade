using DataAccess.Contexts;
using DataAccess.Entities;
using Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Concrete
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        public CityRepository(ETradeContext context) : base(context) { }
    }
}
