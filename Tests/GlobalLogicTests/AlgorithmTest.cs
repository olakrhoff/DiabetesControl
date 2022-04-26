using NUnit.Framework;
using Moq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.GlobalLogic;
using DiabetesContolApp.GlobalLogic.Interfaces;
using DiabetesContolApp.Service.Interfaces;
using DiabetesContolApp.Models;

namespace Tests.GlobalLogicTests
{
    [TestFixture]
    public class AlgorithmTest
    {
        private Mock<IReminderService> _reminderService;
        private Mock<ILogService> _logService;
        private Mock<IDayProfileService> _dayProfileService;
        private Mock<IScalarService> _scalarService;
        private Mock<IGroceryService> _groceryService;
        private Mock<IApplicationProperties> _applicationProperies;

        //Emulate Database
        private List<ReminderModel> _reminderDB;
        private List<LogModel> _logDB;
        private List<DayProfileModel> _dayProfileDB;
        private List<ScalarModel> _scalarDB;
        private List<GroceryModel> _groceryDB;

        [SetUp]
        public void Setup()
        {
            _reminderService = new();
            _logService = new();
            _dayProfileService = new();
            _scalarService = new();
            _groceryService = new();
            _applicationProperies = new();

            AlgorithmBase._reminderService = _reminderService.Object;
            AlgorithmBase._logService = _logService.Object;
            AlgorithmBase._dayProfileService = _dayProfileService.Object;
            AlgorithmBase._scalarService = _scalarService.Object;
            AlgorithmBase._groceryService = _groceryService.Object;
            AlgorithmBase._applicationProperties = _applicationProperies.Object;

            _dayProfileDB = new();
            _logDB = new();
            _reminderDB = new();
            _scalarDB = new();
            _groceryDB = new();

            _groceryDB.AddRange(new List<GroceryModel>()
            {
                new GroceryModel(1, 41.0f, "En brødskive", 33.0f, "Brød", "REMA", 1.0f)
            });

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
                new ReminderModel(1, 7.8f, new(), false, new DateTime(2022, 3, 4, 9, 58, 0)),
                new ReminderModel(2, 8.5f, new(), false, new DateTime(2022, 3, 5, 10, 0, 0)),
                new ReminderModel(3, 7.2f, new(), false, new DateTime(2022, 3, 6, 9, 47, 0)),
                new ReminderModel(4, 8.1f, new(), false, new DateTime(2022, 3, 7, 10, 10, 0)),
                new ReminderModel(5, 10.3f, new(), false, new DateTime(2022, 3, 8, 9, 40, 0)),
                new ReminderModel(6, 6.2f, new(), false, new DateTime(2022, 3, 9, 10, 10, 0)),
                new ReminderModel(7, 7.1f, new(), false, new DateTime(2022, 3, 10, 10, 5, 0)),
                new ReminderModel(8, 6.8f, new(), false, new DateTime(2022, 3, 11, 9, 10, 0)),
                new ReminderModel(9, 11.8f, new(), false, new DateTime(2022, 3, 12, 10, 12, 0)),
                new ReminderModel(10, 4.2f, new(), false, new DateTime(2022, 3, 13, 10, 30, 0)),
            });


            _logDB.AddRange(new List<LogModel>()
            {
                new LogModel(1, new(1), new(1), new DateTime(2022, 3, 4, 6, 58, 0), 9.92f, 9.5f, 11.8f, 4.36f, new List<NumberOfGroceryModel>(){ new NumberOfGroceryModel(1, _groceryDB[0], 2.0f) }, 6.5f),
                new LogModel(2, new(1), new(2), new DateTime(2022, 3, 5, 7, 0, 0), 9.92f, 9.5f, 11.8f, 4.36f, new List<NumberOfGroceryModel>(){ new NumberOfGroceryModel(1, _groceryDB[0], 2.0f) }, 5.6f),
                new LogModel(3, new(1), new(3), new DateTime(2022, 3, 6, 6, 47, 0), 9.92f, 9.5f, 11.8f, 4.36f, new List<NumberOfGroceryModel>(){ new NumberOfGroceryModel(1, _groceryDB[0], 2.0f) }, 7.8f),
                new LogModel(4, new(1), new(4), new DateTime(2022, 3, 7, 7, 10, 0), 9.92f, 9.5f, 11.8f, 4.36f, new List<NumberOfGroceryModel>(){ new NumberOfGroceryModel(1, _groceryDB[0], 2.0f) }, 4.8f),
                new LogModel(5, new(1), new(5), new DateTime(2022, 3, 8, 6, 40, 0), 9.92f, 9.5f, 11.8f, 4.36f, new List<NumberOfGroceryModel>(){ new NumberOfGroceryModel(1, _groceryDB[0], 2.0f) }, 8.2f),
                new LogModel(6, new(1), new(6), new DateTime(2022, 3, 9, 7, 10, 0), 9.92f, 9.5f, 11.8f, 4.36f, new List<NumberOfGroceryModel>(){ new NumberOfGroceryModel(1, _groceryDB[0], 2.0f) }, 6.8f),
                new LogModel(7, new(1), new(7), new DateTime(2022, 3, 10, 7, 5, 0), 9.92f, 9.5f, 11.8f, 4.36f, new List<NumberOfGroceryModel>(){ new NumberOfGroceryModel(1, _groceryDB[0], 2.0f) }, 7.8f),
                new LogModel(8, new(1), new(8), new DateTime(2022, 3, 11, 6, 10, 0), 9.92f, 9.5f, 11.8f, 4.36f, new List<NumberOfGroceryModel>(){ new NumberOfGroceryModel(1, _groceryDB[0], 2.0f) }, 9.8f),
                new LogModel(9, new(1), new(9), new DateTime(2022, 3, 12, 7, 12, 0), 9.92f, 9.5f, 11.8f, 4.36f, new List<NumberOfGroceryModel>(){ new NumberOfGroceryModel(1, _groceryDB[0], 2.0f) }, 5.6f),
                new LogModel(10, new(1), new(10), new DateTime(2022, 3, 13, 7, 1, 0), 9.92f, 9.5f, 11.8f, 4.36f, new List<NumberOfGroceryModel>(){ new NumberOfGroceryModel(1, _groceryDB[0], 2.0f) }, 6.7f),
                new LogModel(11, new(1), new(10), new DateTime(2022, 3, 13, 7, 30, 0), 1.1f, 1.0f, 5.4f, 0.0f, new(), 5.5f)
            });

            _scalarDB.AddRange(new List<ScalarModel>()
            {
                new ScalarModel(1, ScalarTypes.DAY_PROFILE_CARB, 1, 1.0f, new DateTime(2022, 3, 3)),
                new ScalarModel(2, ScalarTypes.DAY_PROFILE_GLUCOSE, 1, 1.0f, new DateTime(2022, 3, 3)),
                new ScalarModel(3, ScalarTypes.CORRECTION_INSULIN, -1, 1.0f, new DateTime(2022, 3, 3)),
                new ScalarModel(4, ScalarTypes.GROCERY, 1, 1.0f, new DateTime(2022, 3, 3))
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
            Assert.False(await AlgorithmBase.RunStatisticsOnReminder(null));
        }

        [Test]
        async public Task RunStatisticsOnReminder_UpdateReminderFail_ReturnsFalse()
        {
            _reminderService.Setup(r => r.UpdateReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(false));
            Assert.False(await AlgorithmBase.RunStatisticsOnReminder(new ReminderModel()));
        }

        [Test]
        async public Task RunStatisticsOnReminder_GetReminderReturnsNull_ReturnsFalse()
        {
            _reminderService.Setup(r => r.UpdateReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(true));
            _reminderService.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns(Task.FromResult<ReminderModel>(null));
            Assert.False(await AlgorithmBase.RunStatisticsOnReminder(new ReminderModel()));
        }

        [Test]
        async public Task RunStatisticsOnReminder_GetLogReturnsNull_ReturnsFalse()
        {
            ReminderModel reminder = _reminderDB[^1];
            _reminderService.Setup(r => r.UpdateReminderAsync(reminder)).Returns(Task.FromResult(true));
            _reminderService.Setup(r => r.GetReminderAsync(reminder.ReminderID)).Returns(Task.FromResult(reminder));

            _logService.Setup(r => r.GetLogAsync(It.IsAny<int>())).Returns(Task.FromResult<LogModel>(null));

            Assert.False(await AlgorithmBase.RunStatisticsOnReminder(reminder));
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


            List<LogModel> logs = await AlgorithmBase.PartitionGlucoseErrorToLogs(reminder);

            Assert.Null(logs);
        }

        [Test]
        async public Task PartitionGlucoseErrorToLogs_TotalInsulinGivenIsZero_ReturnsNull()
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

            List<LogModel> logs = await AlgorithmBase.PartitionGlucoseErrorToLogs(reminder);

            Assert.Null(logs);
        }

        [Test]
        async public Task PartitionGlucoseErrorToLogs_MissingDayProfile_ThrowsNullReferenceException_ReturnsNull()
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


            List<LogModel> logs = await AlgorithmBase.PartitionGlucoseErrorToLogs(reminder);

            Assert.Null(logs);
        }

        [Test]
        async public Task PartitionGlucoseErrorToLogs_UpdateLogFailed_RetursNull()
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


            List<LogModel> logs = await AlgorithmBase.PartitionGlucoseErrorToLogs(reminder);

            Assert.Null(logs);
        }

        [Test]
        async public Task UpdateDayProfileScalars_ErrorInUpdateDayProfile_ReturnsFalse()
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

            _dayProfileService.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int id) =>
            {
                DayProfileModel dayProfile = _dayProfileDB.Find(d => d.DayProfileID == id);
                return Task.FromResult<DayProfileModel>(dayProfile);
            });

            _applicationProperies.Setup(r => r.GetProperty<float>(It.IsAny<string>())).Returns(2.0f);

            Assert.False(await AlgorithmBase.UpdateScalarValues(reminder.Logs));
        }

        [Test]
        async public Task UpdateDayProfileScalars_ErrorInUpdateCorrectionInsulin_ReturnsFalse()
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
            _logService.Setup(r => r.GetAllLogsWithDayProfileIDAsync(It.IsAny<int>())).Returns((int id) =>
            {
                return Task.FromResult(_logDB.FindAll(log => log.DayProfile.DayProfileID == id));
            });

            _logService.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(_logDB));

            _dayProfileService.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int id) =>
            {
                DayProfileModel dayProfile = _dayProfileDB.Find(d => d.DayProfileID == id);
                return Task.FromResult<DayProfileModel>(dayProfile);
            });


            _scalarService.Setup(r => r.GetNewestScalarForTypeWithObjectIDAsync(It.IsAny<ScalarTypes>(), It.IsAny<int>(), It.IsAny<DateTime>())).Returns((ScalarTypes type, int id, DateTime date) =>
            {
                ScalarModel scalar = _scalarDB.FindAll(s => s.TypeOfScalar == type && s.ScalarObjectID == id && s.DateTimeCreated < date).OrderBy(s => s.DateTimeCreated).First();
                return Task.FromResult<ScalarModel>(scalar);
            });

            _applicationProperies.Setup(r => r.GetProperty<float>(It.IsAny<string>())).Returns(2.0f);

            Assert.False(await AlgorithmBase.UpdateScalarValues(reminder.Logs));
        }

        [Test]
        async public Task UpdateDayProfileScalars_ErrorInUpdateGrocery_ReturnsFalse()
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
            _logService.Setup(r => r.GetAllLogsWithDayProfileIDAsync(It.IsAny<int>())).Returns((int id) =>
            {
                return Task.FromResult(_logDB.FindAll(log => log.DayProfile.DayProfileID == id));
            });
            _logService.Setup(r => r.GetAllLogsAfterDateAsync(It.IsAny<DateTime>())).Returns((DateTime date) =>
            {
                List<LogModel> logsAfterDate = _logDB.FindAll(log => log.DateTimeValue > date);
                return Task.FromResult(logsAfterDate);
            });

            _logService.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(_logDB));

            _dayProfileService.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int id) =>
            {
                DayProfileModel dayProfile = _dayProfileDB.Find(d => d.DayProfileID == id);
                return Task.FromResult<DayProfileModel>(dayProfile);
            });


            _scalarService.Setup(r => r.GetNewestScalarForTypeWithObjectIDAsync(It.IsAny<ScalarTypes>(), It.IsAny<int>(), It.IsAny<DateTime>())).Returns((ScalarTypes type, int id, DateTime date) =>
            {
                ScalarModel scalar = _scalarDB.FindAll(s => s.TypeOfScalar == type && s.ScalarObjectID == id && s.DateTimeCreated < date).OrderBy(s => s.DateTimeCreated).First();
                return Task.FromResult<ScalarModel>(scalar);
            });

            _applicationProperies.Setup(r => r.GetProperty<float>(It.IsAny<string>())).Returns(2.0f);

            Assert.False(await AlgorithmBase.UpdateScalarValues(reminder.Logs));
        }

        [Test]
        async public Task UpdateDayProfileScalars_NoErrors_ReturnsTrue()
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
            _logService.Setup(r => r.GetAllLogsWithDayProfileIDAsync(It.IsAny<int>())).Returns((int id) =>
            {
                return Task.FromResult(_logDB.FindAll(log => log.DayProfile.DayProfileID == id));
            });
            _logService.Setup(r => r.GetAllLogsAfterDateAsync(It.IsAny<DateTime>())).Returns((DateTime date) =>
            {
                List<LogModel> logsAfterDate = _logDB.FindAll(log => log.DateTimeValue > date);
                return Task.FromResult(logsAfterDate);
            });

            _logService.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(_logDB));

            _dayProfileService.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int id) =>
            {
                DayProfileModel dayProfile = _dayProfileDB.Find(d => d.DayProfileID == id);
                return Task.FromResult<DayProfileModel>(dayProfile);
            });


            _scalarService.Setup(r => r.GetNewestScalarForTypeWithObjectIDAsync(It.IsAny<ScalarTypes>(), It.IsAny<int>(), It.IsAny<DateTime>())).Returns((ScalarTypes type, int id, DateTime date) =>
            {
                ScalarModel scalar = _scalarDB.FindAll(s => s.TypeOfScalar == type && s.ScalarObjectID == id && s.DateTimeCreated < date).OrderBy(s => s.DateTimeCreated).First();
                return Task.FromResult<ScalarModel>(scalar);
            });

            _groceryService.Setup(r => r.GetGroceryAsync(It.IsAny<int>())).Returns((int id) =>
            {
                GroceryModel grocery = _groceryDB.Find(g => g.GroceryID == id);
                return Task.FromResult(grocery);
            });

            _applicationProperies.Setup(r => r.GetProperty<float>(It.IsAny<string>())).Returns(2.0f);

            Assert.True(await AlgorithmBase.UpdateScalarValues(reminder.Logs));
        }

        [Test]
        async public Task UpdateDayProfileScalars_EnoughData_ReturnsTrue()
        {
            ReminderModel reminder = _reminderDB[^1];

            _dayProfileService.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int id) =>
            {
                return Task.FromResult(_dayProfileDB.Find(d => d.DayProfileID == id));
            });

            _logService.Setup(r => r.GetAllLogsWithDayProfileIDAsync(It.IsAny<int>())).Returns((int id) =>
            {
                return Task.FromResult(_logDB.FindAll(log => log.DayProfile.DayProfileID == id));
            });

            _scalarService.Setup(r => r.GetNewestScalarForTypeWithObjectIDAsync(It.IsAny<ScalarTypes>(), It.IsAny<int>(), It.IsAny<DateTime>())).Returns((ScalarTypes type, int id, DateTime date) =>
            {
                ScalarModel scalar = _scalarDB.FindAll(s => s.TypeOfScalar == type && s.ScalarObjectID == id && s.DateTimeCreated < date).OrderBy(s => s.DateTimeCreated).First();
                return Task.FromResult<ScalarModel>(scalar);
            });

            _applicationProperies.Setup(r => r.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            Assert.True(await AlgorithmBase.UpdateDayProfileScalars(reminder.Logs[^1].DayProfile.DayProfileID));
        }

        [Test]
        public void GetDataPointForDayProfileCarbScalar_ThrowsArgumentException_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();
            log.DayProfile = null;

            DataPoint dataPoint = AlgorithmBase.GetDataPointForDayProfileCarbScalar(log);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForDayProfileCarbScalar_ThrowsArgumentNullException_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();
            log.GlucoseAfterMeal = null;

            DataPoint dataPoint = AlgorithmBase.GetDataPointForDayProfileCarbScalar(log);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForDayProfileCarbScalar_ThrowsExceptionOnCurruptData_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();
            log.InsulinEstimate = 0;

            DataPoint dataPoint = AlgorithmBase.GetDataPointForDayProfileCarbScalar(log);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForDayProfileCarbScalar_NoErrors_ReturnsValidDataPoint()
        {
            LogModel log = _logDB.First();

            DataPoint dataPoint = AlgorithmBase.GetDataPointForDayProfileCarbScalar(log);

            Assert.NotNull(dataPoint);
            Assert.True(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForDayProfileGlucoseScalar_ThrowsArgumentException_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();
            log.DayProfile = null;

            DataPoint dataPoint = AlgorithmBase.GetDataPointForDayProfileGlucoseScalar(log);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForDayProfileGlucoseScalar_ThrowsArgumentNullException_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();
            log.GlucoseAfterMeal = null;

            DataPoint dataPoint = AlgorithmBase.GetDataPointForDayProfileGlucoseScalar(log);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForDayProfileGlucoseScalar_ThrowsExceptionOnCurruptData_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();
            log.InsulinEstimate = 0;

            DataPoint dataPoint = AlgorithmBase.GetDataPointForDayProfileGlucoseScalar(log);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForDayProfileGlucoseScalar_NoErrors_ReturnsValidDataPoint()
        {
            LogModel log = _logDB.First();

            DataPoint dataPoint = AlgorithmBase.GetDataPointForDayProfileGlucoseScalar(log);

            Assert.NotNull(dataPoint);
            Assert.True(dataPoint.IsValid);
        }

        [Test]
        async public Task UpdateCorrectionInsulinScalar_EnoughData_ReturnsTrue()
        {
            _logService.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(_logDB));

            _scalarService.Setup(r => r.GetNewestScalarForTypeWithObjectIDAsync(It.IsAny<ScalarTypes>(), It.IsAny<int>(), It.IsAny<DateTime>())).Returns((ScalarTypes type, int id, DateTime date) =>
            {
                ScalarModel scalar = _scalarDB.FindAll(s => s.TypeOfScalar == type && s.ScalarObjectID == id && s.DateTimeCreated < date).OrderBy(s => s.DateTimeCreated).First();
                return Task.FromResult<ScalarModel>(scalar);
            });

            _logService.Setup(r => r.GetAllLogsAfterDateAsync(It.IsAny<DateTime>())).Returns((DateTime date) =>
            {
                List<LogModel> logsAfterDate = _logDB.FindAll(log => log.DateTimeValue > date);
                return Task.FromResult(logsAfterDate);
            });

            _applicationProperies.Setup(r => r.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            Assert.True(await AlgorithmBase.UpdateCorrectionInsulinScalar());
        }


        [Test]
        public void GetDataPointForCorrectionInsulin_InvalidLog_ThrowsArgumentException_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();
            log.GlucoseAfterMeal = null;

            DataPoint dataPoint = AlgorithmBase.GetDataPointForCorrectionInsulin(log);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForCorrectionInsulin_ThrowsArgumentExceptionOnCurruptData_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();
            log.InsulinEstimate = 0;

            DataPoint dataPoint = AlgorithmBase.GetDataPointForCorrectionInsulin(log);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForCorrectionInsulin_NoErrors_ReturnsValidDataPoint()
        {
            LogModel log = _logDB.First();

            DataPoint dataPoint = AlgorithmBase.GetDataPointForCorrectionInsulin(log);

            Assert.NotNull(dataPoint);
            Assert.True(dataPoint.IsValid);
        }

        [Test]
        async public Task UpdateGroceryScalar_NoCurrentGrocery_ReturnsFalse()
        {
            GroceryModel grocery = _groceryDB.First();

            Assert.False(await AlgorithmBase.UpdateGroceryScalar(grocery.GroceryID));
        }

        [Test]
        async public Task UpdateGroceryScalar_CorruptData_ReturnsTrue()
        {
            GroceryModel grocery = _groceryDB.First();

            _logDB.ForEach(log => log.GlucoseAfterMeal = null);

            _groceryService.Setup(r => r.GetGroceryAsync(It.IsAny<int>())).Returns((int id) =>
            {
                return Task.FromResult(_groceryDB.Find(g => g.GroceryID == id));
            });

            _logService.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(_logDB));

            _scalarService.Setup(r => r.GetNewestScalarForTypeWithObjectIDAsync(It.IsAny<ScalarTypes>(), It.IsAny<int>(), It.IsAny<DateTime>())).Returns((ScalarTypes type, int id, DateTime date) =>
            {
                ScalarModel scalar = _scalarDB.FindAll(s => s.TypeOfScalar == type && s.ScalarObjectID == id && s.DateTimeCreated < date).OrderBy(s => s.DateTimeCreated).First();
                return Task.FromResult<ScalarModel>(scalar);
            });

            _logService.Setup(r => r.GetAllLogsAfterDateAsync(It.IsAny<DateTime>())).Returns((DateTime date) =>
            {
                List<LogModel> logsAfterDate = _logDB.FindAll(log => log.DateTimeValue > date);
                return Task.FromResult(logsAfterDate);
            });

            Assert.True(await AlgorithmBase.UpdateGroceryScalar(grocery.GroceryID));
        }

        [Test]
        async public Task UpdateGroceryScalar_EnoughData_ReturnsTrue()
        {
            GroceryModel grocery = _groceryDB.First();

            _groceryService.Setup(r => r.GetGroceryAsync(It.IsAny<int>())).Returns((int id) =>
            {
                return Task.FromResult(_groceryDB.Find(g => g.GroceryID == id));
            });

            _logService.Setup(r => r.GetAllLogsAsync()).Returns(Task.FromResult(_logDB));

            _scalarService.Setup(r => r.GetNewestScalarForTypeWithObjectIDAsync(It.IsAny<ScalarTypes>(), It.IsAny<int>(), It.IsAny<DateTime>())).Returns((ScalarTypes type, int id, DateTime date) =>
            {
                ScalarModel scalar = _scalarDB.FindAll(s => s.TypeOfScalar == type && s.ScalarObjectID == id && s.DateTimeCreated < date).OrderBy(s => s.DateTimeCreated).First();
                return Task.FromResult<ScalarModel>(scalar);
            });

            _logService.Setup(r => r.GetAllLogsAfterDateAsync(It.IsAny<DateTime>())).Returns((DateTime date) =>
            {
                List<LogModel> logsAfterDate = _logDB.FindAll(log => log.DateTimeValue > date);
                return Task.FromResult(logsAfterDate);
            });

            _applicationProperies.Setup(r => r.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            Assert.True(await AlgorithmBase.UpdateGroceryScalar(grocery.GroceryID));
        }

        [Test]
        public void GetDataPointForGrocery_ThrowsArgumentException_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();
            log.GlucoseAfterMeal = null;

            DataPoint dataPoint = AlgorithmBase.GetDataPointForGrocery(log, log.NumberOfGroceries.First().Grocery.GroceryID);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForGrocery_ThrowsArgumentOutOfRangeException_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();


            DataPoint dataPoint = AlgorithmBase.GetDataPointForGrocery(log, -1);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForGrocery_ThrowsExceptionOnCurruptData_ReturnsInvalidDataPoint()
        {
            LogModel log = _logDB.First();
            log.InsulinEstimate = 0;

            DataPoint dataPoint = AlgorithmBase.GetDataPointForGrocery(log, log.NumberOfGroceries.First().Grocery.GroceryID);

            Assert.NotNull(dataPoint);
            Assert.False(dataPoint.IsValid);
        }

        [Test]
        public void GetDataPointForGrocery_NoErrors_ReturnsValidDataPoint()
        {
            LogModel log = _logDB.First();

            DataPoint dataPoint = AlgorithmBase.GetDataPointForGrocery(log, log.NumberOfGroceries.First().Grocery.GroceryID);

            Assert.NotNull(dataPoint);
            Assert.True(dataPoint.IsValid);
        }

        [Test]
        public void GetGreatestSafeDistanceFromWantedLine_DistanceGreaterThanLimit_ReturnMaxDistance()
        {
            DateTime today = DateTime.Now;
            List<DataPoint> dataPoints = new()
            {
                new DataPoint(true, today.AddDays(75 - 200), 0.48d),
                new DataPoint(true, today.AddDays(145 - 200), 1.09d),
                new DataPoint(true, today.AddDays(55 - 200), 0.53d),
                new DataPoint(true, today.AddDays(88 - 200), 0.97d),
                new DataPoint(true, today.AddDays(122 - 200), 0.78d)
            };

            Assert.AreEqual(AlgorithmBase.ABSOLUTE_MAXIMUM_DISTANCE_CHANGE, Math.Abs(AlgorithmBase.GetGreatestSafeDistanceFromWantedLine(dataPoints)));
        }
    }
}
