using Business.Models;
using DataAccess.Entities;
using Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract
{
    public interface IUserService    {
        Result Add(UserModel model); // Create işlemleri

        UserModel? Get(Expression<Func<User, bool>> expression);
    }
}
