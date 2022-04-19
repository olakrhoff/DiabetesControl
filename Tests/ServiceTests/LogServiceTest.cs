using NUnit.Framework;
using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using DiabetesContolApp.Service;
using DiabetesContolApp.Repository;
using DiabetesContolApp.Repository.Interfaces;
using DiabetesContolApp.Models;

namespace Tests
{
    [TestFixture]
    public class LogServiceTest
    {
        private LogService _logService;
        private Mock<ILogRepo> _logRepo;
        private Mock<IGroceryLogRepo> _grocerLogRepo;
        private Mock<IReminderRepo> _reminderRepo;
        private Mock<IDayProfileRepo> _dayProfileRepo;

        [SetUp]
        public void Setup()
        {
            _logRepo = new();
            _grocerLogRepo = new();
            _reminderRepo = new();
            _dayProfileRepo = new();

            Task<bool> returnValue = It.IsAny<Task<bool>>();
            _logRepo.Setup(r => r.InsertLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));
            _logRepo.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(new List<LogModel>() { new LogModel() }));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns(Task.FromResult(new LogModel()));
            _grocerLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));
            _grocerLogRepo.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModel>>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns(Task.FromResult(new ReminderModel()));
            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns(Task.FromResult(new DayProfileModel()));

            _logService = new(_logRepo.Object, _grocerLogRepo.Object, _reminderRepo.Object, _dayProfileRepo.Object);
        }

        [Test]
        async public Task InsertLog_ValidLog_ReturnTrue()
        {
            try
            {
                DayProfileModel dayProfile = new();
                List<NumberOfGroceryModel> numberOfGroceries = new List<NumberOfGroceryModel> { new(0, new(), 0), new(0, new(), 0) };

                LogModel newLog = new(dayProfile, new(), DateTime.Now, 5.6f, 5.5f, 5.4f, numberOfGroceries);

                bool result = await _logService.InsertLogAsync(newLog);

                Assert.AreEqual(true, result);
                return;
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine("Error in test");
                Console.WriteLine(nre.StackTrace);
                Console.WriteLine(nre.Message);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in test");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                Assert.Fail();
            }
        }
    }
}
