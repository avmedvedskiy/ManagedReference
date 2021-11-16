using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManagedReference
{
    public class ManagedReferenceExample : MonoBehaviour
    {
        #region Example classes
        public interface ITask
        {
            void Run();
        }
        [System.Serializable]
        public class A : ITask
        {
            [Range(0,100)]
            public float value;
            public virtual void Run()
            {
                throw new System.NotImplementedException();
            }
        }

        [System.Serializable]
        public class B : A
        {
            public string Bint;
            public override void Run()
            {
                throw new System.NotImplementedException();
            }
        }


        [System.Serializable]
        public class C : B
        {
            public ScriptableObject CSO;
            public override void Run()
            {
                throw new System.NotImplementedException();
            }
        }

        [System.Serializable]
        [ManagedReferenceGroup("Nested")]
        public class NesteD : ITask
        {
            [SerializeReference, ManagedReference]
            public ITask nestedTask;

            public void Run()
            {
                throw new System.NotImplementedException();
            }
        }

        [System.Serializable]
        [ManagedReferenceGroup("Nested")]
        public class ArrayNested : ITask
        {
            [SerializeReference, ManagedReference]
            public ITask[] array;

            public void Run()
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion

        [ManagedReference,SerializeReference]
        public ITask singleTask;

        [ManagedReference, SerializeReference]
        public A nesterAclasses;

        [ManagedReference, SerializeReference]
        public List<ITask> listTasks;
    }
}

