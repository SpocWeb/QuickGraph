﻿namespace QuickGraph.Algorithms
{
    public static class DistanceRelaxers
    {
        public static readonly IDistanceRelaxer ShortestDistance = new ShortestDistanceRelaxer();

        sealed class ShortestDistanceRelaxer
            : IDistanceRelaxer
        {
            internal ShortestDistanceRelaxer() { }

            public double InitialDistance => double.MaxValue;

            public int Compare(double a, double b)
            {
                return a.CompareTo(b);
            }

            public double Combine(double distance, double weight)
            {
                return distance + weight;
            }
        }

        public readonly static IDistanceRelaxer CriticalDistance = new CriticalDistanceRelaxer();

        sealed class CriticalDistanceRelaxer :
            IDistanceRelaxer
        {
            internal CriticalDistanceRelaxer() { }

            public double InitialDistance => double.MinValue;

            public int Compare(double a, double b)
            {
                return -a.CompareTo(b);
            }

            public double Combine(double distance, double weight)
            {
                return distance + weight;
            }
        }

        public readonly static IDistanceRelaxer EdgeShortestDistance = new EdgeDistanceRelaxer();

        sealed class EdgeDistanceRelaxer
            : IDistanceRelaxer
        {

            public double InitialDistance => 0;

            public int Compare(double a, double b)
            {
                return a.CompareTo(b);
            }

            public double Combine(double distance, double weight)
            {
                return distance + weight;
            }
        }
    }
}
