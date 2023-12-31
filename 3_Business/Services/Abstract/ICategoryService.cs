﻿using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract
{
    public interface ICategoryService : IGenericService<CategoryModel>
    {
        Task<List<CategoryModel>> GetListAsync();
        List<CategoryModel> GetList();
        CategoryModel? Get(int id);
    }

}
