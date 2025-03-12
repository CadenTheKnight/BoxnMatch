using System.Threading.Tasks;

namespace Assets.Scripts.Framework.Utilities
{
    public class Tests
    {
        /// <summary>
        /// Test method to simulate loading.
        /// </summary>
        /// <param name="milliSeconds">Time in milliseconds to simulate.</param>
        public static async Task LoadingTest(int milliSeconds)
        {
            await Task.Delay(milliSeconds);
        }
    }
}