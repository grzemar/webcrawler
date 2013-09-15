using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NaiveBayesClassifier
{
    internal static class MathUtility
    {
        internal static double Variance(this IEnumerable<double> source)
        {
            if (source.Count() == 1) return 0.0d;
            double avg = source.Average();
            double d = source.Aggregate(0.0d,
                (total, next) => total += Math.Pow(next - avg, 2));
            return d / (double)(source.Count() - 1);
        }

        internal static double Mean(this IEnumerable<double> source)
        {
            if (source.Count() < 1)
                return 0.0d;
            double length = source.Count();
            double sum = source.Sum();
            return sum / length;
        }

        internal static double NormalDistance(double myValue, double mean, double standardDeviation)
        {
            double factor = standardDeviation * Math.Sqrt(2.0d * Math.PI);
            if (factor < 0.001d && factor > -0.001d) return 0.0d;
            double exponent = (myValue - mean) * (myValue - mean) / (2.0d * standardDeviation * standardDeviation);
            return Math.Exp(-exponent) / factor;
        }

        internal static double SquareRoot(double source)
        {
            return Math.Sqrt(source);
        }

        internal static IEnumerable<double> SelectRows(this DataTable table, int column, string filter)
        {
            List<double> doubleList = new List<double>();
            DataRow[] rows = table.Select(filter);
            for (int i = 0; i < rows.Length; i++)
            {
                doubleList.Add((double)rows[i][column]);
            }

            return doubleList;
        }
    }
}
