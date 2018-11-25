using System;

namespace Tools
{
    /// <summary>
    /// Provides a single generator for different random numbers.
    /// </summary>
    public static class RandomHelper
    {
        static Random rand;

        /// <summary>
        /// Initializes the random number generator
        /// </summary>
        public static void Initialize()
        {
            rand = new Random();
        }

        /// <summary>
        /// Returns a nonnegative random number less than maxValue (exclusive)
        /// </summary>
        /// <param name="maxValue">the exclusive max value</param>
        /// <returns>the random number</returns>
        public static int Next(int maxValue)
        {
            return rand.Next(maxValue);
        }

        /// <summary>
        /// Returns a nonnegative random number less than maxValue (exclusive)
        /// </summary>
        /// <param name="minValue">the inclusive min value</param>
        /// <param name="maxValue">the exclusive max value</param>
        /// <returns>the random number</returns>
        public static int Next(int minValue, int maxValue)
        {
            return rand.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a nonnegative random number less than maxValue (exclusive)
        /// </summary>
        /// <param name="maxValue">the exclusive max value</param>
        /// <returns>the random number</returns>
        public static float NextFloat(float maxValue)
        {
            return (float)rand.NextDouble() * maxValue;
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0
        /// </summary>
        /// <returns>the random number</returns>
        public static double NextDouble()
        {
            return rand.NextDouble();
        }

        /// <summary>
        /// Returns a random number between -1.0 and 1.0
        /// with much chance around 0.
        /// </summary>
        /// <returns>the random number</returns>
        public static double NextBinomial()
        {
            return rand.NextDouble() - rand.NextDouble();
        }
    }
}
