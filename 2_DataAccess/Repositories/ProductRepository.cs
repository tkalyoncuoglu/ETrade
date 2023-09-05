﻿using DataAccess.Contexts;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{ 
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ETradeContext context) : base(context) { }
    }
}