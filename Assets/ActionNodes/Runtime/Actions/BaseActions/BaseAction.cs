using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Actions
{
    public abstract class BaseAction : IAction
    {
        protected IActionRunner Runner { get; private set; }

        [SerializeField]
        private bool _dontWait;

        public bool DontWait => _dontWait;

        public abstract Task RunAsync(CancellationToken cancellationToken);
        public abstract void Stop();

        void IAction.SetParent(IActionRunner runner)
        {
            Runner = runner;
        }
    }
}
