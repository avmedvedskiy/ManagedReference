using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ManagedReference
{
    [AttributeUsage(AttributeTargets.Field)]
    [MeansImplicitUse]
    public class DynamicReferenceAttribute : PropertyAttribute
    {
        /// <summary>
        /// Will get setted type from other property, write a propertyName
        /// </summary>
        public string PropertyName { get; }
        public Type ExtraType { get; }

        public DynamicReferenceAttribute(string propertyName, Type extraType)
        {
            PropertyName = propertyName;
            ExtraType = extraType;
        }
        public DynamicReferenceAttribute(string propertyName)
        {
            PropertyName = propertyName;
            ExtraType = null;
        }
    }
}