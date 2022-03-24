﻿using System;
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
        public static Tuple<double, double> PredictionInterval(List<double> xValues, List<double> yValues, double alphaHat, double betaHat, double confidenceAlpha)
        {
            int n = xValues.Count; //n, number of observations

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

            double nextX = xValues[xValues.Count - 1]; //Takes the last x-value as an estimate for the next x-value

            double first = alphaHat + betaHat * nextX;
            double second = tValue * s * Math.Sqrt(1 + 1 / n + Math.Pow((nextX - xAverage) / (s / SEBetaHat), 2));
            double upper = first + second;
            double lower = first - second;

            return new(upper, lower);
        }

        private static double GetCriticalTValue(double alpha, int freedom)
        {
            return StudentT.InvCDF(0, 1, freedom, 1 - alpha / 2);
        }
    }
}
