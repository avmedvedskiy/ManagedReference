using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ManagedReference
{
    public static class ReflectionExtensions
    {
        public static void CopyValuesFrom(this object to, object from)
        {
            if (from == null || to == null)
                return;

            var fromAllFields = from.GetType().GetAllSerializedFields();
            var toAllFields = to.GetType().GetAllSerializedFields();
            foreach (var toField in toAllFields)
            {
                var referenceField = fromAllFields.Find(x => IsSameFieldName(x.Name,toField.Name));
                if (referenceField != null)
                {
                    CopyValue(referenceField, from, toField, to);
                }
            }
        }

        private static bool IsSameFieldName(string name1, string name2)
        {
            name1 = name1.Replace("_", string.Empty);
            name2 = name2.Replace("_", string.Empty);
            return name1 == name2;
        }

        private static void CopyValue(FieldInfo fromField, object from, FieldInfo toField, object to)
        {
            if (fromField.FieldType == toField.FieldType)
            {
                toField.SetValue(to, fromField.GetValue(from));
            }
        }

        private static List<FieldInfo> GetAllSerializedFields(this Type type)
        {
            var fields = new List<FieldInfo>();
            type.GetAllParentFields(fields);
            return fields;
        }

        private static void GetAllParentFields(this Type type, List<FieldInfo> fieldInfos)
        {
            if (type.BaseType == null)
                return;

            fieldInfos.AddRange(type.GetSerializedFields());
            type.BaseType?.GetAllParentFields(fieldInfos);
        }

        private static List<FieldInfo> GetSerializedFields(this Type type,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            => type
                .GetFields(flags)
                .Where(x => x.GetCustomAttributes().OfType<SerializeField>().Any() || x.IsPublic)
                .ToList();
    }
}