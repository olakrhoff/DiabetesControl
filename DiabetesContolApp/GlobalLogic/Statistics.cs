using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.Distributions;

namespace DiabetesContolApp.GlobalLogic
{
    public static class Statistics
    {
        /// <summary>
        /// Takes a list of data points on which it runs a prediction intervall with a given
        /// certainty.
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="alphaHat"></param>
        /// <param name="betaHat"></param>
        /// <param name="confidenceAlpha"></param>
        /// <returns>Returns the upper and lower values of the prediction intervall for the next expected point</returns>
        public static Tuple<double, double> PredictionInterval(List<double> xValues, List<double> yValues, double alphaHat, double betaHat, double confidenceAlpha, double? checkXValue = null)
        {
            int n = xValues.Count; //n, number of observations

            if (n < 3)
                throw new ArgumentOutOfRangeException("There must be at least three values in x and y");
            if (xValues.Count != yValues.Count)
                throw new ArgumentException("There must be an equal amount of data in x and y");
            for (int i = 0; i < xValues.Count; ++i)
                if (Double.IsNaN(xValues[i]) || Double.IsNaN(yValues[i]))
                    throw new ArgumentException("NaN values are not accepted in prediction interval data.");

            double xAverage = xValues.Average();
            double yAverage = yValues.Average();

            double SST = 0, SSR = 0;

            yValues.ForEach(y => SST += Math.Pow(y - yAverage, 2)); //Setting SST

            xValues.ForEach(x => SSR += Math.Pow(alphaHat + betaHat * x - yAverage, 2)); //Setting SSR

            double SSE = SST - SSR;

            double tValue = GetCriticalTValue(confidenceAlpha, n - 2);

            double s = Math.Sqrt(SSE / (n - 2)); //Estimate for sigma

            double xMinusAverageSum = 0;

            xValues.ForEach(x => xMinusAverageSum += Math.Pow(x - xAverage, 2));

            double betaHatVariance = Math.Pow(s, 2) / xMinusAverageSum;

            double SEBetaHat = Math.Sqrt(betaHatVariance);


            double nextX;
            if (checkXValue == null)
                nextX = xValues[xValues.Count - 1]; //Takes the last x-value as an estimate for the next x-value
            else
                nextX = (double)checkXValue;
            double first = alphaHat + betaHat * nextX;
            double second = tValue * s * Math.Sqrt(1 + 1 / n + Math.Pow((nextX - xAverage) / (s / SEBetaHat), 2));
            if (Double.IsNaN(second))
                throw new ArithmeticException("Prediction interval is not valid");
            double upper = first + second;
            double lower = first - second;
            return new(upper, lower);
        }

        public static double GetCriticalTValue(double alpha, int freedom)
        {
            return StudentT.InvCDF(0, 1, freedom, 1 - alpha / 2);
        }
    }
}
