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
    }
}
