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
    public sealed class ParallelArrayAction : GenericParallelArrayAction<IAction>
    {

    }
}
