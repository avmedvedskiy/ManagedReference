using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ManagedReference.Editor;
using UnityEditor;
using UnityEngine;

namespace ManagedReference
{
    public static class ManagedReferenceExtensions
    {
        public static List<Type> GetTypes(Type parentType)
        {
            return TypeCache
                .GetTypesDerivedFrom(parentType)
                .Where(p =>
                    (p.IsPublic || p.IsNestedPublic) &&
                    !p.IsAbstract &&
                    !p.IsGenericType &&
                    !typeof(UnityEngine.Object).IsAssignableFrom(p) &&
                    Attribute.IsDefined(p, typeof(SerializableAttribute)))
                .OrderBy(x => x.Name)
                .ToList();
        }

        public static List<Type> GetTypes(Type parentType, params Type[] genericType)
        {
            return TypeCache
                .GetTypesDerivedFrom(parentType)
                .Where(p =>
                    (p.IsPublic || p.IsNestedPublic) &&
                    !p.IsAbstract &&
                    p.ContainsGenericInterfaceTypeArgumentDeep(genericType) &&
                    !typeof(UnityEngine.Object).IsAssignableFrom(p) &&
                    Attribute.IsDefined(p, typeof(SerializableAttribute)))
                .OrderBy(x => x.Name)
                .ToList();
        }

        public static List<Type> GetTypesByAttribute(Type typeAttribute)
        {
            return TypeCache
                .GetTypesWithAttribute(typeAttribute)
                .Where(p =>
                    (p.IsPublic || p.IsNestedPublic) &&
                    !p.IsAbstract &&
                    !p.IsGenericType &&
                    !typeof(UnityEngine.Object).IsAssignableFrom(p) &&
                    Attribute.IsDefined(p, typeof(SerializableAttribute)))
                .OrderBy(x => x.Name)
                .ToList();
        }

        public static Type GetManagedReferenceType(this SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                throw SerializedPropertyTypeMustBeManagedReference(nameof(property));
            }

            return string.IsNullOrEmpty(property.managedReferenceFullTypename)
                ? null
                : GetType(property.managedReferenceFullTypename);
        }

        public static Type GetManagedReferenceFieldType(this SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                throw SerializedPropertyTypeMustBeManagedReference(nameof(property));
            }

            return GetType(property.managedReferenceFieldTypename);
        }

        public static object SetManagedReference(this SerializedProperty property, Type type)
        {
            object obj = type != null ? Activator.CreateInstance(type) : null;
            property.managedReferenceValue = obj;
            return obj;
        }

        public static object SetManagedReferenceWithCopyValues(this SerializedProperty property, Type type)
        {
            object oldValue = property.managedReferenceValue;
            object newValue = type != null ? Activator.CreateInstance(type) : null;
            newValue.CopyValuesFrom(oldValue);
            property.managedReferenceValue = newValue;
            return newValue;
        }

        static Type GetType(string typeName)
        {
            int splitIndex = typeName.IndexOf(' ');
            var assembly = Assembly.Load(typeName.Substring(0, splitIndex));
            return assembly.GetType(typeName.Substring(splitIndex + 1));
        }

        static ArgumentException SerializedPropertyTypeMustBeManagedReference(string paramName)
        {
            return new ArgumentException(
                "The serialized property type must be SerializedPropertyType.ManagedReference.", paramName);
        }
    }
}