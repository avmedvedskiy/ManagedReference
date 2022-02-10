using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManagedReference
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ManagedReferenceAttribute : PropertyAttribute
    {
        /// <summary>
        /// Draw dropdown with all types with this attribute type, use DrawInManagedReferenceAttribute or own custom
        /// </summary>
        public Type customAttribute;
    }
}