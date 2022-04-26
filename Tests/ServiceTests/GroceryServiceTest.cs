using NUnit.Framework;
using Moq;

using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Service;
using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;
using DiabetesContolApp.Repository.Interfaces;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class GroceryServiceTest
    {
        private GroceryService _groceryService;
        private Mock<IGroceryRepo> _groceryRepo;
        private Mock<IGroceryLogRepo> _groceryLogRepo;
        private Mock<ILogRepo> _logRepo;
        private Mock<IReminderRepo> _reminderRepo;
        private Mock<IDayProfileRepo> _dayProfileRepo;

        [SetUp]
        public void Setup()
        {
            _groceryRepo = new();
            _groceryLogRepo = new();
            _logRepo = new();
            _reminderRepo = new();
            _dayProfileRepo = new();

            _groceryRepo.Setup(r => r.GetAllGroceriesAsync()).Returns(Task.FromResult(new List<GroceryModel>()));

            _groceryService = new(_groceryRepo.Object, _groceryLogRepo.Object, _logRepo.Object, _reminderRepo.Object, _dayProfileRepo.Object);
        }

        [Test]
        async public Task GetAllGroceriesAsync_WhenNoGroceries_ReturnEmptyList()
        {
            List<GroceryModel> groceries = await _groceryService.GetAllGroceriesAsync();
            Assert.NotNull(groceries);
        }

        [Test]
        async public Task InsertGroceryAsync_WithValidGrocery_ReturnsTrue()
        {
            _groceryRepo.Setup(r => r.InsertGroceryAsync(It.IsAny<GroceryModel>())).Returns(Task.FromResult(true));

            Assert.True(await _groceryService.InsertGroceryAsync(new GroceryModel()));
        }

        [Test]
        async public Task InsertGroceryAsync_WithNullRefGrocery_ReturnsFalse()
        {
            _groceryRepo.Setup(r => r.InsertGroceryAsync(null)).Returns(Task.FromResult(false));

            Assert.False(await _groceryService.InsertGroceryAsync(null));
        }

        [Test]
        async public Task UpdateGroceryAsync_WithValidGroceryID_ReturnsTrue()
        {
            _groceryRepo.Setup(r => r.UpdateGroceryAsync(It.IsAny<GroceryModel>())).Returns(Task.FromResult(true));

            Assert.True(await _groceryService.UpdateGroceryAsync(new GroceryModel()));
        }

        [Test]
        async public Task UpdateGroceryAsync_WithInvalidGroceryID_ReturnsFalse()
        {
            _groceryRepo.Setup(r => r.UpdateGroceryAsync(It.IsAny<GroceryModel>())).Returns(Task.FromResult(false));

            Assert.False(await _groceryService.UpdateGroceryAsync(new GroceryModel()));
        }

        [Test]
        async public Task UpdateGroceryAsync_WithNullRefGrocery_ReturnsFalse()
        {
            _groceryRepo.Setup(r => r.UpdateGroceryAsync(null)).Returns(Task.FromResult(false));

            Assert.False(await _groceryService.UpdateGroceryAsync(null));
        }

        [Test]
        async public Task DeleteGroceryAsync_WithValidID_ReturnsTrue()
        {
            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithGroceryID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));
            _groceryRepo.Setup(r => r.DeleteGroceryAsync(It.IsAny<int>())).Returns(Task.FromResult(true));

            Assert.True(await _groceryService.DeleteGroceryAsync(8));
        }

        [Test]
        async public Task DeleteGroceryAsync_WithInvalidID_ReturnsFalse()
        {
            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithGroceryID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));
            _groceryRepo.Setup(r => r.DeleteGroceryAsync(It.IsAny<int>())).Returns(Task.FromResult(false));

            Assert.False(await _groceryService.DeleteGroceryAsync(-1));
        }

        [Test]
        async public Task GetGroceryAsync_WithValidID_ReturnsGroceryWithID()
        {
            const int GROCERY_ID = 1;

            _groceryRepo.Setup(r => r.GetGroceryAsync(GROCERY_ID)).Returns(Task.FromResult(new GroceryModel(GROCERY_ID)));

            GroceryModel grocery = await _groceryService.GetGroceryAsync(GROCERY_ID);

            Assert.AreEqual(GROCERY_ID, grocery.GroceryID);
        }

        [Test]
        async public Task GetGroceryAsync_WithInvalidID_ReturnsNull()
        {
            const int GROCERY_ID = -1;

            _groceryRepo.Setup(r => r.GetGroceryAsync(GROCERY_ID)).Returns(Task.FromResult<GroceryModel>(null));

            GroceryModel grocery = await _groceryService.GetGroceryAsync(GROCERY_ID);

            Assert.AreEqual(null, grocery);
        }
    }
}
