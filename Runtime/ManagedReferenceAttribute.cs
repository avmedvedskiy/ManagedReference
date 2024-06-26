using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ManagedReference
{
    [AttributeUsage(AttributeTargets.Field)]
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
        /// Will be filtered by generic type, need set genericAttribute = true and Type
        /// </summary>
        public Type genericType;
    }
}