using AutoMapper;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Infrastructure
{
    public static class AutoMapperFactory
    {
        public static IMapper Create()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(BookService).Assembly);
                cfg.AddMaps(typeof(CartService).Assembly);
                cfg.AddMaps(typeof(OrderService).Assembly);
                cfg.AddMaps(typeof(UserService).Assembly);
                cfg.AddMaps(typeof(AuthService).Assembly);
            });

            return config.CreateMapper();
        }
    }
}
