using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Actions
{
    [ManagedReference.ManagedReferenceGroup("Array")]
    [Serializable]
    public class ArrayAction : BaseAction
    {
        [SerializeReference, ManagedReference.ManagedReference]
        protected List<IAction> _actions;

        private ActionRunner _runner;
        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            //maybe need cancelation token, not sure - need to test
            _runner = new ActionRunner();
            await _runner.RunAsync(_actions);
        }

        public override void Stop()
        {
            _runner.Stop();
        }
    }
}
