using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ManagedReference
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [MeansImplicitUse]
    public class ManagedReferenceAttribute : PropertyAttribute
    {
        /// <summary>
        /// Draw dropdown with all types with this attribute type, use DrawInManagedReferenceAttribute or own custom
        /// </summary>
        public Type customAttribute;

        /// <summary>
        /// Will be filtered by parent generic type
        /// </summary>
        public bool genericAttribute;
        
        /// <summary>
        /// Order of generic attributes 0 - first, 1 second
        /// </summary>
        public int genericAttributeOrder = 0;
        
        /// <summary>
        /// Will be filtered by generic type, need set genericAttribute = true and Type
        /// </summary>
        public Type genericType;
    }
}