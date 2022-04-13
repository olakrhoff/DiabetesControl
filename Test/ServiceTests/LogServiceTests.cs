
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;

using DiabetesContolApp.Persistence;
using DiabetesContolApp.Repository;
using DiabetesContolApp.Service;
using DiabetesContolApp.Models;

using Xamarin.Forms;
using SQLite;
using System.IO;

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
            Xamarin.Forms.Forms.Init();
            logRepo = new();

            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentPath, "MySQLite.db");

            SQLiteAsyncConnection connection = new(path);
            logService = new(new LogRepo(new LogDatabase()), connection);
        }

        [Test]
        public void InsertLog_ValidLog_ReturnTrue()
        {
            try
            {
                bool result = Task.Run(async () =>
                {
                    DayProfileModel dayProfile = new();
                    List<NumberOfGroceryModel> numberOfGroceries = new List<NumberOfGroceryModel> { new(0, new(), 0), new(0, new(), 0) };

                    LogModel newLog = new(dayProfile, new(), DateTime.Now, 5.6f, 5.5f, 5.4f, numberOfGroceries);

                    return await logService.InsertLogAsync(newLog);
                }).GetAwaiter().GetResult();
                

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
