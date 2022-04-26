using NUnit.Framework;
using Moq;

using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Repository;
using DiabetesContolApp.Persistence.Interfaces;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Models;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class DayProfileRepoTest
    {
        private DayProfileRepo _dayProfileRepo;

        private Mock<IDayProfileDatabase> _dayProfileDatabase;


        [SetUp]
        public void Setup()
        {
            _dayProfileDatabase = new();

            _dayProfileRepo = new(_dayProfileDatabase.Object);
        }

        [Test]
        async public Task GetAllDayProfilesAsync_WithDayProfiles_ReturnsListWithDayProfiles()
        {
            const int LIST_LENGTH = 3;
            List<DayProfileModelDAO> dayProfileDAOs = new();
            for (int i = 0; i < LIST_LENGTH; ++i)
                dayProfileDAOs.Add(new(new DayProfileModel(i)));

            Assert.AreEqual(LIST_LENGTH, dayProfileDAOs.Count);

            _dayProfileDatabase.Setup(r => r.GetAllDayProfilesAsync()).Returns(Task.FromResult(dayProfileDAOs));

            List<DayProfileModel> dayProfiles = await _dayProfileRepo.GetAllDayProfilesAsync();

            Assert.NotNull(dayProfiles);
            Assert.AreEqual(LIST_LENGTH, dayProfiles.Count);
        }

        [Test]
        async public Task GetAllDayProfilesAsync_WithNoDayProfiles_ReturnsEmptyList()
        {
            _dayProfileDatabase.Setup(r => r.GetAllDayProfilesAsync()).Returns(Task.FromResult(new List<DayProfileModelDAO>()));

            List<DayProfileModel> dayProfiles = await _dayProfileRepo.GetAllDayProfilesAsync();

            Assert.NotNull(dayProfiles);
            Assert.AreEqual(0, dayProfiles.Count);
        }

        [Test]
        async public Task UpdateDayProfileAsync_WithSuccess_ReturnsTrue()
        {
            _dayProfileDatabase.Setup(r => r.UpdateDayProfileAsync(It.IsAny<DayProfileModelDAO>())).Returns(Task.FromResult(1));

            Assert.True(await _dayProfileRepo.UpdateDayProfileAsync(new()));
        }

        [Test]
        async public Task UpdateDayProfileAsync_WithFailure_ReturnsFalse()
        {
            _dayProfileDatabase.Setup(r => r.UpdateDayProfileAsync(It.IsAny<DayProfileModelDAO>())).Returns(Task.FromResult(-1));

            Assert.False(await _dayProfileRepo.UpdateDayProfileAsync(new()));
        }

        [Test]
        async public Task DeleteDayProfileAsync_WithSuccess_ReturnsTrue()
        {
            _dayProfileDatabase.Setup(r => r.DeleteDayProfileAsync(It.IsAny<int>())).Returns(Task.FromResult(1));

            Assert.True(await _dayProfileRepo.DeleteDayProfileAsync(new()));
        }

        [Test]
        async public Task DeleteDayProfileAsync_WithFailure_ReturnsFalse()
        {
            _dayProfileDatabase.Setup(r => r.DeleteDayProfileAsync(It.IsAny<int>())).Returns(Task.FromResult(-1));

            Assert.False(await _dayProfileRepo.DeleteDayProfileAsync(new()));
        }

        [Test]
        async public Task InsertDayProfileAsync_WithSuccess_ReturnsTrue()
        {
            _dayProfileDatabase.Setup(r => r.InsertDayProfileAsync(It.IsAny<DayProfileModelDAO>())).Returns(Task.FromResult(1));

            Assert.True(await _dayProfileRepo.InsertDayProfileAsync(new()));
        }

        [Test]
        async public Task InsertDayProfileAsync_WithFailure_ReturnsFalse()
        {
            _dayProfileDatabase.Setup(r => r.InsertDayProfileAsync(It.IsAny<DayProfileModelDAO>())).Returns(Task.FromResult(-1));

            Assert.False(await _dayProfileRepo.InsertDayProfileAsync(new()));
        }

        [Test]
        async public Task GetDayProfileAsync_WithSuccess_ReturnsTrue()
        {
            _dayProfileDatabase.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns(Task.FromResult(new DayProfileModelDAO()));

            DayProfileModel dayProfile = await _dayProfileRepo.GetDayProfileAsync(It.IsAny<int>());

            Assert.NotNull(dayProfile);
        }

        [Test]
        async public Task GetDayProfileAsync_WithFailure_ReturnsFalse()
        {
            _dayProfileDatabase.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns(Task.FromResult<DayProfileModelDAO>(null));

            DayProfileModel dayProfile = await _dayProfileRepo.GetDayProfileAsync(It.IsAny<int>());

            Assert.Null(dayProfile);
        }
    }
}
