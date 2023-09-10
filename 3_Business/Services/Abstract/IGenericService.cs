using Business.Models;
using Business.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract
{
    public interface IGenericService<T> where T : class
    {
        Result Add(T model);
        Result Update(T model);
        Result Delete(int id);
        List<T> GetList();
        T? Get(int id);
    }
}
