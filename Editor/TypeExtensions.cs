using System;
using System.ComponentModel;
using UnityEditor;

namespace ManagedReference.Editor
{
    public static class TypeExtensions
    {
        public static Type GenericTypeArgumentDeep(this Type type, int order = 0)
        {
            if (type == null)
                return null;

            return type.GenericTypeArguments.Length > 0
                ? type.GenericTypeArguments[order]
                : GenericTypeArgumentDeep(type.BaseType);
        }

        public static bool ContainsGenericInterfaceTypeArgumentDeep(this Type type, Type searchedType, int order = 0)
        {
            //else check interfaces
            foreach (var i in type.GetInterfaces())
            {
                if (i.GenericTypeArguments.Length > 0 && order < i.GenericTypeArguments.Length)
                {
                    if (i.GenericTypeArguments[order].IsAssignableFrom(searchedType))
                        return true;
                }
            }

            //else check base class
            var baseGenericType = type.GenericTypeArgumentDeep();
            return baseGenericType != null && baseGenericType.IsAssignableFrom(searchedType);
        }

        public static string GetNameWithCategory(this Type type)
        {
            var group = (CategoryAttribute)Attribute.GetCustomAttribute(type,
                typeof(CategoryAttribute));

            return group == null ? type.Name : $"{group.Category}/{type.Name}";
        }


        public static Type GenericTargetTypeArgumentDeep(this SerializedProperty property, int order = 0)
        {
            return property.serializedObject.targetObject.GetType().GenericTypeArgumentDeep(order);
        }
    }
}