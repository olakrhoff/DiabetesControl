using NUnit.Framework;
using Moq;

using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Repository;
using DiabetesContolApp.Persistence.Interfaces;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Models;
using System;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class LogRepoTest
    {
        private LogRepo _logRepo;

        private Mock<ILogDatabase> _logDatabase;

        [SetUp]
        public void Setup()
        {
            _logDatabase = new();

            _logRepo = new(_logDatabase.Object);
        }

        [Test]
        async public Task InsertLogAsync_WithValidLog_ReturnsTrue()
        {
            _logDatabase.Setup(r => r.InsertLogAsync(It.IsAny<LogModelDAO>())).Returns(Task.FromResult(1));

            Assert.True(await _logRepo.InsertLogAsync(new()));
        }

        [Test]
        async public Task InsertLogAsync_WithInvalidLog_ReturnsFalse()
        {
            _logDatabase.Setup(r => r.InsertLogAsync(It.IsAny<LogModelDAO>())).Returns(Task.FromResult(-1));

            Assert.False(await _logRepo.InsertLogAsync(new()));
        }

        [Test]
        async public Task DeleteLogAsync_WithValidLogID_ReturnsTrue()
        {
            _logDatabase.Setup(r => r.DeleteLogAsync(It.IsAny<int>())).Returns(Task.FromResult(1));

            Assert.True(await _logRepo.DeleteLogAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task DeleteLogAsync_WithInvalidLogID_ReturnsFalse()
        {
            _logDatabase.Setup(r => r.DeleteLogAsync(It.IsAny<int>())).Returns(Task.FromResult(-1));

            Assert.False(await _logRepo.DeleteLogAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task GetAllLogsWithReminderIDAsync_WithResults_ReturnsNonEmptyList()
        {
            const int LIST_LENGTH = 3;
            List<LogModelDAO> logDAOs = new();
            for (int i = 0; i < LIST_LENGTH; ++i)
            {
                LogModelDAO logDAO = new(new LogModel(i + 1));
                logDAOs.Add(logDAO);
            }

            _logDatabase.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns(Task.FromResult(logDAOs));

            List<LogModel> logs = await _logRepo.GetAllLogsWithReminderIDAsync(It.IsAny<int>());

            Assert.NotNull(logs);
            Assert.AreEqual(LIST_LENGTH, logs.Count);
        }

        [Test]
        async public Task GetAllLogsWithDayProfileIDAsync_WithResults_ReturnsNonEmptyList()
        {
            const int LIST_LENGTH = 3;
            List<LogModelDAO> logDAOs = new();
            for (int i = 0; i < LIST_LENGTH; ++i)
            {
                LogModelDAO logDAO = new(new LogModel(i + 1));
                logDAOs.Add(logDAO);
            }

            _logDatabase.Setup(r => r.GetAllLogsWithDayProfileIDAsync(It.IsAny<int>())).Returns(Task.FromResult(logDAOs));

            List<LogModel> logs = await _logRepo.GetAllLogsWithDayProfileIDAsync(It.IsAny<int>());

            Assert.NotNull(logs);
            Assert.AreEqual(LIST_LENGTH, logs.Count);
        }

        [Test]
        async public Task GetAllLogsAsync_WithResults_ReturnsNonEmptyList()
        {
            const int LIST_LENGTH = 3;
            List<LogModelDAO> logDAOs = new();
            for (int i = 0; i < LIST_LENGTH; ++i)
            {
                LogModelDAO logDAO = new(new LogModel(i + 1));
                logDAOs.Add(logDAO);
            }

            _logDatabase.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(logDAOs));

            List<LogModel> logs = await _logRepo.GetAllLogsAsync();

            Assert.NotNull(logs);
            Assert.AreEqual(LIST_LENGTH, logs.Count);
        }

        [Test]
        async public Task GetLogAsync_WithValidLogID_ReturnsLog()
        {
            LogModelDAO logDAO = new(new LogModel(1));

            _logDatabase.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns(Task.FromResult(logDAO));

            LogModel logs = await _logRepo.GetLogAsync(It.IsAny<int>());

            Assert.NotNull(logs);
        }

        [Test]
        async public Task GetLogAsync_WithInvalidLogID_ReturnsNullRef()
        {
            LogModelDAO logDAO = new(new LogModel(-1));

            _logDatabase.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns(Task.FromResult<LogModelDAO>(null));

            LogModel logs = await _logRepo.GetLogAsync(It.IsAny<int>());

            Assert.Null(logs);
        }

        [Test]
        async public Task UpdateLogAsync_WithValidLog_ReturnsTrue()
        {
            LogModelDAO logDAO = new(new LogModel(1));

            _logDatabase.Setup(r => r.UpdateLogAsync(It.IsAny<LogModelDAO>())).Returns(Task.FromResult(1));

            Assert.True(await _logRepo.UpdateLogAsync(new()));
        }

        [Test]
        async public Task UpdateLogAsync_WithInvalidLog_ReturnsFalse()
        {
            LogModelDAO logDAO = new(new LogModel(-1));

            _logDatabase.Setup(r => r.UpdateLogAsync(It.IsAny<LogModelDAO>())).Returns(Task.FromResult(-1));

            Assert.False(await _logRepo.UpdateLogAsync(new()));
        }
    }
}
