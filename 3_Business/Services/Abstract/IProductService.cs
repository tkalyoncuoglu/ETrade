using Business.Models;
using Business.Results;
using Services.Abstract;
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
    }

}
