using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions.Example
{
    public class ActionExample : MonoBehaviour
    {
        public ActionStorage storage;

        private ActionRunner _runner;

        [ContextMenu("RunAsync")]
        async void RunAsync()
        {
            _runner = new ActionRunner();
            await _runner.RunAsync(storage.Nodes);
            Debug.Log("Finish");
        }

        [ContextMenu("Stop")]
        void Stop()
        {
            _runner.Stop();
        }
    }
}
