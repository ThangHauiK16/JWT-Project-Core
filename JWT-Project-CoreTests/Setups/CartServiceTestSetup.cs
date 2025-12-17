using AutoMapper;
using JWT_Project_Core.Data;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Service;
using JWT_Project_CoreTests.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Setups
{
    public class CartServiceTestSetup
    {
        public ApplicationDbContext Context { get; }
        public CartService Service { get; }
        public IMapper Mapper { get; }
        public CartServiceTestSetup(string dbName)
        {
            Context = DbContextFactory.Create(dbName);
            Mapper = AutoMapperFactory.Create();

            Service = new CartService(
                Context,
                Mapper
            );
        }
    }
}
