using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPlanner.Exceptions
{
    internal class DbUserNotFoundException : DbException
    {
        public DbUserNotFoundException(string user) : base($"Username {user} was not found in the database")
        {

        }
    }
}
