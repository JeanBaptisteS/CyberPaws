using System;
using System.Threading.Tasks;

namespace PxlSpace.Fox
{
	public static class TaskExt
	{
        /// <summary>
        /// Blocks while condition is true or timeout occurs.
        /// </summary>
        /// <param name="_condition">The condition that will perpetuate the block.</param>
        /// <param name="_frequency">The frequency at which the condition will be check, in milliseconds.</param>
        /// <param name="_timeout">Timeout in milliseconds.</param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task WaitWhile(Func<bool> _condition, int _frequency = 25, int _timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (_condition()) await Task.Delay(_frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(_timeout)))
                throw new TimeoutException();
        }

        /// <summary>
        /// Blocks until condition is true or timeout occurs.
        /// </summary>
        /// <param name="_condition">The break condition.</param>
        /// <param name="_frequency">The frequency at which the condition will be checked.</param>
        /// <param name="_timeout">The timeout in milliseconds.</param>
        /// <returns></returns>
        public static async Task WaitUntil(Func<bool> _condition, int _frequency = 25, int _timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!_condition()) await Task.Delay(_frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask,
                    Task.Delay(_timeout)))
                throw new TimeoutException();
        }
    }
}