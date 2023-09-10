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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ETradeContext context) : base(context) { }

        public int GetRoleId(string name)
        {
            var role = _context.Set<Role>().Where(x => x.Name == name).FirstOrDefault();

            if (role == null)
            {
                throw new Exception("Role with given name does not exist!");
            }

            return role.Id;
        }
    }
}
