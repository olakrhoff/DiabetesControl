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
    public class GroceryLogRepoTest
    {
        private GroceryLogRepo _groceryLogRepo;

        private Mock<IGroceryLogDatabase> _groceryLogDatabase;

        [SetUp]
        public void Setup()
        {
            _groceryLogDatabase = new();

            _groceryLogRepo = new(_groceryLogDatabase.Object);
        }

        [Test]
        async public Task InsertAllGroceryLogsAsync_WithValidList_ReturnsTrue()
        {
            const int LIST_LENGTH = 3;
            List<GroceryLogModel> groceryLogs = new();
            for (int i = 0; i < LIST_LENGTH; i++)
            {
                GroceryLogModel groceryLog = new();
                groceryLog.GroceryLogID = i + 1;
                groceryLog.Log = new();
                groceryLog.Grocery = new();
                groceryLogs.Add(groceryLog);
            }

            _groceryLogDatabase.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModelDAO>>())).Returns(Task.FromResult(LIST_LENGTH));

            Assert.True(await _groceryLogRepo.InsertAllGroceryLogsAsync(groceryLogs));
        }

        [Test]
        async public Task InsertAllGroceryLogsAsync_WithEmptyList_ReturnsTrue()
        {
            Assert.True(await _groceryLogRepo.InsertAllGroceryLogsAsync(new()));
        }

        [Test]
        async public Task InsertAllGroceryLogsAsync_WithValidListErrorInDatabase_ReturnsFalse()
        {
            const int LIST_LENGTH = 3;
            List<GroceryLogModel> groceryLogs = new();
            for (int i = 0; i < LIST_LENGTH; i++)
            {
                GroceryLogModel groceryLog = new();
                groceryLog.GroceryLogID = i + 1;
                groceryLog.Log = new();
                groceryLog.Grocery = new();
                groceryLogs.Add(groceryLog);
            }

            _groceryLogDatabase.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModelDAO>>())).Returns(Task.FromResult(-1));

            Assert.False(await _groceryLogRepo.InsertAllGroceryLogsAsync(groceryLogs));
        }

        [Test]
        async public Task DeleteAllGroceryLogsWithLogIDAsync_WithValidLogID_ReturnsTrue()
        {
            _groceryLogDatabase.Setup(r => r.DeleteAllGroceryLogsWithLogIDAsync(It.IsAny<int>())).Returns(Task.FromResult(1));

            Assert.True(await _groceryLogRepo.DeleteAllGroceryLogsWithLogIDAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task DeleteAllGroceryLogsWithLogIDAsync_WithInvalidLogID_ReturnsFalse()
        {
            _groceryLogDatabase.Setup(r => r.DeleteAllGroceryLogsWithLogIDAsync(It.IsAny<int>())).Returns(Task.FromResult(-1));

            Assert.False(await _groceryLogRepo.DeleteAllGroceryLogsWithLogIDAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task DeleteAllGroceryLogsWithGroceryIDAsync_WithValidGroceryID_ReturnsTrue()
        {
            _groceryLogDatabase.Setup(r => r.DeleteAllGroceryLogsWithGroceryIDAsync(It.IsAny<int>())).Returns(Task.FromResult(1));

            Assert.True(await _groceryLogRepo.DeleteAllGroceryLogsWithGroceryIDAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task DeleteAllGroceryLogsWithGroceryIDAsync_WithInvalidGroceryID_ReturnsFalse()
        {
            _groceryLogDatabase.Setup(r => r.DeleteAllGroceryLogsWithGroceryIDAsync(It.IsAny<int>())).Returns(Task.FromResult(-1));

            Assert.False(await _groceryLogRepo.DeleteAllGroceryLogsWithGroceryIDAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task GetAllGroceryLogsWithGroceryID_WithValidID_ReturnsList()
        {
            const int LIST_LENGTH = 3;
            List<GroceryLogModelDAO> groceryLogDAOs = new();
            for (int i = 0; i < LIST_LENGTH; i++)
            {
                GroceryLogModelDAO groceryLogDAO = new();
                groceryLogDAO.GroceryLogID = i + 1;
                groceryLogDAO.GroceryID = 1;
                groceryLogDAO.LogID = 2;
                groceryLogDAOs.Add(groceryLogDAO);
            }

            _groceryLogDatabase.Setup(r => r.GetAllGroceryLogsWithGroceryID(It.IsAny<int>())).Returns(Task.FromResult(groceryLogDAOs));

            List<GroceryLogModel> groceryLogs = await _groceryLogRepo.GetAllGroceryLogsWithGroceryID(It.IsAny<int>());

            Assert.NotNull(groceryLogs);
            Assert.AreEqual(LIST_LENGTH, groceryLogs.Count);
        }

        [Test]
        async public Task GetAllGroceryLogsWithLogID_WithValidID_ReturnsList()
        {
            const int LIST_LENGTH = 3;
            List<GroceryLogModelDAO> groceryLogDAOs = new();
            for (int i = 0; i < LIST_LENGTH; i++)
            {
                GroceryLogModelDAO groceryLogDAO = new();
                groceryLogDAO.GroceryLogID = i + 1;
                groceryLogDAO.GroceryID = 1;
                groceryLogDAO.LogID = 2;
                groceryLogDAOs.Add(groceryLogDAO);
            }

            _groceryLogDatabase.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(groceryLogDAOs));

            List<GroceryLogModel> groceryLogs = await _groceryLogRepo.GetAllGroceryLogsWithLogID(It.IsAny<int>());

            Assert.NotNull(groceryLogs);
            Assert.AreEqual(LIST_LENGTH, groceryLogs.Count);
        }
    }
}
