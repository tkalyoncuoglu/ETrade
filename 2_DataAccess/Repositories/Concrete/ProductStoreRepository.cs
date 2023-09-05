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
    public class ProductStoreRepository : GenericRepository<ProductStore>, IProductStoreRepository
    {
        public ProductStoreRepository(ETradeContext context) : base(context) { }
    }
}
