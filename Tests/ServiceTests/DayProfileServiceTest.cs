using NUnit.Framework;
using Moq;

using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Service;
using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;
using DiabetesContolApp.Repository.Interfaces;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class DayProfileServiceTest
    {
        private DayProfileService _dayProfileService;

        private Mock<IDayProfileRepo> _dayProfileRepo;
        private Mock<ILogRepo> _logRepo;
        private Mock<IGroceryLogRepo> _groceryLogRepo;
        private Mock<IReminderRepo> _reminderRepo;

        [SetUp]
        public void Setup()
        {
            _dayProfileRepo = new();
            _logRepo = new();
            _groceryLogRepo = new();
            _reminderRepo = new();

            _dayProfileService = new(_dayProfileRepo.Object, _logRepo.Object, _groceryLogRepo.Object, _reminderRepo.Object);
        }

        [Test]
        async public Task GetAllDayProfilesAsync_NoDayProfiles_ReturnsEmptyList()
        {
            _dayProfileRepo.Setup(r => r.GetAllDayProfilesAsync()).Returns(Task.FromResult(new List<DayProfileModel>()));

            List<DayProfileModel> dayProfiles = await _dayProfileService.GetAllDayProfilesAsync();

            Assert.NotNull(dayProfiles);
            Assert.AreEqual(0, dayProfiles.Count);
        }

        [Test]
        async public Task GetAllDayProfilesAsync_WithDayProfiles_ReturnsNonEmptyList()
        {
            _dayProfileRepo.Setup(r => r.GetAllDayProfilesAsync()).Returns(Task.FromResult(new List<DayProfileModel>() { new DayProfileModel(1), new DayProfileModel(2), new DayProfileModel(3), }));
            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            List<DayProfileModel> dayProfiles = await _dayProfileService.GetAllDayProfilesAsync();

            Assert.NotNull(dayProfiles);
            Assert.AreEqual(3, dayProfiles.Count);
            Assert.AreEqual(1, dayProfiles[0].DayProfileID);
            Assert.AreEqual(2, dayProfiles[1].DayProfileID);
            Assert.AreEqual(3, dayProfiles[2].DayProfileID);
        }

        [Test]
        async public Task InsertDayProfileAsync_WithValidDayProfile_ReturnsPositiveInt()
        {
            const int NEW_ID = 1;
            _dayProfileRepo.Setup(r => r.InsertDayProfileAsync(It.IsAny<DayProfileModel>())).Returns(Task.FromResult(true));
            _dayProfileRepo.Setup(r => r.GetAllDayProfilesAsync()).Returns(Task.FromResult(new List<DayProfileModel>() { new DayProfileModel(NEW_ID) }));
            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            int id = await _dayProfileService.InsertDayProfileAsync(new DayProfileModel());

            Assert.GreaterOrEqual(id, 0);
        }

        [Test]
        async public Task InsertDayProfileAsync_WithErrorInInsertRepo_ReturnsMinusOne()
        {
            _dayProfileRepo.Setup(r => r.InsertDayProfileAsync(It.IsAny<DayProfileModel>())).Returns(Task.FromResult(false));

            int id = await _dayProfileService.InsertDayProfileAsync(new DayProfileModel());

            Assert.AreEqual(-1, id);
        }

        [Test]
        async public Task InsertDayProfileAsync_WithNoNewestDayProfile_ReturnsMinusOne()
        {
            _dayProfileRepo.Setup(r => r.InsertDayProfileAsync(It.IsAny<DayProfileModel>())).Returns(Task.FromResult(true));
            _dayProfileRepo.Setup(r => r.GetAllDayProfilesAsync()).Returns(Task.FromResult(new List<DayProfileModel>()));

            int id = await _dayProfileService.InsertDayProfileAsync(new DayProfileModel());

            Assert.AreEqual(-1, id);
        }

        [Test]
        async public Task GetNewestDayProfileAsync_WithDayProfilesExisits_ReturnsDayProfile()
        {
            _dayProfileRepo.Setup(r => r.GetAllDayProfilesAsync()).Returns(Task.FromResult(new List<DayProfileModel>() { new DayProfileModel() }));
            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            DayProfileModel dayProfile = await _dayProfileService.GetNewestDayProfileAsync();

            Assert.NotNull(dayProfile);
        }

        [Test]
        async public Task GetNewestDayProfileAsync_WithNoDayProfiles_ReturnsDayProfile()
        {
            _dayProfileRepo.Setup(r => r.GetAllDayProfilesAsync()).Returns(Task.FromResult(new List<DayProfileModel>()));

            DayProfileModel dayProfile = await _dayProfileService.GetNewestDayProfileAsync();

            Assert.Null(dayProfile);
        }

        [Test]
        async public Task UpdateDayProfileAsync_WithValidDayProfile_ReturnsTrue()
        {
            _dayProfileRepo.Setup(r => r.UpdateDayProfileAsync(It.IsAny<DayProfileModel>())).Returns(Task.FromResult(true));

            Assert.True(await _dayProfileService.UpdateDayProfileAsync(new DayProfileModel()));
        }

        [Test]
        async public Task UpdateDayProfileAsync_WithInvalidDayProfile_ReturnsFalse()
        {
            _dayProfileRepo.Setup(r => r.UpdateDayProfileAsync(It.IsAny<DayProfileModel>())).Returns(Task.FromResult(false));

            Assert.False(await _dayProfileService.UpdateDayProfileAsync(new DayProfileModel()));
        }

        [Test]
        async public Task DeleteDayProfileAsync_WithValidDayProfile_ReturnsTrue()
        {
            _dayProfileRepo.Setup(r => r.DeleteDayProfileAsync(It.IsAny<int>())).Returns(Task.FromResult(true));

            Assert.True(await _dayProfileService.DeleteDayProfileAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task DeleteDayProfileAsync_WithInvalidDayProfile_ReturnsFalse()
        {
            _dayProfileRepo.Setup(r => r.DeleteDayProfileAsync(It.IsAny<int>())).Returns(Task.FromResult(false));

            Assert.False(await _dayProfileService.DeleteDayProfileAsync(It.IsAny<int>()));
        }

        [Test]
        async public Task GetDayProfileAsync_WithValidID_ReturnsDayProfileWithID()
        {
            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) => Task.FromResult(new DayProfileModel(x)));

            const int VALID_ID = 1;

            DayProfileModel dayProfile = await _dayProfileService.GetDayProfileAsync(VALID_ID);

            Assert.NotNull(dayProfile);
            Assert.AreEqual(VALID_ID, dayProfile.DayProfileID);
        }

        [Test]
        async public Task GetDayProfileAsync_WithInvalidID_ReturnsNullRef()
        {
            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns(Task.FromResult<DayProfileModel>(null));

            const int VALID_ID = 1;

            DayProfileModel dayProfile = await _dayProfileService.GetDayProfileAsync(VALID_ID);

            Assert.Null(dayProfile);
        }

        [Test]
        async public Task GetDayProfileAsync_WithInvalidTargetGlucose_ReturnsNullRef()
        {
            _dayProfileRepo.Setup(r => r.GetDayProfileAsync(It.IsAny<int>())).Returns((int x) =>
            {
                DayProfileModel d = new(x);
                d.TargetGlucoseValue = -1.0f;
                return Task.FromResult(d);
            });

            const int VALID_ID = 1;

            DayProfileModel dayProfile = await _dayProfileService.GetDayProfileAsync(VALID_ID);

            Assert.Null(dayProfile);
        }
    }
}
