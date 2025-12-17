using JWT_Project_Core.DTO;
using JWT_Project_Core.Model;
using JWT_Project_CoreTests.Setups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Services
{
    [TestClass]
    public class BookServiceTests
    {
        private BookServiceTestSetup _setup = null!;

        [TestInitialize]
        public void Init()
        {
            _setup = new BookServiceTestSetup(Guid.NewGuid().ToString());
        }

    
        [TestMethod]
        public async Task GetAllAsync_ShouldReturnOnlyNotDeletedBooks()
        {
            _setup.Context.Books.AddRange(
                new Book { MaSach = "B1", TenSach = "Book 1", IsDeleted = false },
                new Book { MaSach = "B2", TenSach = "Book 2", IsDeleted = true }
            );
            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.GetAllAsync();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("B1", result.First().MaSach);
        }

  
        [TestMethod]
        public async Task GetByMaSach_WhenExists_ShouldReturnBook()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "MS01",
                TenSach = "Clean Code",
                IsDeleted = false
            });
            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.GetByMaSach("MS01");

            Assert.IsNotNull(result);
            Assert.AreEqual("Clean Code", result.TenSach);
        }

        [TestMethod]
        public async Task GetByMaSach_WhenNotExists_ShouldReturnNull()
        {
            var result = await _setup.Service.GetByMaSach("NOT_EXIST");

            Assert.IsNull(result);
        }

     
        [TestMethod]
        public async Task AddAsync_WhenValid_ShouldAddBook()
        {
            var dto = new BookDTO
            {
                MaSach = "ADD01",
                TenSach = "ASP.NET Core",
                GiaBan = 150000
            };

            var result = await _setup.Service.AddAsync(dto);

            Assert.IsNotNull(result);

            var bookInDb = await _setup.Context.Books.FindAsync("ADD01");
            Assert.IsNotNull(bookInDb);
            Assert.AreEqual("ASP.NET Core", bookInDb.TenSach);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task AddAsync_WhenDuplicateMaSach_ShouldThrow()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "DUP01",
                TenSach = "Old Book"
            });
            await _setup.Context.SaveChangesAsync();

            await _setup.Service.AddAsync(new BookDTO
            {
                MaSach = "DUP01",
                TenSach = "New Book"
            });
        }

    
        [TestMethod]
        public async Task UpdateAsync_WhenExists_ShouldUpdateBook()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "UP01",
                TenSach = "Old Name",
                GiaBan = 100000
            });
            await _setup.Context.SaveChangesAsync();

            var dto = new BookDTO
            {
                TenSach = "New Name",
                GiaBan = 200000
            };

            var result = await _setup.Service.UpdateAsync(dto, "UP01");

            Assert.IsNotNull(result);
            Assert.AreEqual("New Name", result.TenSach);
            Assert.AreEqual(200000, result.GiaBan);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenNotExists_ShouldReturnNull()
        {
            var result = await _setup.Service.UpdateAsync(new BookDTO
            {
                TenSach = "Does not matter"
            }, "NOT_EXIST");

            Assert.IsNull(result);
        }

       
        [TestMethod]
        public async Task DeleteAsync_WhenExists_ShouldReturnTrue()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "DEL01",
                IsDeleted = false
            });
            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.DeleteAsync("DEL01");

            Assert.IsTrue(result);

            var book = await _setup.Context.Books.FindAsync("DEL01");
            Assert.IsTrue(book!.IsDeleted);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenNotExists_ShouldReturnFalse()
        {
            var result = await _setup.Service.DeleteAsync("NONE");

            Assert.IsFalse(result);
        }

   
        [TestMethod]
        public async Task GetAllCategoriesAsync_ShouldReturnDistinctCategories()
        {
            _setup.Context.Books.AddRange(
                new Book { MaSach = "1", TheLoai = "IT", IsDeleted = false },
                new Book { MaSach = "2", TheLoai = "IT", IsDeleted = false },
                new Book { MaSach = "3", TheLoai = "Math", IsDeleted = false }
            );
            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.GetAllCategoriesAsync();

            Assert.AreEqual(2, result.Count());
        }
    }
}
