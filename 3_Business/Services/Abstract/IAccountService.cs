using Business.Models.Account;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Results;

namespace Services.Abstract
{
    public interface IAccountService
    {
        Result Login(AccountLoginModel accountLoginModel, UserModel userResultModel);
        Result Register(AccountRegisterModel accountRegisterModel);
    }
}
