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
        private Mock<IGroceryLogRepo> _groceryLogRepo;
        private Mock<IReminderRepo> _reminderRepo;
        private Mock<IDayProfileRepo> _dayProfileRepo;

        [SetUp]
        public void Setup()
        {
            _logRepo = new();
            _groceryLogRepo = new();
            _reminderRepo = new();
            _dayProfileRepo = new();

            _logService = new(_logRepo.Object, _groceryLogRepo.Object, _reminderRepo.Object, _dayProfileRepo.Object);
        }

        [Test]
        async public Task InsertLogAsync_ValidLogWithReminder_ReturnTrue()
        {
            LogModel log = new(new(), new(), DateTime.Now, 2.0f, 2.0f, 5.4f, new());

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));
            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));
            _logRepo.Setup(r => r.InsertLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));
            _logRepo.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(new List<LogModel>() { log }));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new LogModel(x)));
            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));
            _groceryLogRepo.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModel>>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            Assert.True(await _logService.InsertLogAsync(log));
        }

        [Test]
        async public Task InsertLogAsync_ValidLogWithoutReminder_ReturnTrue()
        {
            LogModel log = new(new(), null, DateTime.Now, 2.0f, 2.0f, 5.4f, new() { new(new GroceryModel()) });
            const int NEW_REMINDER_ID = 1;

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));
            _reminderRepo.Setup(r => r.InsertReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(true));
            _reminderRepo.Setup(r => r.GetAllRemindersAsync()).Returns(Task.FromResult(new List<ReminderModel>() { new ReminderModel(NEW_REMINDER_ID) }));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _logRepo.Setup(r => r.InsertLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));
            _logRepo.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(new List<LogModel>() { log }));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new LogModel(x)));
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new List<LogModel>()));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));
            _groceryLogRepo.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModel>>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            Assert.True(await _logService.InsertLogAsync(log));
        }

        [Test]
        async public Task InsertLogAsync_ValidLogWithErrorOnCreateReminder_ReturnFalse()
        {
            LogModel log = new(new(), null, DateTime.Now, 2.0f, 2.0f, 5.4f, new());
            const int NEW_REMINDER_ID = 1;

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));
            _reminderRepo.Setup(r => r.InsertReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(false));
            _reminderRepo.Setup(r => r.GetAllRemindersAsync()).Returns(Task.FromResult(new List<ReminderModel>() { new ReminderModel(NEW_REMINDER_ID) }));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _logRepo.Setup(r => r.InsertLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));
            _logRepo.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(new List<LogModel>() { log }));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new LogModel(x)));
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new List<LogModel>()));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));
            _groceryLogRepo.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModel>>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            Assert.False(await _logService.InsertLogAsync(log));
        }

        [Test]
        async public Task InsertLogAsync_WithInvalidReminder_ReturnFalse()
        {
            LogModel log = new(new(), new(), DateTime.Now, 2.0f, 2.0f, 5.4f, new());

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns(Task.FromResult<ReminderModel>(null));

            Assert.False(await _logService.InsertLogAsync(log));
        }

        [Test]
        async public Task InsertLogAsync_WithInvalidDayProfile_ReturnFalse()
        {
            LogModel log = new(new(), new(), DateTime.Now, 2.0f, 2.0f, 5.4f, new());

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns(Task.FromResult(new ReminderModel()));
            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult<DayProfileModel>(null));

            Assert.False(await _logService.InsertLogAsync(log));
        }

        [Test]
        async public Task InsertLogAsync_WithErrorOnInsertLog_ReturnFalse()
        {
            LogModel log = new(new(), new(), DateTime.Now, 2.0f, 2.0f, 5.4f, new());

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns(Task.FromResult(new ReminderModel()));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel()));

            _logRepo.Setup(r => r.InsertLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(false));

            Assert.False(await _logService.InsertLogAsync(log));
        }

        [Test]
        async public Task InsertLogAsync_ValidLogWithErrorOnInsertGroceryLogs_ReturnFalse()
        {
            LogModel log = new(new(), null, DateTime.Now, 2.0f, 2.0f, 5.4f, new());
            const int NEW_REMINDER_ID = 1;

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));
            _reminderRepo.Setup(r => r.InsertReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(true));
            _reminderRepo.Setup(r => r.GetAllRemindersAsync()).Returns(Task.FromResult(new List<ReminderModel>() { new ReminderModel(NEW_REMINDER_ID) }));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _logRepo.Setup(r => r.InsertLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));
            _logRepo.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(new List<LogModel>() { log }));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new LogModel(x)));
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new List<LogModel>()));
            _logRepo.Setup(r => r.DeleteLogAsync(It.IsAny<int>())).Returns(Task.FromResult(true));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));
            _groceryLogRepo.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModel>>(), It.IsAny<int>())).Returns(Task.FromResult(false));

            Assert.False(await _logService.InsertLogAsync(log));
        }

        [Test]
        async public Task InsertLogAsync_ValidLogWithErrorOnInsertGroceryLogsAndDeleteLog_ReturnFalse()
        {
            LogModel log = new(new(), null, DateTime.Now, 2.0f, 2.0f, 5.4f, new());
            const int NEW_REMINDER_ID = 1;

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));
            _reminderRepo.Setup(r => r.InsertReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(true));
            _reminderRepo.Setup(r => r.GetAllRemindersAsync()).Returns(Task.FromResult(new List<ReminderModel>() { new ReminderModel(NEW_REMINDER_ID) }));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _logRepo.Setup(r => r.InsertLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));
            _logRepo.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(new List<LogModel>() { log }));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new LogModel(x)));
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new List<LogModel>()));
            _logRepo.Setup(r => r.DeleteLogAsync(It.IsAny<int>())).Returns(Task.FromResult(false));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));
            _groceryLogRepo.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModel>>(), It.IsAny<int>())).Returns(Task.FromResult(false));

            try
            {
                await _logService.InsertLogAsync(log);
            }
            catch (Exception e)
            {
                Assert.AreEqual("This state should not be possible", e.Message);
                return;
            }
            Assert.Fail();
        }

        [Test]
        async public Task GetAllLogsAfterDateAsync_WithLogs_ReturnsListWithLogs()
        {
            List<LogModel> logs = new();
            for (int i = -4; i < 2; ++i)
            {
                logs.Add(new LogModel(new(), new(), DateTime.Now.AddDays(i), 1.0f, 1.0f, 5.4f, new()));
                logs[logs.Count - 1].LogID = logs.Count - 1;
            }

            _logRepo.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(logs));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(logs[x]));

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));

            List<LogModel> logsAfterDate = await _logService.GetAllLogsAfterDateAsync(DateTime.Now);

            Assert.NotNull(logsAfterDate);
            Assert.AreEqual(2, logsAfterDate.Count);
        }

        [Test]
        async public Task GetAllLogsOnDateAsync_WithLogs_ReturnsListWithLogs()
        {
            List<LogModel> logs = new();
            for (int i = -4; i < 2; ++i)
            {
                logs.Add(new LogModel(new(), new(), DateTime.Now.AddDays(i), 1.0f, 1.0f, 5.4f, new()));
                logs[logs.Count - 1].LogID = logs.Count - 1;
            }

            _logRepo.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(logs));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(logs[x]));

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));

            List<LogModel> logsOnDate = await _logService.GetAllLogsOnDateAsync(DateTime.Now);

            Assert.NotNull(logsOnDate);
            Assert.AreEqual(1, logsOnDate.Count);
        }

        [Test]
        async public Task GetAllLogsAsync_WithLogs_ReturnsListWithLogs()
        {
            List<LogModel> logs = new();
            for (int i = -4; i < 2; ++i)
            {
                logs.Add(new LogModel(new(), new(), DateTime.Now.AddDays(i), 1.0f, 1.0f, 5.4f, new()));
                logs[logs.Count - 1].LogID = logs.Count - 1;
            }

            _logRepo.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(logs));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(logs[x]));

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));

            List<LogModel> allLogs = await _logService.GetAllLogsAsync();

            Assert.NotNull(allLogs);
            Assert.AreEqual(6, allLogs.Count);
        }

        [Test]
        async public Task DeleteAllLogsAsync_WithValidLogIDs_ReturnsTrue()
        {
            List<int> logIDs = new() { 1, 2, 3 };

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));
            _reminderRepo.Setup(r => r.DeleteReminderAsync(It.IsAny<int>())).Returns(Task.FromResult(true));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new LogModel(x)));
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new List<LogModel>()));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));

            Assert.True(await _logService.DeleteAllLogsAsync(logIDs));
        }

        [Test]
        async public Task DeleteAllLogsAsync_WithInvalidLogIDs_ReturnsFalse()
        {
            List<int> logIDs = new() { 1, 2, 3 };

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));
            _reminderRepo.Setup(r => r.DeleteReminderAsync(It.IsAny<int>())).Returns(Task.FromResult(true));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult<LogModel>(null));
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new List<LogModel>()));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));


            Assert.False(await _logService.DeleteAllLogsAsync(logIDs));
        }

        [Test]
        async public Task DeleteLogAsync_WithValidLogID_ReturnsTrue()
        {
            const int LOG_ID = 1;

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));
            _reminderRepo.Setup(r => r.DeleteReminderAsync(It.IsAny<int>())).Returns(Task.FromResult(true));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new LogModel(LOG_ID)));
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new List<LogModel>() { new LogModel(LOG_ID) }));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));


            Assert.True(await _logService.DeleteLogAsync(LOG_ID));
        }

        [Test]
        async public Task DeleteAllWithDayProfileIDAsync_WithLogs_ReturnsTrue()
        {
            const int DAY_PROFILE_ID = 1;

            List<LogModel> logs = new();
            for (int i = 0; i < 3; ++i)
            {
                LogModel log = new(i);
                log.DayProfile.DayProfileID = DAY_PROFILE_ID;
                logs.Add(log);
            }

            _logRepo.Setup(r => r.GetAllLogsWithDayProfileIDAsync(DAY_PROFILE_ID)).Returns(Task.FromResult(logs));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new LogModel(x)));
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new List<LogModel>()));

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));

            Assert.True(await _logService.DeleteAllWithDayProfileIDAsync(DAY_PROFILE_ID));
        }

        [Test]
        async public Task DeleteAllWithDayProfileIDAsync_WithLogsDeleteFail_ReturnsFalse()
        {
            const int DAY_PROFILE_ID = 1;

            List<LogModel> logs = new();
            for (int i = 0; i < 3; ++i)
            {
                LogModel log = new(i);
                log.DayProfile.DayProfileID = DAY_PROFILE_ID;
                logs.Add(log);
            }

            _logRepo.Setup(r => r.GetAllLogsWithDayProfileIDAsync(DAY_PROFILE_ID)).Returns(Task.FromResult(logs));
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult<LogModel>(null));
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new List<LogModel>()));

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));

            Assert.False(await _logService.DeleteAllWithDayProfileIDAsync(DAY_PROFILE_ID));
        }

        [Test]
        async public Task GetAllLogsWithReminderIDAsync_WithLogs_ReturnsListWithLogs()
        {
            List<LogModel> logs = new();
            for (int i = 0; i < 3; ++i)
            {
                LogModel log = new(i);
                log.DayProfile.DayProfileID = 1;
                logs.Add(log);
            }

            MockGetLogAsyncToSuccess();

            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns(Task.FromResult(logs));

            List<LogModel> logsWithReminderID = await _logService.GetAllLogsWithReminderIDAsync(1);

            Assert.NotNull(logsWithReminderID);
            Assert.AreEqual(logs.Count, logsWithReminderID.Count);
        }

        [Test]
        async public Task GetAllLogsWithDayProfileIDAsync_WithLogs_ReturnsListWithLogs()
        {
            List<LogModel> logs = new();
            for (int i = 0; i < 3; ++i)
            {
                LogModel log = new(i);
                log.DayProfile.DayProfileID = 1;
                logs.Add(log);
            }

            MockGetLogAsyncToSuccess();

            _logRepo.Setup(r => r.GetAllLogsWithDayProfileIDAsync(It.IsAny<int>())).Returns(Task.FromResult(logs));

            List<LogModel> logsWithDayProfileID = await _logService.GetAllLogsWithDayProfileIDAsync(1);

            Assert.NotNull(logsWithDayProfileID);
            Assert.AreEqual(logs.Count, logsWithDayProfileID.Count);
        }

        [Test]
        async public Task GetLogAsync_WithValidLogID_ReturnsTrue()
        {
            const int LOG_ID = 1;

            MockGetLogAsyncToSuccess();

            LogModel log = await _logService.GetLogAsync(LOG_ID);

            Assert.NotNull(log);
            Assert.AreEqual(LOG_ID, log.LogID);
        }

        [Test]
        async public Task GetLogAsync_WithInvalidLogID_ReturnsNullRef()
        {
            const int LOG_ID = -1;

            MockGetLogAsyncToSuccess();

            LogModel log = await _logService.GetLogAsync(LOG_ID);

            Assert.Null(log);
        }

        [Test]
        async public Task GetLogAsync_WithInvalidReminderOrDayProfile_ReturnsNullRef()
        {
            const int LOG_ID = 1;
            const int DAY_PROFILE_ID = 2;
            const int REMINDER_ID = 3;

            _logRepo.Setup(r => r.GetLogAsync(LOG_ID)).Returns(Task.FromResult(new LogModel(LOG_ID)));

            _dayProfileRepo.Setup(r => r.InsertDayProfileAsync(It.IsAny<DayProfileModel>())).Returns(Task.FromResult(true));
            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(DAY_PROFILE_ID)).Returns((int x) => Task.FromResult(new DayProfileModel(x)));
            _dayProfileRepo.Setup(r => r.GetAllDayProfilesAsync()).Returns(Task.FromResult(new List<DayProfileModel>() { new DayProfileModel(DAY_PROFILE_ID) }));


            _reminderRepo.Setup(r => r.InsertReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(true));
            _reminderRepo.Setup(r => r.GetReminderAsync(REMINDER_ID)).Returns((int x) => Task.FromResult(new ReminderModel(x)));
            _reminderRepo.Setup(r => r.GetAllRemindersAsync()).Returns(Task.FromResult(new List<ReminderModel>() { new ReminderModel(REMINDER_ID) }));

            LogModel log = await _logService.GetLogAsync(LOG_ID);

            Assert.Null(log);
        }

        [Test]
        async public Task UpdateLogAsync_WithValidLog_ReturnsTrue()
        {
            const int LOG_ID = 1;

            LogModel log = new(LOG_ID);
            log.NumberOfGroceries.Add(new(new GroceryLogModel()));

            _logRepo.Setup(r => r.UpdateLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));

            _groceryLogRepo.Setup(r => r.DeleteAllGroceryLogsWithLogIDAsync(LOG_ID)).Returns(Task.FromResult(true));
            _groceryLogRepo.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModel>>(), LOG_ID)).Returns(Task.FromResult(true));

            Assert.True(await _logService.UpdateLogAsync(log));
        }

        [Test]
        async public Task UpdateLogAsync_WithErrorInUpdateLog_ReturnsFalse()
        {
            _logRepo.Setup(r => r.UpdateLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(false));

            Assert.False(await _logService.UpdateLogAsync(new()));
        }

        [Test]
        async public Task UpdateLogAsync_WithErrorOnDelete_ReturnsFalse()
        {
            const int LOG_ID = 1;

            LogModel log = new(LOG_ID);
            log.NumberOfGroceries.Add(new(new GroceryLogModel()));

            _logRepo.Setup(r => r.UpdateLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));

            _groceryLogRepo.Setup(r => r.DeleteAllGroceryLogsWithLogIDAsync(LOG_ID)).Returns(Task.FromResult(false));
            _groceryLogRepo.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModel>>(), LOG_ID)).Returns(Task.FromResult(true));

            Assert.False(await _logService.UpdateLogAsync(log));
        }

        [Test]
        async public Task UpdateLogAsync_WithErrorOnInsert_ReturnsFalse()
        {
            const int LOG_ID = 1;

            LogModel log = new(LOG_ID);
            log.NumberOfGroceries.Add(new(new GroceryLogModel()));

            _logRepo.Setup(r => r.UpdateLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));

            _groceryLogRepo.Setup(r => r.DeleteAllGroceryLogsWithLogIDAsync(LOG_ID)).Returns(Task.FromResult(true));
            _groceryLogRepo.Setup(r => r.InsertAllGroceryLogsAsync(It.IsAny<List<GroceryLogModel>>(), LOG_ID)).Returns(Task.FromResult(false));

            Assert.False(await _logService.UpdateLogAsync(log));
        }

        [Test]
        async public Task GetNewestLogAsync_WithNoLogs_ReturnNullRef()
        {
            _logRepo.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(new List<LogModel>()));

            LogModel log = await _logService.GetNewestLogAsync();

            Assert.Null(log);
        }

        private void MockGetLogAsyncToSuccess()
        {
            _logRepo.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int x) =>
            {
                if (x >= 0)
                    return Task.FromResult(new LogModel(x));
                return Task.FromResult<LogModel>(null);
            });

            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));

            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            _groceryLogRepo.Setup(r => r.GetAllGroceryLogsWithLogID(It.IsAny<int>())).Returns(Task.FromResult(new List<GroceryLogModel>()));
        }
    }
}
