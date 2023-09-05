using Business.Models;
using Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IProductService : IGenericService<ProductModel>  
    {
        Result DeleteImage(int id); 
    }

}
