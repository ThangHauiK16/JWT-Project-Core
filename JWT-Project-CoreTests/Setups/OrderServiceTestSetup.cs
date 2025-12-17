using AutoMapper;
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
    public class OrderServiceTestSetup
    {
        public ApplicationDbContext Context { get; set; }
        public OrderService Service{ get; }
        public IMapper Mapper { get; }
        public OrderServiceTestSetup(string dbName)
        {
            Context = DbContextFactory.Create(dbName);
            Mapper = AutoMapperFactory.Create();
            Service = new OrderService(Context , Mapper);
        }
    }
}
