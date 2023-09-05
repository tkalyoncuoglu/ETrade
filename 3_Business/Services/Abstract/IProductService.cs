using Business.Models;
using Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract
{
    public interface IProductService : IGenericService<ProductModel>
    {
        Result DeleteImage(int id);
        List<StoreModel> GetStores();
        List<CategoryModel> GetCategories();
    }

}
