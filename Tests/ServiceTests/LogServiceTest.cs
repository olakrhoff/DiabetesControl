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

        [SetUp]
        public void Setup()
        {
            _logRepo = new();
            Task<bool> returnValue = It.IsAny<Task<bool>>();
            _logRepo.Setup(r => r.InsertLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));
            _logService = new(_logRepo.Object);
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
