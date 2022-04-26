using NUnit.Framework;
using Moq;

using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Repository;
using DiabetesContolApp.Persistence.Interfaces;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Models;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class GroceryRepoTest
    {
        private GroceryRepo _groceryRepo;

        private Mock<IGroceryDatabase> _groceryDatabase;

        [SetUp]
        public void Setup()
        {
            _groceryDatabase = new();

            _groceryRepo = new(_groceryDatabase.Object);
        }

        [Test]
        async public Task GetAllGroceriesAsync_WithGroceries_ReturnsTrue()
        {
            const int LIST_LENGTH = 3;

            List<GroceryModelDAO> groceryDAOs = new();
            for (int i = 0; i < LIST_LENGTH; ++i)
                groceryDAOs.Add(new());

            _groceryDatabase.Setup(r => r.GetAllGroceriesAsync()).Returns(Task.FromResult(groceryDAOs));

            List<GroceryModel> groceries = await _groceryRepo.GetAllGroceriesAsync();

            Assert.NotNull(groceries);
            Assert.AreEqual(LIST_LENGTH, groceries.Count);
        }

        [Test]
        async public Task GetGroceryAsync_WithValidGroceryID_ReturnsGrocery()
        {
            _groceryDatabase.Setup(r => r.GetGroceryAsync(It.IsAny<int>())).Returns(Task.FromResult(new GroceryModelDAO()));

            GroceryModel grocery = await _groceryRepo.GetGroceryAsync(It.IsAny<int>());

            Assert.NotNull(grocery);
        }

        [Test]
        async public Task GetGroceryAsync_WithInvalidGroceryID_ReturnsNullRef()
        {
            _groceryDatabase.Setup(r => r.GetGroceryAsync(It.IsAny<int>())).Returns(Task.FromResult<GroceryModelDAO>(null));

            GroceryModel grocery = await _groceryRepo.GetGroceryAsync(It.IsAny<int>());

            Assert.Null(grocery);
        }

        [Test]
        async public Task UpdateGroceryAsync_WithValidGrocery_ReturnsTrue()
        {
            _groceryDatabase.Setup(r => r.UpdateGroceryAsync(It.IsAny<GroceryModelDAO>())).Returns(Task.FromResult(1));

            Assert.True(await _groceryRepo.UpdateGroceryAsync(new GroceryModel()));
        }

        [Test]
        async public Task UpdateGroceryAsync_WithInvalidGrocery_ReturnsFalse()
        {
            _groceryDatabase.Setup(r => r.UpdateGroceryAsync(It.IsAny<GroceryModelDAO>())).Returns(Task.FromResult(-1));

            Assert.False(await _groceryRepo.UpdateGroceryAsync(new GroceryModel()));
        }

        [Test]
        async public Task DeleteGroceryAsync_WithValidGroceryID_ReturnsTrue()
        {
            _groceryDatabase.Setup(r => r.DeleteGroceryAsync(It.IsAny<int>())).Returns(Task.FromResult(1));

            Assert.True(await _groceryRepo.DeleteGroceryAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task DeleteGroceryAsync_WithInvalidGroceryID_ReturnsFalse()
        {
            _groceryDatabase.Setup(r => r.DeleteGroceryAsync(It.IsAny<int>())).Returns(Task.FromResult(-1));

            Assert.False(await _groceryRepo.DeleteGroceryAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task InsertGroceryAsync_WithValidGrocery_ReturnsTrue()
        {
            _groceryDatabase.Setup(r => r.InsertGroceryAsync(It.IsAny<GroceryModelDAO>())).Returns(Task.FromResult(1));

            Assert.True(await _groceryRepo.InsertGroceryAsync(new GroceryModel()));
        }

        [Test]
        async public Task InsertGroceryAsync_WithInvalidGrocery_ReturnsFalse()
        {
            _groceryDatabase.Setup(r => r.InsertGroceryAsync(It.IsAny<GroceryModelDAO>())).Returns(Task.FromResult(-1));

            Assert.False(await _groceryRepo.InsertGroceryAsync(new GroceryModel()));
        }
    }
}
