using NUnit.Framework;
using Moq;

using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Service;
using DiabetesContolApp.Models;
using DiabetesContolApp.Repository.Interfaces;
using System;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class ScalarServiceTest
    {
        private ScalarService _scalarService;

        private Mock<IScalarRepo> _scalarRepo;

        [SetUp]
        public void SetUp()
        {
            _scalarRepo = new();

            _scalarService = new(_scalarRepo.Object);
        }

        [Test]
        async public Task GetNewestScalarForTypeWithObjectIDAsync_ScalarExists_ReturnsScalar()
        {
            const int ID = 1;
            const ScalarTypes TYPE = ScalarTypes.GROCERY;

            List<ScalarModel> scalars = new() { new ScalarModel(1, TYPE, ID, 1.0f, DateTime.Now), new ScalarModel(2, TYPE, 2, 1.0f, DateTime.Now) };

            _scalarRepo.Setup(r => r.GetAllScalarsOfTypeAsync(It.IsAny<ScalarTypes>())).Returns(Task.FromResult(scalars));

            ScalarModel scalar = await _scalarService.GetNewestScalarForTypeWithObjectIDAsync(TYPE, ID, DateTime.Now.AddDays(-1));

            Assert.NotNull(scalar);
            Assert.AreEqual(TYPE, scalar.TypeOfScalar);
            Assert.AreEqual(ID, scalar.ScalarObjectID);
        }

        [Test]
        async public Task GetNewestScalarForTypeWithObjectIDAsync_ScalarDoesNotExists_ReturnsScalar()
        {
            const int SCALAR_ID = 1;
            const int OBJECT_ID = 2;
            const ScalarTypes TYPE = ScalarTypes.GROCERY;

            ScalarModel s = new(SCALAR_ID, TYPE, OBJECT_ID, 1.0f, DateTime.Now);

            _scalarRepo.Setup(r => r.GetAllScalarsOfTypeAsync(It.IsAny<ScalarTypes>())).Returns(Task.FromResult(new List<ScalarModel>()));
            _scalarRepo.Setup(r => r.InsertScalarAsync(It.IsAny<ScalarModel>())).Returns(Task.FromResult(true));
            _scalarRepo.Setup(r => r.GetAllScalarsAsync()).Returns(Task.FromResult(new List<ScalarModel>() { s })); ;
            _scalarRepo.Setup(r => r.GetScalarAsync(SCALAR_ID)).Returns(Task.FromResult(s));

            ScalarModel scalar = await _scalarService.GetNewestScalarForTypeWithObjectIDAsync(TYPE, OBJECT_ID, DateTime.Now.AddDays(-1));

            Assert.NotNull(scalar);
            Assert.AreEqual(TYPE, scalar.TypeOfScalar);
            Assert.AreEqual(OBJECT_ID, scalar.ScalarObjectID);
            Assert.AreEqual(SCALAR_ID, scalar.ScalarID);
        }

        [Test]
        async public Task InsertScalarAsync_WithInsertTrueAndGetNotNull_ReturnsPositiveInt()
        {
            const int SCALAR_ID = 1;
            const int OBJECT_ID = 2;
            const ScalarTypes TYPE = ScalarTypes.GROCERY;

            ScalarModel scalar = new(SCALAR_ID, TYPE, OBJECT_ID, 1.0f, DateTime.Now);

            _scalarRepo.Setup(r => r.InsertScalarAsync(It.IsAny<ScalarModel>())).Returns(Task.FromResult(true));
            _scalarRepo.Setup(r => r.GetAllScalarsAsync()).Returns(Task.FromResult(new List<ScalarModel>() { scalar })); ;
            _scalarRepo.Setup(r => r.GetScalarAsync(SCALAR_ID)).Returns(Task.FromResult(scalar));

            int scalarID = await _scalarService.InsertScalarAsync(scalar);

            Assert.Greater(scalarID, 0);
            Assert.AreEqual(SCALAR_ID, scalarID);
        }

        [Test]
        async public Task InsertScalarAsync_WithInsertFalseAndGetNotNull_ReturnsMinusOne()
        {
            const int SCALAR_ID = 1;
            const int OBJECT_ID = 2;
            const ScalarTypes TYPE = ScalarTypes.GROCERY;

            ScalarModel scalar = new(SCALAR_ID, TYPE, OBJECT_ID, 1.0f, DateTime.Now);

            _scalarRepo.Setup(r => r.InsertScalarAsync(It.IsAny<ScalarModel>())).Returns(Task.FromResult(false));
            _scalarRepo.Setup(r => r.GetAllScalarsAsync()).Returns(Task.FromResult(new List<ScalarModel>() { scalar })); ;
            _scalarRepo.Setup(r => r.GetScalarAsync(SCALAR_ID)).Returns(Task.FromResult(scalar));

            int scalarID = await _scalarService.InsertScalarAsync(scalar);

            Assert.Less(scalarID, 0);
            Assert.AreEqual(-1, scalarID);
        }

        [Test]
        async public Task InsertScalarAsync_WithInsertTrueAndGetNull_ReturnsMinusOne()
        {
            const int SCALAR_ID = 1;
            const int OBJECT_ID = 2;
            const ScalarTypes TYPE = ScalarTypes.GROCERY;

            ScalarModel scalar = new(SCALAR_ID, TYPE, OBJECT_ID, 1.0f, DateTime.Now);

            _scalarRepo.Setup(r => r.InsertScalarAsync(It.IsAny<ScalarModel>())).Returns(Task.FromResult(true));
            _scalarRepo.Setup(r => r.GetAllScalarsAsync()).Returns(Task.FromResult(new List<ScalarModel>()));

            int scalarID = await _scalarService.InsertScalarAsync(scalar);

            Assert.Less(scalarID, 0);
            Assert.AreEqual(-1, scalarID);
        }

        [Test]
        async public Task UpdateScalarAsync_WithScalarThatExists_ReturnsTrue()
        {
            const int SCALAR_ID = 1;
            const int OBJECT_ID = 2;
            const ScalarTypes TYPE = ScalarTypes.GROCERY;

            ScalarModel scalar = new(SCALAR_ID, TYPE, OBJECT_ID, 1.0f, DateTime.Now);

            _scalarRepo.Setup(r => r.UpdateScalarAsync(It.IsAny<ScalarModel>())).Returns(Task.FromResult(true));

            Assert.True(await _scalarService.UpdateScalarAsync(scalar));
        }

        [Test]
        async public Task UpdateScalarAsync_WithScalarThatDoesNotExists_ReturnsFalse()
        {
            const int SCALAR_ID = -1;
            const int OBJECT_ID = 2;
            const ScalarTypes TYPE = ScalarTypes.GROCERY;

            ScalarModel scalar = new(SCALAR_ID, TYPE, OBJECT_ID, 1.0f, DateTime.Now);

            _scalarRepo.Setup(r => r.UpdateScalarAsync(It.IsAny<ScalarModel>())).Returns(Task.FromResult(false));

            Assert.False(await _scalarService.UpdateScalarAsync(scalar));
        }
    }
}
