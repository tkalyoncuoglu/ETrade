using DataAccess.Contexts;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ProductStoreRepository : GenericRepository<ProductStore> , IProductStoreRepository
    {
        public ProductStoreRepository(ETradeContext context) : base(context) { }
    }
}
