using NUnit.Framework;
using Moq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;

using MathNet.Numerics;

using DiabetesContolApp.GlobalLogic;
using DiabetesContolApp.GlobalLogic.Interfaces;
using DiabetesContolApp.Service.Interfaces;
using DiabetesContolApp.Models;
using DiabetesContolApp;
using DiabetesContolApp.Service;

namespace Tests.GlobalLogicTests
{
    [TestFixture]
    public class AlgorithmTest
    {
        private Mock<IReminderService> _reminderService;
        private Mock<ILogService> _logService;
        private Mock<IDayProfileService> _dayProfileService;
        private Mock<IApplicationProperties> _applicationProperies;

        //Emulate Database
        private List<ReminderModel> _reminderDB;
        private List<LogModel> _logDB;
        private List<DayProfileModel> _dayProfileDB;

        [SetUp]
        public void Setup()
        {
            _reminderService = new();
            _logService = new();
            _dayProfileService = new();
            _applicationProperies = new();

            Algorithm._reminderService = _reminderService.Object;
            Algorithm._logService = _logService.Object;
            Algorithm._dayProfileService = _dayProfileService.Object;
            Algorithm._applicationProperties = _applicationProperies.Object;

            _dayProfileDB = new();
            _logDB = new();
            _reminderDB = new();

            _dayProfileDB.AddRange(new List<DayProfileModel>()
            {
                new DayProfileModel(1, 5.4f, "Frokost", new DateTime(2000, 5, 5, 6, 0, 0), 1.5f, 1.5f),
                new DayProfileModel(2, 5.4f, "Lunsj", new DateTime(2000, 5, 5, 10, 30, 0), 1.0f, 1.0f),
                new DayProfileModel(3, 5.4f, "Middag", new DateTime(2000, 5, 5, 15, 30, 0), 1.0f, 1.0f),
                new DayProfileModel(4, 6.5f, "Kveldsmat", new DateTime(2000, 5, 5, 19, 30, 0), 1.0f, 1.0f)
            });

            //All reminders are in the morning
            _reminderDB.AddRange(new List<ReminderModel>()
            {
                new ReminderModel(1, 7.8f, new(), false, new DateTime(2022, 3, 4, 6, 58, 0)),
                new ReminderModel(2, 8.5f, new(), false, new DateTime(2022, 3, 5, 7, 0, 0)),
                new ReminderModel(3, 7.2f, new(), false, new DateTime(2022, 3, 6, 6, 47, 0)),
                new ReminderModel(4, 8.1f, new(), false, new DateTime(2022, 3, 7, 7, 10, 0)),
                new ReminderModel(5, 10.3f, new(), false, new DateTime(2022, 3, 8, 6, 40, 0)),
                new ReminderModel(6, 6.2f, new(), false, new DateTime(2022, 3, 9, 7, 10, 0)),
                new ReminderModel(7, 7.1f, new(), false, new DateTime(2022, 3, 10, 7, 5, 0)),
                new ReminderModel(8, 6.8f, new(), false, new DateTime(2022, 3, 11, 6, 10, 0)),
                new ReminderModel(9, 11.8f, new(), false, new DateTime(2022, 3, 12, 7, 12, 0)),
                new ReminderModel(10, 4.2f, new(), false, new DateTime(2022, 3, 13, 7, 1, 0)),
            });


            _logDB.AddRange(new List<LogModel>()
            {
                new LogModel(1, new(1), new(1), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new()),
                new LogModel(1, new(1), new(2), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new()),
                new LogModel(1, new(1), new(3), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new()),
                new LogModel(1, new(1), new(4), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new()),
                new LogModel(1, new(1), new(5), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new()),
                new LogModel(1, new(1), new(6), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new()),
                new LogModel(1, new(1), new(7), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new()),
                new LogModel(1, new(1), new(8), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new()),
                new LogModel(1, new(1), new(9), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new()),
                new LogModel(1, new(1), new(10), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new()),
            });

            _logDB.ForEach(log =>
            {
                log.Reminder = _reminderDB.Find(r => r.ReminderID == log.Reminder.ReminderID);
                log.DayProfile = _dayProfileDB.Find(d => d.DayProfileID == log.DayProfile.DayProfileID);
            });

            _reminderDB.ForEach(reminder =>
            {
                reminder.Logs = _logDB.FindAll(log => log.Reminder.ReminderID == reminder.ReminderID);
            });
        }

        [Test]
        async public Task RunStatisticsOnReminder_ReminderEqualsNullRef_ReturnsFalse()
        {
            Assert.False(await Algorithm.RunStatisticsOnReminder(null));
        }

        [Test]
        async public Task RunStatisticsOnReminder_UpdateReminderFail_ReturnsFalse()
        {
            _reminderService.Setup(r => r.UpdateReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(false));
            Assert.False(await Algorithm.RunStatisticsOnReminder(new ReminderModel()));
        }

        [Test]
        async public Task RunStatisticsOnReminder_GetReminderReturnsNull_ReturnsFalse()
        {
            _reminderService.Setup(r => r.UpdateReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(true));
            _reminderService.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns(Task.FromResult<ReminderModel>(null));
            Assert.False(await Algorithm.RunStatisticsOnReminder(new ReminderModel()));
        }

        [Test]
        async public Task RunStatisticsOnReminder_GetLogReturnsNull_ReturnsFalse()
        {
            ReminderModel reminder = _reminderDB[^1];
            _reminderService.Setup(r => r.UpdateReminderAsync(reminder)).Returns(Task.FromResult(true));
            _reminderService.Setup(r => r.GetReminderAsync(reminder.ReminderID)).Returns(Task.FromResult(reminder));

            _logService.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns(Task.FromResult<LogModel>(null));

            Assert.False(await Algorithm.RunStatisticsOnReminder(reminder));
        }

        [Test]
        async public Task PartitionGlucoseErrorToLogs_ReminderHasNotValidGlucoseAfterMeal_ThrowsArgumentNullException_ReturnsNullRef()
        {
            ReminderModel reminder = _reminderDB[^1];
            reminder.GlucoseAfterMeal = null; //Set it invalid
            _reminderService.Setup(r => r.UpdateReminderAsync(reminder)).Returns(Task.FromResult(true));
            _reminderService.Setup(r => r.GetReminderAsync(reminder.ReminderID)).Returns(Task.FromResult(reminder));

            _logService.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int id) =>
            {
                LogModel log = _logDB.Find(log => log.LogID == id);
                return Task.FromResult(log);
            });


            Assert.False(await Algorithm.RunStatisticsOnReminder(reminder));
        }

        [Test]
        async public Task PartitionGlucoseErrorToLogs_TotalInsulinGivenIsZero_ReturnsNull_ReturnsFalse()
        {
            ReminderModel reminder = _reminderDB[^1];
            _reminderService.Setup(r => r.UpdateReminderAsync(reminder)).Returns(Task.FromResult(true));
            _reminderService.Setup(r => r.GetReminderAsync(reminder.ReminderID)).Returns(Task.FromResult(reminder));

            _logService.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int id) =>
            {
                LogModel log = _logDB.Find(log => log.LogID == id);
                log.InsulinEstimate = 0; //Create scenario
                return Task.FromResult(log);
            });


            Assert.False(await Algorithm.RunStatisticsOnReminder(reminder));
        }

        [Test]
        async public Task PartitionGlucoseErrorToLogs_MissingDayProfile_ThrowsNullReferenceException_ReturnsNull_ReturnsFalse()
        {
            ReminderModel reminder = _reminderDB[^1];
            _reminderService.Setup(r => r.UpdateReminderAsync(reminder)).Returns(Task.FromResult(true));
            _reminderService.Setup(r => r.GetReminderAsync(reminder.ReminderID)).Returns(Task.FromResult(reminder));

            _logService.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int id) =>
            {
                LogModel log = _logDB.Find(log => log.LogID == id);
                log.DayProfile = null;
                return Task.FromResult(log);
            });


            Assert.False(await Algorithm.RunStatisticsOnReminder(reminder));
        }

        [Test]
        async public Task PartitionGlucoseErrorToLogs_UpdateLogFailed_RetursNull_ReturnsFalse()
        {
            ReminderModel reminder = _reminderDB[^1];
            _reminderService.Setup(r => r.UpdateReminderAsync(reminder)).Returns(Task.FromResult(true));
            _reminderService.Setup(r => r.GetReminderAsync(reminder.ReminderID)).Returns(Task.FromResult(reminder));

            _logService.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int id) =>
            {
                LogModel log = _logDB.Find(log => log.LogID == id);
                return Task.FromResult(log);
            });

            _logService.Setup(r => r.UpdateLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(false));


            Assert.False(await Algorithm.RunStatisticsOnReminder(reminder));
        }

        [Test]
        async public Task UpdateDayProfileScalars_ErrorOnGetDayProfile_ThrowsNullReferenceException_ReturnsFalse()
        {
            ReminderModel reminder = _reminderDB[^1];
            _reminderService.Setup(r => r.UpdateReminderAsync(reminder)).Returns(Task.FromResult(true));
            _reminderService.Setup(r => r.GetReminderAsync(reminder.ReminderID)).Returns(Task.FromResult(reminder));

            _logService.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns((int id) =>
            {
                LogModel log = _logDB.Find(log => log.LogID == id);
                return Task.FromResult(log);
            });

            _logService.Setup(r => r.UpdateLogAsync(It.IsAny<LogModel>())).Returns(Task.FromResult(true));

            _dayProfileService.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns(Task.FromResult<DayProfileModel>(null));

            _applicationProperies.Setup(r => r.GetProperty<float>(It.IsAny<string>())).Returns(2.0f);

            Assert.False(await Algorithm.RunStatisticsOnReminder(reminder));
        }
    }
}
