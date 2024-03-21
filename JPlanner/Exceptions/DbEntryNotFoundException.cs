using JPlanner.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPlanner.Exceptions
{
    internal class DbEntryNotFoundException : DbException
    {
        public DbEntryNotFoundException(MealEntry entry, string username, int userId) : base($"MealEntry [{entry}] was not found in the database for username [{username}] with id [{userId}]")
        {

        }
    }
}
