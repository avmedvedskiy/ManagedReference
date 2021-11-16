using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManagedReference
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ManagedReferenceAttribute : PropertyAttribute
    {

    }
}