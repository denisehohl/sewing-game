namespace Ateo.Extensions
{
    /// <summary>
    /// Timef provides functionality related to time
    /// </summary>
    public static class Timef
    {
        /// <summary>
        /// Converts seconds to milliseconds and returns an integer
        /// </summary>
        /// <param name="seconds">Seconds as float</param>
        public static int SecondsToMilliseconds(float seconds)
        {
            return (int) (seconds * 1000f);
        }
        
        /// <summary>
        /// Converts seconds to milliseconds and returns a long
        /// </summary>
        /// <param name="seconds">Seconds as double</param>
        public static long SecondsToMilliseconds(double seconds)
        {
            return (long) (seconds * 1000f);
        }
        
        /// <summary>
        /// Converts milliseconds to seconds and returns a float
        /// </summary>
        /// /// <param name="milliseconds">Seconds as integer</param>
        public static float MillisecondsToSeconds(int milliseconds)
        {
            return milliseconds / 1000f;
        }
        
        /// <summary>
        /// Converts milliseconds to seconds and returns a float
        /// </summary>
        /// /// <param name="milliseconds">Seconds as double</param>
        public static double MillisecondsToSeconds(long milliseconds)
        {
            return milliseconds / 1000f;
        }
    }
}