using NUnit.Framework;
using Moq;

using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Service;
using DiabetesContolApp.Models;
using DiabetesContolApp.Repository.Interfaces;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class ReminderServiceTest
    {
        private ReminderService _reminderService;

        private Mock<IReminderRepo> _reminderRepo;
        private Mock<ILogRepo> _logRepo;

        [SetUp]
        public void Setup()
        {
            _reminderRepo = new();
            _logRepo = new();


            _reminderService = new(_reminderRepo.Object, _logRepo.Object);
        }

        [Test]
        async public Task InsertReminderAsync_WithSuccessfulInsert_ReturnsPositiveInt()
        {
            const int ID = 1;
            _reminderRepo.Setup(r => r.InsertReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(true));
            _reminderRepo.Setup(r => r.GetAllRemindersAsync()).Returns(Task.FromResult(new List<ReminderModel>() { new ReminderModel(ID) }));
            _reminderRepo.Setup(r => r.GetReminderAsync(ID)).Returns(Task.FromResult(new ReminderModel(ID)));
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns(Task.FromResult(new List<LogModel>()));

            int reminderID = await _reminderService.InsertReminderAsync(new ReminderModel());

            Assert.Greater(reminderID, 0);
            Assert.AreEqual(ID, reminderID);
        }

        [Test]
        async public Task InsertReminderAsync_WithUnsuccessfulInsert_ReturnsMinusOne()
        {
            _reminderRepo.Setup(r => r.InsertReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(false));

            int reminderID = await _reminderService.InsertReminderAsync(new ReminderModel());

            Assert.Less(reminderID, 0);
            Assert.AreEqual(-1, reminderID);
        }

        [Test]
        async public Task GetNewestReminderAsync_WithNotRemindersFound_ReturnsNullRef()
        {
            _reminderRepo.Setup(r => r.GetAllRemindersAsync()).Returns(Task.FromResult(new List<ReminderModel>()));

            ReminderModel reminder = await _reminderService.GetNewestReminderAsync();

            Assert.Null(reminder);
        }

        [Test]
        async public Task GetNewestReminderAsync_WithRemindersFound_ReturnsNullRef()
        {
            const int ID = 1;
            _reminderRepo.Setup(r => r.GetAllRemindersAsync()).Returns(Task.FromResult(new List<ReminderModel>() { new ReminderModel(ID) }));
            _reminderRepo.Setup(r => r.GetReminderAsync(ID)).Returns(Task.FromResult(new ReminderModel(ID)));

            ReminderModel reminder = await _reminderService.GetNewestReminderAsync();

            Assert.NotNull(reminder);
            Assert.AreEqual(ID, reminder.ReminderID);
        }

        [Test]
        async public Task GetReminderAsync_NotReminderWithID_ReturnsNullRef()
        {
            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns(Task.FromResult<ReminderModel>(null));

            ReminderModel reminder = await _reminderService.GetReminderAsync(It.IsAny<int>());

            Assert.Null(reminder);
        }

        [Test]
        async public Task HandleRemindersAsync_NoUnhandledRemidners_ReturnsVoid()
        {
            _reminderRepo.Setup(r => r.GetAllUnhandledRemindersAsync()).Returns(Task.FromResult(new List<ReminderModel>()));

            await _reminderService.HandleRemindersAsync();
        }

        [Test]
        async public Task HandleRemindersAsync_WithUnhandledRemidnersNotHandled_RemindersUnhandled()
        {
            List<ReminderModel> reminders = new() { new ReminderModel(), new ReminderModel() };
            _reminderRepo.Setup(r => r.GetAllUnhandledRemindersAsync()).Returns(Task.FromResult(reminders));
            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new ReminderModel(x)));

            await _reminderService.HandleRemindersAsync();

            Assert.AreEqual(reminders[0].IsHandled, false);
            Assert.AreEqual(reminders[1].IsHandled, false);
        }

        [Test]
        async public Task HandleRemindersAsync_WithUnhandledRemidnersHandled_RemindersHandled()
        {
            List<ReminderModel> reminders = new() { new ReminderModel(1), new ReminderModel(2) };
            _reminderRepo.Setup(r => r.GetAllUnhandledRemindersAsync()).Returns(Task.FromResult(reminders));
            _reminderRepo.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns((int x) =>
            {
                ReminderModel r = new(x);
                r.IsHandled = true;
                return Task.FromResult(r);
            });
            _logRepo.Setup(r => r.GetAllLogsWithReminderIDAsync(It.IsAny<int>())).Returns(Task.FromResult(new List<LogModel>()));
            _reminderRepo.Setup(r => r.UpdateReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(true));

            await _reminderService.HandleRemindersAsync();

            Assert.AreEqual(reminders[0].IsHandled, true);
            Assert.AreEqual(reminders[1].IsHandled, true);
        }

        [Test]
        async public Task UpdateReminderAsync_WithValidID_ReturnsTrue()
        {
            const int ID = 1;

            _reminderRepo.Setup(r => r.UpdateReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(true));

            ReminderModel reminder = new(ID);
            reminder.Logs.Add(new LogModel());

            bool result = await _reminderService.UpdateReminderAsync(reminder);

            Assert.True(result);
        }

        [Test]
        async public Task UpdateReminderAsync_WithInvalidID_ReturnsFalse()
        {
            const int ID = -1;

            _reminderRepo.Setup(r => r.UpdateReminderAsync(It.IsAny<ReminderModel>())).Returns(Task.FromResult(false));

            ReminderModel reminder = new(ID);
            reminder.Logs.Add(new LogModel());

            bool result = await _reminderService.UpdateReminderAsync(reminder);

            Assert.False(result);
        }

        [Test]
        async public Task DeleteReminderAsync_WithValidAndInvalidID_ReturnsTrueAndFalse()
        {
            _reminderRepo.Setup(r => r.DeleteReminderAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(x > 0));

            Assert.True(await _reminderService.DeleteReminderAsync(1));
            Assert.False(await _reminderService.DeleteReminderAsync(-1));
        }
    }
}