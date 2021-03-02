using System;
using System.Collections.Generic;
using UnityEngine;

namespace MHLab.Patch.Launcher.Scripts
{
    public sealed class Dispatcher : MonoBehaviour
    {
        private readonly Queue<Action> _actions = new Queue<Action>();

        public void Invoke(Action action)
        {
            lock (_actions)
            {
                _actions.Enqueue(action);
            }
        }

        private void Update()
        {
            lock (_actions)
            {
                while (_actions.Count > 0)
                {
                    var action = _actions.Dequeue();
                    action.Invoke();
                }
            }
        }
    }
}