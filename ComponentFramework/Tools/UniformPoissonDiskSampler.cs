using System;
using System.Collections.Generic;
using SlimDX;

namespace ComponentFramework.Tools
{
    // Adapated from java source by Herman Tulleken
    // http://www.luma.co.za/labs/2008/02/27/poisson-disk-sampling/

    // The algorithm is from the "Fast Poisson Disk Sampling in Arbitrary Dimensions" paper by Robert Bridson
    // http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf

    public class UniformPoissonDiskSampler
    {
        static readonly float SquareRootTwo = (float) Math.Sqrt(2);

        readonly Random Random = new Random();

        Vector2 TopLeft, LowerRight, Center;
        Vector2 Dimensions;
        float? RejectionSqDistance;
        float MinimumDistance;
        float CellSize;
        int GridWidth, GridHeight;

        Vector2?[,] Grid;
        List<Vector2> ActivePoints, Points;

        public static float GetMinimumDistanceForUnitCircle(int sampleCount)
        {
            // Best-fit curve on the maximum sample count for distances between 0.201 (64 samples) and 0.998 (4 samples} 
            // with increments of 0.001, on 360 points per iteration for one second
            // Meaning that getting the said number of samples with these distances should take less than a second

            // Curve fitted by zunzun.com (User-Selectable Polyfunctional)
            // SSQABS: 0.00318540168163, RMSE: 0.00761028213744

            const double a = 4.4950272998476089E+02;
            const double b = 4.6112869281886475E+02;
            const double c = -6.3638213263954427E+01;
            const double d = -2.4524750522452123E+01;
            const double offset = -7.0601110895232512E+02;

            var x = sampleCount;

            return (float) (a * Math.Atan(x) + b * 1.0 / x + c * Math.Exp(-x) + d * Math.Pow(x, -1.5) + offset);
        }

        public static Vector2[] SampleUnitCircle(int sampleCount)
        {
            var minimumDistance = GetMinimumDistanceForUnitCircle(sampleCount);
            Vector2[] samples;
            do
            {
                samples = new UniformPoissonDiskSampler().Sample(new Vector2(-1), new Vector2(1), 1, minimumDistance, 360);
            } 
            while (samples.Length != sampleCount);
            return samples;
        }

        public static Vector2[] SampleCircle(Vector2 center, float radius, float minimumDistance)
        {
            return SampleCircle(center, radius, minimumDistance, 30);
        }
        public static Vector2[] SampleCircle(Vector2 center, float radius, float minimumDistance, int pointsPerIteration)
        {
            return new UniformPoissonDiskSampler().Sample(center - new Vector2(radius), center + new Vector2(radius), radius, minimumDistance, pointsPerIteration);
        }

        public static Vector2[] SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance)
        {
            return SampleRectangle(topLeft, lowerRight, minimumDistance, 30);
        }
        public static Vector2[] SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance, int pointsPerIteration)
        {
            return new UniformPoissonDiskSampler().Sample(topLeft, lowerRight, null, minimumDistance, pointsPerIteration);
        }

        Vector2[] Sample(Vector2 topLeft, Vector2 lowerRight, float? rejectionDistance, float minimumDistance, int pointsPerIteration)
        {
            TopLeft = topLeft;
            LowerRight = lowerRight;
            Dimensions = lowerRight - topLeft;
            Center = (topLeft + lowerRight) / 2;
            CellSize = minimumDistance / SquareRootTwo;
            MinimumDistance = minimumDistance;
            RejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance;
            GridWidth = (int) (Dimensions.X / CellSize) + 1;
            GridHeight = (int) (Dimensions.Y / CellSize) + 1;

            Grid = new Vector2?[GridWidth, GridHeight];
            ActivePoints = new List<Vector2>();
            Points = new List<Vector2>();

            AddFirstPoint();

            while (ActivePoints.Count != 0)
            {
                var listIndex = Random.Next(ActivePoints.Count);

                var point = ActivePoints[listIndex];
                var found = false;

                for (var k = 0; k < pointsPerIteration; k++)
                    found |= AddNextPoint(point);

                if (!found)
                    ActivePoints.RemoveAt(listIndex);
            }

            return Points.ToArray();
        }

        void AddFirstPoint()
        {
            var added = false;
            while (!added)
            {
                var d = Random.NextDouble();
                var xr = TopLeft.X + Dimensions.X * d;

                d = Random.NextDouble();
                var yr = TopLeft.Y + Dimensions.Y * d;

                var p = new Vector2((float) xr, (float) yr);
                if (RejectionSqDistance != null && Vector2.DistanceSquared(Center, p) > RejectionSqDistance)
                    continue;
                added = true;

                var index = Denormalize(p, TopLeft, CellSize);

                Grid[(int) index.X, (int) index.Y] = p;

                ActivePoints.Add(p);
                Points.Add(p);
            } 
        }

        bool AddNextPoint(Vector2 point)
        {
            var found = false;
            var q = GenerateRandomAround(point, MinimumDistance);

            if (q.X >= TopLeft.X && q.X < LowerRight.X && 
                q.Y > TopLeft.Y && q.Y < LowerRight.Y &&
                (RejectionSqDistance == null || Vector2.DistanceSquared(Center, q) <= RejectionSqDistance))
            {
                var qIndex = Denormalize(q, TopLeft, CellSize);
                var tooClose = false;

                for (var i = (int)Math.Max(0, qIndex.X - 2); i < Math.Min(GridWidth, qIndex.X + 3) && !tooClose; i++)
                    for (var j = (int)Math.Max(0, qIndex.Y - 2); j < Math.Min(GridHeight, qIndex.Y + 3) && !tooClose; j++)
                        if (Grid[i, j].HasValue && Vector2.Distance(Grid[i, j].Value, q) < MinimumDistance)
                            tooClose = true;

                if (!tooClose)
                {
                    found = true;
                    ActivePoints.Add(q);
                    Points.Add(q);
                    Grid[(int)qIndex.X, (int)qIndex.Y] = q;
                }
            }
            return found;
        }

        Vector2 GenerateRandomAround(Vector2 center, float minimumDistance)
        {
            var d = Random.NextDouble();
            var radius = minimumDistance + minimumDistance * d;

            d = Random.NextDouble();
            var angle = MathHelper.TwoPi * d;

            var newX = radius * Math.Sin(angle);
            var newY = radius * Math.Cos(angle);

            return new Vector2((float) (center.X + newX), (float) (center.Y + newY));
        }

        static Vector2 Denormalize(Vector2 point, Vector2 origin, double cellSize)
        {
            return new Vector2((int) ((point.X - origin.X) / cellSize), (int) ((point.Y - origin.Y) / cellSize));
        }
    }
}