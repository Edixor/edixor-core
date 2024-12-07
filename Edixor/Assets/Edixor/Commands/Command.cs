using UnityEngine;
using System;

namespace Commands
{
    public abstract class Command
    {
        private string name;
        public abstract void Tasks();

        public abstract void JobRollback();

        public abstract void CallAction(Action action = null);

        public abstract void RollbackAction(Action action = null);

        public abstract T Get<T>(string clarifications = null) where T : class;
    }
}

