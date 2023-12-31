﻿using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Abstract
{
    public interface IUserRepository : IGenericRepository<User>
    {
        int GetRoleId(string name);
    }
}
