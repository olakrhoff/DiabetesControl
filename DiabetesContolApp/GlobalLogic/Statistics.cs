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
            const uint NUMBER_OF_SIGNIFICAT_FIGURES = 2;
            int n = xValues.Count; //n, number of observations

            if (n < 3)
                throw new ArgumentOutOfRangeException("There must be at least three values in x and y");
            if (xValues.Count != yValues.Count)
                throw new ArgumentException("There must be an equal amount of data in x and y");
            for (int i = 0; i < xValues.Count; ++i)
                if (Double.IsNaN(xValues[i]) || Double.IsNaN(yValues[i]))
                    throw new ArgumentException("NaN values are not accepted in prediction interval data.");


            alphaHat.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);
            betaHat.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            double xAverage = xValues.Average();
            xAverage.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);
            double yAverage = yValues.Average();
            yAverage.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            double SST = 0, SSR = 0;

            yValues.ForEach(y => SST += Math.Pow(y - yAverage, 2)); //Setting SST
            SST.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            xValues.ForEach(x => SSR += Math.Pow(alphaHat + betaHat * x - yAverage, 2)); //Setting SSR
            SSR.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            double SSE = SST - SSR;
            SSE.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            double tValue = GetCriticalTValue(confidenceAlpha + (1 - confidenceAlpha) / 2, n - 2);
            tValue.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            double s = Math.Sqrt(SSE / (n - 2)); //Estimate for sigma
            s.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            double xMinusAverageSum = 0;

            xValues.ForEach(x => xMinusAverageSum += Math.Pow(x - xAverage, 2));
            xMinusAverageSum.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            double betaHatVariance = Math.Pow(s, 2) / xMinusAverageSum;
            betaHatVariance.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            double SEBetaHat = Math.Sqrt(betaHatVariance);
            SEBetaHat.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            double nextX;
            if (checkXValue == null)
                nextX = xValues[xValues.Count - 1]; //Takes the last x-value as an estimate for the next x-value
            else
                nextX = (double)checkXValue;
            double first = alphaHat + betaHat * nextX;
            first.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            double second = tValue * s * Math.Sqrt(1d + 1d / n + Math.Pow((nextX - xAverage) / (s / SEBetaHat), 2));
            second.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES + 1);

            if (Double.IsNaN(second))
                throw new ArithmeticException("Prediction interval is not valid");
            double upper = first + second;
            upper.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES);

            double lower = first - second;
            lower.FirstNSignificantDigits(NUMBER_OF_SIGNIFICAT_FIGURES);

            return new(upper, lower);
        }

        public static double GetCriticalTValue(double alpha, int freedom)
        {
            return StudentT.InvCDF(0, 1, freedom, alpha);
        }
    }
}
