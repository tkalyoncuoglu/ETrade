using DataAccess.Contexts;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CountryRepository : GenericRepository<Country> , ICountryRepository
    {
        public CountryRepository(ETradeContext context) : base(context) { }
    }
}
