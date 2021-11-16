using System.Threading;
using System.Threading.Tasks;

namespace Actions
{
    public interface IAction
    {
        bool DontWait { get; }

        /// <summary>
        /// Run Action async
        /// </summary>
        /// <returns></returns>
        Task RunAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Called when action should be stoped
        /// </summary>
        void Stop();

#if UNITY_2020_1_OR_NEWER
        internal void SetParent(IActionRunner runner);
#else
        void SetParent(IActionRunner runner);
#endif
    }
}
