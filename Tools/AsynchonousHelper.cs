using System.Threading.Tasks;

namespace Tools
{
    public static class AsynchronousHelper
    {
        /// <summary>
        /// Return `value` after `timeToWait` milliseconds.
        /// </summary>
        public static async Task<bool> WaitBeforeSetting(int timeToWait, bool value)
        {
            await Task.Delay(timeToWait);
            return value;
        }
    }

}
