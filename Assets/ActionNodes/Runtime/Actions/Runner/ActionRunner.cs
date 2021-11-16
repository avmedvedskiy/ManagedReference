#pragma warning disable CS4014
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Actions
{
    public interface IActionRunner
    {
        Task RunAsync<T>(List<T> actions, int startedIndex = 0) where T : class, IAction;
    }

    public class ActionRunner : IActionRunner
    {
        public bool IsRunned { get; private set; }
        public IAction CurrentAction { get => _currentAction; }

        private Queue<IAction> _queue = new Queue<IAction>();
        private CancellationTokenSource _token;
        private IAction _currentAction;

        public async Task RunAsync<T>(List<T> actions, int startedIndex = 0) where T: class, IAction
        {
            for (int i = startedIndex; i < actions.Count; i++)
            {
                _queue.Enqueue(actions[i]);
            }

            if (IsRunned)
                return;

            await Run();
        }

        public async Task RunAsync(List<IAction> actions, int startedIndex = 0) 
        {
            await RunAsync<IAction>(actions, startedIndex);
        }

        private async Task Run()
        {
            _token = new CancellationTokenSource();
            IsRunned = true;


            do
            {
                _currentAction = this._queue.Dequeue();
                _currentAction.SetParent(this);
                try
                {
                    if (_currentAction.DontWait)
                        _currentAction.RunAsync(_token.Token);
                    else
                        await _currentAction.RunAsync(_token.Token);
                }
                catch (TaskCanceledException)
                {
                    Debug.Log($"Task stoped {_currentAction}");
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            while (this._queue.Count > 0 && !_token.Token.IsCancellationRequested);

            IsRunned = false;
        }


        public void Stop()
        {
            _token?.Cancel();
            _token?.Dispose();
            while (this._queue.Count > 0)
            {
                try
                {
                    var action = this._queue.Dequeue();
                    action.Stop();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }
}
