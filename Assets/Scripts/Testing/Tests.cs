using UnityEngine;
using System.Threading.Tasks;

namespace Assets.Scripts.Testing
{
    public class Tests : MonoBehaviour
    {
        /// <summary>
        /// Delay for testing loading sequences.
        /// </summary>
        /// <param name="delay">The delay in milliseconds.</param>
        public static async Task TestDelay(int delay)
        {
            await Task.Delay(delay);
        }

        /// <summary>
        /// Throw an exception for testing exception handling.
        /// </summary>
        public static void TestException()
        {
            throw new System.Exception("Test exception.");
        }
    }
}