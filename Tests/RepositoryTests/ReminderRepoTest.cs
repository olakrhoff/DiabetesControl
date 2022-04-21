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
    public class ReminderRepoTest
    {
        private ReminderRepo _reminderRepo;

        private Mock<IReminderDatabase> _reminderDatabase;

        [SetUp]
        public void Setup()
        {
            _reminderDatabase = new();

            _reminderRepo = new(_reminderDatabase.Object);
        }

        [Test]
        async public Task GetReminderAsync_WithValidID_ReturnsReminder()
        {
            const int REMINDER_ID = 1;

            _reminderDatabase.Setup(r => r.GetReminderAsync(REMINDER_ID)).Returns(Task.FromResult(new ReminderModelDAO(new(REMINDER_ID))));

            ReminderModel reminder = await _reminderRepo.GetReminderAsync(REMINDER_ID);

            Assert.NotNull(reminder);
            Assert.AreEqual(REMINDER_ID, reminder.ReminderID);
        }

        [Test]
        async public Task GetReminderAsync_WithInvalidID_ReturnsNullRef()
        {
            _reminderDatabase.Setup(r => r.GetReminderAsync(It.IsAny<int>())).Returns(Task.FromResult<ReminderModelDAO>(null));

            ReminderModel reminder = await _reminderRepo.GetReminderAsync(It.IsAny<int>());

            Assert.Null(reminder);
        }

        [Test]
        async public Task UpdateReminderAsync_WithValidReminder_ReturnsTrue()
        {
            _reminderDatabase.Setup(r => r.UpdateReminderAsync(It.IsAny<ReminderModelDAO>())).Returns(Task.FromResult(1));

            Assert.True(await _reminderRepo.UpdateReminderAsync(new ReminderModel()));
        }

        [Test]
        async public Task UpdateReminderAsync_WithInvalidReminder_ReturnsFalse()
        {
            _reminderDatabase.Setup(r => r.UpdateReminderAsync(It.IsAny<ReminderModelDAO>())).Returns(Task.FromResult(-1));

            Assert.False(await _reminderRepo.UpdateReminderAsync(new ReminderModel()));
        }

        [Test]
        async public Task DeleteReminderAsync_WithValidReminderID_ReturnsTrue()
        {
            _reminderDatabase.Setup(r => r.DeleteReminderAsync(It.IsAny<int>())).Returns(Task.FromResult(1));

            Assert.True(await _reminderRepo.DeleteReminderAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task DeleteReminderAsync_WithInvalidReminderID_ReturnsFalse()
        {
            _reminderDatabase.Setup(r => r.DeleteReminderAsync(It.IsAny<int>())).Returns(Task.FromResult(-1));

            Assert.False(await _reminderRepo.DeleteReminderAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task GetAllRemindersAsync_WithResults_ReturnsNonEmptyList()
        {
            const int LIST_LENGTH = 3;

            List<ReminderModelDAO> reminderDAOs = new();
            for (int i = 0; i < LIST_LENGTH; ++i)
            {
                ReminderModelDAO reminderDAO = new(new ReminderModel(i + 1));
                reminderDAOs.Add(reminderDAO);
            }

            _reminderDatabase.Setup(r => r.GetAllRemindersAsync()).Returns(Task.FromResult(reminderDAOs));

            List<ReminderModel> reminders = await _reminderRepo.GetAllRemindersAsync();

            Assert.NotNull(reminders);
            Assert.AreEqual(LIST_LENGTH, reminders.Count);
        }

        [Test]
        async public Task GetAllUnhandledRemindersAsync_WithResults_ReturnsNonEmptyList()
        {
            const int LIST_LENGTH = 3;

            List<ReminderModelDAO> reminderDAOs = new();
            for (int i = 0; i < LIST_LENGTH; ++i)
            {
                ReminderModelDAO reminderDAO = new(new ReminderModel(i + 1));
                reminderDAOs.Add(reminderDAO);
            }

            _reminderDatabase.Setup(r => r.GetAllUnhandledRemindersAsync()).Returns(Task.FromResult(reminderDAOs));

            List<ReminderModel> reminders = await _reminderRepo.GetAllUnhandledRemindersAsync();

            Assert.NotNull(reminders);
            Assert.AreEqual(LIST_LENGTH, reminders.Count);
        }

        [Test]
        async public Task InsertReminderAsync_WithValidReminder_ReturnsTrue()
        {
            _reminderDatabase.Setup(r => r.InsertReminderAsync(It.IsAny<ReminderModelDAO>())).Returns(Task.FromResult(1));

            Assert.True(await _reminderRepo.InsertReminderAsync(new ReminderModel()));
        }

        [Test]
        async public Task InsertReminderAsync_WithInvalidReminder_ReturnsFalse()
        {
            _reminderDatabase.Setup(r => r.InsertReminderAsync(It.IsAny<ReminderModelDAO>())).Returns(Task.FromResult(-1));

            Assert.False(await _reminderRepo.InsertReminderAsync(new ReminderModel()));
        }
    }
}
