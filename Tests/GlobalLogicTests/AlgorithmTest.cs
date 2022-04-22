using NUnit.Framework;
using Moq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathNet.Numerics;

using DiabetesContolApp.GlobalLogic;
using DiabetesContolApp.Service.Interfaces;
using DiabetesContolApp.Models;
using DiabetesContolApp.Service;

namespace Tests.GlobalLogicTests
{
    [TestFixture]
    public class AlgorithmTest
    {
        private Mock<IReminderService> _reminderService;

        [SetUp]
        public void Setup()
        {
            _reminderService = new();
            Algorithm._reminderService = _reminderService.Object;
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

    }
}
