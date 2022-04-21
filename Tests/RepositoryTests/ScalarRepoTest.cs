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
    public class ScalarRepoTest
    {
        private ScalarRepo _scalarRepo;

        private Mock<IScalarDatabase> _scalarDatabase;

        [SetUp]
        public void Setup()
        {
            _scalarDatabase = new();

            _scalarRepo = new(_scalarDatabase.Object);
        }

        [Test]
        async public Task GetAllScalarsOfTypeWithObjectID_WithNoResults_ReturnsEmptyList()
        {
            _scalarDatabase.Setup(r => r.GetAllScalarsOfTypeWithObjectIDAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(new List<ScalarModelDAO>()));

            List<ScalarModel> scalars = await _scalarRepo.GetAllScalarsOfTypeWithObjectID(It.IsAny<ScalarTypes>(), It.IsAny<int>());

            Assert.NotNull(scalars);
            Assert.AreEqual(0, scalars.Count);
        }

        [Test]
        async public Task GetAllScalarsOfTypeWithObjectID_WithResults_ReturnsNonEmptyList()
        {
            const int LIST_LENGTH = 3;
            List<ScalarModelDAO> scalarDAOs = new();
            for (int i = 0; i < LIST_LENGTH; i++)
                scalarDAOs.Add(new());

            _scalarDatabase.Setup(r => r.GetAllScalarsOfTypeWithObjectIDAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(scalarDAOs));

            List<ScalarModel> scalars = await _scalarRepo.GetAllScalarsOfTypeWithObjectID(It.IsAny<ScalarTypes>(), It.IsAny<int>());

            Assert.NotNull(scalars);
            Assert.AreEqual(LIST_LENGTH, scalars.Count);
        }

        [Test]
        async public Task UpdateScalarAsync_WithValidScalar_ReturnsTrue()
        {
            _scalarDatabase.Setup(r => r.UpdateScalarAsync(It.IsAny<ScalarModelDAO>())).Returns(Task.FromResult(1));

            Assert.True(await _scalarRepo.UpdateScalarAsync(new ScalarModel(1, ScalarTypes.GROCERY, 1, 1.0f, DateTime.Now)));
        }

        [Test]
        async public Task UpdateScalarAsync_WithInvalidScalar_ReturnsFalse()
        {
            _scalarDatabase.Setup(r => r.UpdateScalarAsync(It.IsAny<ScalarModelDAO>())).Returns(Task.FromResult(-1));

            Assert.False(await _scalarRepo.UpdateScalarAsync(new ScalarModel(1, ScalarTypes.GROCERY, 1, 1.0f, DateTime.Now)));
        }

        [Test]
        async public Task GetAllScalarsOfTypeAsync_WithResults_ReturnsNonEmptyList()
        {
            const int LIST_LENGTH = 3;
            List<ScalarModelDAO> scalarDAOs = new();
            for (int i = 0; i < LIST_LENGTH; i++)
                scalarDAOs.Add(new());

            _scalarDatabase.Setup(r => r.GetAllScalarsOfTypeAsync(It.IsAny<int>())).Returns(Task.FromResult(scalarDAOs));

            List<ScalarModel> scalars = await _scalarRepo.GetAllScalarsOfTypeAsync(It.IsAny<ScalarTypes>());

            Assert.NotNull(scalars);
            Assert.AreEqual(LIST_LENGTH, scalars.Count);
        }

        [Test]
        async public Task InsertScalarAsync_WithValidScalar_ReturnsTrue()
        {
            _scalarDatabase.Setup(r => r.InsertScalarAsync(It.IsAny<ScalarModelDAO>())).Returns(Task.FromResult(1));

            Assert.True(await _scalarRepo.InsertScalarAsync(new(1, ScalarTypes.GROCERY, 1, 1.0f, DateTime.Now)));
        }

        [Test]
        async public Task InsertScalarAsync_WithInvalidScalar_ReturnsFalse()
        {
            _scalarDatabase.Setup(r => r.InsertScalarAsync(It.IsAny<ScalarModelDAO>())).Returns(Task.FromResult(-1));

            Assert.False(await _scalarRepo.InsertScalarAsync(new(1, ScalarTypes.GROCERY, 1, 1.0f, DateTime.Now)));
        }

        [Test]
        async public Task GetScalarAsync_WithValidID_ReturnsScalar()
        {
            const int SCALAR_ID = 1;

            ScalarModelDAO scalarDAO = new(SCALAR_ID, 1, 1, 1.0f, DateTime.Now);

            _scalarDatabase.Setup(r => r.GetScalarAsync(SCALAR_ID)).Returns(Task.FromResult(scalarDAO));

            ScalarModel scalar = await _scalarRepo.GetScalarAsync(SCALAR_ID);

            Assert.NotNull(scalar);
            Assert.AreEqual(SCALAR_ID, scalar.ScalarID);
        }

        [Test]
        async public Task GetScalarAsync_WithInvalidID_ReturnsNull()
        {
            _scalarDatabase.Setup(r => r.GetScalarAsync(It.IsAny<int>())).Returns(Task.FromResult<ScalarModelDAO>(null));

            ScalarModel scalar = await _scalarRepo.GetScalarAsync(It.IsAny<int>());

            Assert.Null(scalar);
        }

        [Test]
        async public Task GetAllScalarsAsync_WithNoResults_ReturnsEmptyList()
        {
            const int LIST_LENGTH = 3;
            List<ScalarModelDAO> scalarDAOs = new();
            for (int i = 0; i < LIST_LENGTH; i++)
                scalarDAOs.Add(new());

            _scalarDatabase.Setup(r => r.GetAllScalarsAsync()).Returns(Task.FromResult(scalarDAOs));

            List<ScalarModel> scalars = await _scalarRepo.GetAllScalarsAsync();

            Assert.NotNull(scalars);
            Assert.AreEqual(LIST_LENGTH, scalars.Count);
        }
    }
}
