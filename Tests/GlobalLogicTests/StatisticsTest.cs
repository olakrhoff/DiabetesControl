using NUnit.Framework;

using System;

using DiabetesContolApp.GlobalLogic;
using System.Collections.Generic;
using MathNet.Numerics;

namespace Tests.GlobalLogicTests
{
    [TestFixture]
    public class StatisticsTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GetCriticalTValue_DifferentValidValues_ReturnsCorrect()
        {
            //Check 85% interval with 1 degrees of freedom
            Assert.AreEqual(Math.Round(1.963d, 3), Math.Round(Statistics.GetCriticalTValue(0.85, 1), 3));
            //Check 90% interval with 2 degrees of freedom
            Assert.AreEqual(Math.Round(1.886d, 3), Math.Round(Statistics.GetCriticalTValue(0.90, 2), 3));
            //Check 92.5% interval with 3 degrees of freedom
            Assert.AreEqual(Math.Round(1.924d, 3), Math.Round(Statistics.GetCriticalTValue(0.925, 3), 3));
            //Check 95% interval with 4 degrees of freedom
            Assert.AreEqual(Math.Round(2.132d, 3), Math.Round(Statistics.GetCriticalTValue(0.95, 4), 3));
            //Check 97.5% interval with 5 degrees of freedom
            Assert.AreEqual(Math.Round(2.571d, 3), Math.Round(Statistics.GetCriticalTValue(0.975, 5), 3));
            //Check 99% interval with 6 degrees of freedom
            Assert.AreEqual(Math.Round(3.143d, 3), Math.Round(Statistics.GetCriticalTValue(0.99, 6), 3));
            //Check 99.5% interval with 7 degrees of freedom
            Assert.AreEqual(Math.Round(3.499d, 3), Math.Round(Statistics.GetCriticalTValue(0.995, 7), 3));
            //Check 99.9% interval with 8 degrees of freedom
            Assert.AreEqual(Math.Round(4.501d, 3), Math.Round(Statistics.GetCriticalTValue(0.999, 8), 3));
            //Check 99.95% interval with 9 degrees of freedom
            Assert.AreEqual(Math.Round(4.781d, 3), Math.Round(Statistics.GetCriticalTValue(0.9995, 9), 3));
        }

        [Test]
        public void PredictionInterval_ValidData_ReturnsCorrect()
        {
            List<double> x = new() { 75d, 145d, 55d, 88d, 122d }, y = new() { 0.48d, 1.09d, 0.53d, 0.97d, 0.78d };
            double knownX = 90d;
            Tuple<double, double> alphaAndBetaHat = Fit.Line(x.ToArray(), y.ToArray());
            //95% prediction interval
            Tuple<double, double> predictionIntervallNextPoint = Statistics.PredictionInterval(x, y, alphaAndBetaHat.Item1, alphaAndBetaHat.Item2, 0.95, knownX);
            double lower = predictionIntervallNextPoint.Item2;
            double upper = predictionIntervallNextPoint.Item1;
            lower.FirstNSignificantDigits(2);
            upper.FirstNSignificantDigits(2);
            Assert.AreEqual(Math.Round(0.062d, 3), Math.Round(lower, 3));
            Assert.AreEqual(Math.Round(1.40d, 3), Math.Round(upper, 3));
        }

        [Test]
        public void PredictionInterval_WithNotEnoughData_ThrowsError()
        {
            List<double> x = new() { 75d, 145d }, y = new() { 0.48d, 1.09d };
            double knownX = 90d;
            Tuple<double, double> alphaAndBetaHat = Fit.Line(x.ToArray(), y.ToArray());
            //95% prediction interval
            try
            {
                Tuple<double, double> predictionIntervallNextPoint = Statistics.PredictionInterval(x, y, alphaAndBetaHat.Item1, alphaAndBetaHat.Item2, 0.95, knownX);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aoore)
            {
                Assert.True(aoore.Message.Contains("There must be at least three values in x and y"));
            }
        }

        [Test]
        public void PredictionInterval_WithNotEqualAmountOfXAndYData_ThrowsError()
        {
            List<double> x = new() { 75d, 145d, 55d, 88d, 122d }, y = new() { 0.48d, 1.09d, 0.53d, 0.97d, 0.78d };
            double knownX = 90d;
            Tuple<double, double> alphaAndBetaHat = Fit.Line(x.ToArray(), y.ToArray());
            //95% prediction interval
            try
            {
                x.RemoveAt(x.Count - 1);
                Tuple<double, double> predictionIntervallNextPoint = Statistics.PredictionInterval(x, y, alphaAndBetaHat.Item1, alphaAndBetaHat.Item2, 0.95, knownX);
                Assert.Fail();
            }
            catch (ArgumentException ae)
            {
                Assert.True(ae.Message.Contains("There must be an equal amount of data in x and y"));
            }
        }

        [Test]
        public void PredictionInterval_WithNaNValueInData_ThrowsError()
        {
            List<double> x = new() { 75d, 145d, 55d, 88d, 122d }, y = new() { 0.48d, 1.09d, 0.53d, 0.97d, 0.78d };
            double knownX = 90d;
            Tuple<double, double> alphaAndBetaHat = Fit.Line(x.ToArray(), y.ToArray());
            //95% prediction interval
            try
            {
                x[0] = Double.NaN;
                Tuple<double, double> predictionIntervallNextPoint = Statistics.PredictionInterval(x, y, alphaAndBetaHat.Item1, alphaAndBetaHat.Item2, 0.95, knownX);
                Assert.Fail();
            }
            catch (ArgumentException ae)
            {
                Assert.True(ae.Message.Contains("NaN values are not accepted in prediction interval data."));
            }
        }

        [Test]
        public void PredictionInterval_WithSecondIsNaN_ThrowsError()
        {
            List<double> x = new() { 75d, 145d, 55d, 88d, 122d }, y = new() { 0d, 0d, 0d, 0d, 0d };
            double knownX = 90d;
            Tuple<double, double> alphaAndBetaHat = Fit.Line(x.ToArray(), y.ToArray());
            //95% prediction interval
            try
            {
                Tuple<double, double> predictionIntervallNextPoint = Statistics.PredictionInterval(x, y, alphaAndBetaHat.Item1, alphaAndBetaHat.Item2, 0.95, knownX);
                Assert.Fail();
            }
            catch (ArithmeticException ae)
            {
                Assert.True(ae.Message.Contains("Prediction interval is not valid"));
            }
        }

        [Test]
        public void PredictionInterval_WithNoCheckXValue_ReturnsSuccess()
        {
            List<double> x = new() { 75d, 145d, 55d, 88d, 122d }, y = new() { 0.48d, 1.09d, 0.53d, 0.97d, 0.78d };

            Tuple<double, double> alphaAndBetaHat = Fit.Line(x.ToArray(), y.ToArray());
            //95% prediction interval
            Tuple<double, double> predictionIntervallNextPoint = Statistics.PredictionInterval(x, y, alphaAndBetaHat.Item1, alphaAndBetaHat.Item2, 0.95);

            double lower = predictionIntervallNextPoint.Item2;
            double upper = predictionIntervallNextPoint.Item1;
            lower.FirstNSignificantDigits(2);
            upper.FirstNSignificantDigits(2);
            Assert.AreEqual(Math.Round(0.22d, 3), Math.Round(lower, 3));
            Assert.AreEqual(Math.Round(1.60d, 3), Math.Round(upper, 3));
        }
    }
}
