using System;
using System.Linq;
using ComponentFramework.Tools;

namespace ComponentFramework.Structures
{
    public static class GaussianKernel
    {
        public static double IdealStandardDeviation(int taps)
        {
            switch (taps)
            {
                case 5: return 7.013915463849E-01;
                case 7: return 8.09171316279E-01;
                case 9: return 9.0372907227E-01;
                case 11: return 9.890249035E-01;
                case 13: return 1.067359295;
                case 15: return 1.1402108;
                case 17: return 1.2086;
                default:
                    throw new ArgumentException("No ideal standard deviation for this tap count : " + taps, "taps");
            }
        }

        public static double RecommendedStandardDeviation(int taps)
        {
            switch (taps)
            {
                case 5: return 1.54;
                case 7: return 1.78;
                case 9: return 2.12;
                case 11: return 2.54;
                case 13: return 2.95;
                case 15: return 3.34;
                case 17: return 4.95;
                default:
                    throw new ArgumentException("No ideal standard deviation for this tap count : " + taps, "taps");
            }
        }

        public static float[] GetWeights(int taps)
        {
            return GetWeights(taps, IdealStandardDeviation(taps));
        }
        public static float[] GetWeights(int taps, double standardDeviation)
        {
            if (taps < 1 || taps % 2 == 0)
                throw new ArgumentException("Invalid tap count : " + taps, "taps");

            return GetWeightsInternal(taps, standardDeviation).Select(x => (float)x).ToArray();
        }

        public static void GetFastWeights(int taps, out float[] weights, out float[] offsets)
        {
            GetFastWeights(taps, IdealStandardDeviation(taps), out weights, out offsets);
        }
        public static void GetFastWeights(int taps, double standardDeviation, out float[] weights, out float[] offsets)
        {
            if (taps < 1 || taps % 2 == 0)
                throw new ArgumentException("Invalid tap count : " + taps, "taps");

            double[] tempWeights = GetWeightsInternal(taps, standardDeviation);

            bool hasEndTap = tempWeights.Length % 2 == 0;
            int joinedTaps = (tempWeights.Length - 1) / 2,
                totalTaps = joinedTaps + 1 + (hasEndTap ? 1 : 0);

            offsets = new float[totalTaps];
            weights = new float[totalTaps];

            weights[0] = (float)tempWeights[0];
            offsets[0] = 0;

            for (int i = 0; i < joinedTaps; i++)
            {
                double sum = tempWeights[i * 2 + 1] + tempWeights[i * 2 + 2];
                weights[i + 1] = (float)sum;
                offsets[i + 1] = (float)(0.5 - tempWeights[i * 2 + 1] / sum);
            }

            if (hasEndTap)
                weights[weights.Length - 1] = (float)tempWeights[tempWeights.Length - 1];
        }

        static double[] GetWeightsInternal(int taps, double standardDeviation)
        {
            int halfTaps = (taps - 1) / 2 + 1;

            var weights = new double[halfTaps];
            for (int i = 0; i < halfTaps; i++)
                weights[i] = Gaussian(i, standardDeviation);

            double augmentationFactor = 1 + LostLight(taps, standardDeviation);
            for (int i = 0; i < halfTaps; i++)
                weights[i] *= augmentationFactor;

            return weights;
        }

        static double LostLight(int taps, double standardDeviation)
        {
            int halfTaps = (taps - 1) / 2 + 1;

            double sum = Gaussian(0, standardDeviation);
            for (int i = 1; i < halfTaps; i++)
                sum += Gaussian(i, standardDeviation) * 2;

            return -1 + 1 / sum;
        }

        static double Gaussian(int distance, double standardDeviation)
        {
            return 1 / (Math.Sqrt(MathHelper.TwoPi) * standardDeviation) *
                   Math.Exp(-Math.Pow(distance, 2) / (2 * Math.Pow(standardDeviation, 2)));
        }
    }
}