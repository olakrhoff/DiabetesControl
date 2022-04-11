
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;

using DiabetesContolApp.Repository;
using DiabetesContolApp.Service;
using DiabetesContolApp.Models;

namespace Test.ServiceTests
{
    [TestFixture]
    public class LogServiceTests
    {

        private LogService logService;
        private Mock<LogRepo> logRepo;

        [SetUp]
        public void Setup()
        {
            logRepo = new();
            logService = new();
        }

        [Test]
        public void Pass()
        {
            Assert.True(true);
        }

        [Test]
        public void Fail()
        {
            Assert.False(true);
        }

        [Test]
        [Ignore("another time")]
        public void Ignore()
        {
            Assert.True(false);
        }

        [Test]
        async public Task InsertLog_ValidLog_ReturnTrue()
        {
            try
            {
                DayProfileModel dayProfile = new();
                List<NumberOfGroceryModel> numberOfGroceries = new List<NumberOfGroceryModel> { new(0, new(), 0), new(0, new(), 0) };

                LogModel newLog = new(dayProfile, new(), DateTime.Now, 5.6f, 5.5f, 5.4f, numberOfGroceries);

                bool result = await logService.InsertLogAsync(newLog);

                Assert.AreEqual(result, true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.Message);
                Assert.Fail();
            }

        }
    }
}
