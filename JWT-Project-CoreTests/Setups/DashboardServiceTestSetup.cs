using JWT_Project_Core.Data;
using JWT_Project_Core.Service;
using JWT_Project_CoreTests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Setups
{
    public class DashboardServiceTestSetup
    {
        public ApplicationDbContext Context { get; }
        public DashboardService Service { get; }
        public DashboardServiceTestSetup(string dbName)
        {
            Context = DbContextFactory.Create(dbName);
            Service = new DashboardService(Context);
        }
    }
}
