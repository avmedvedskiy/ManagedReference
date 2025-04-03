using System;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ManagedReference.Editor
{
    public static class TypeExtensions
    {
        private static Type GenericTypeArgumentDeep(this Type type)
        {
            if (type == null)
                return null;

            return type.GenericTypeArguments.Length > 0
                ? type.GenericTypeArguments[0]
                : GenericTypeArgumentDeep(type.BaseType);
        }

        private static bool IsAssignableFrom(this Type type, params Type[] parentTypes) =>
            parentTypes.Any(type.IsAssignableFrom);

        public static bool ContainsGenericInterfaceTypeArgumentDeep(this Type type, params Type[] searchedType)
        {
            //else check interfaces
            if (type.BaseType == null)
            {
                foreach (var i in type.GetInterfaces())
                {
                    if (i.GenericTypeArguments.Any(x => x.IsAssignableFrom(searchedType)))
                        return true;
                }
            }

            //else check base class
            var baseGenericType = type.GenericTypeArgumentDeep();
            return baseGenericType != null && baseGenericType.IsAssignableFrom(searchedType);
        }

        public static string GetNameWithCategory(this Type type)
        {
            var category = type.GetCategory();

            return string.IsNullOrEmpty(category) ? type.Name : $"{category}/{type.Name}";
        }

        public static string GetCategory(this Type type)
        {
            var group = (CategoryAttribute)Attribute.GetCustomAttribute(type,
                typeof(CategoryAttribute));
            return group?.Category;
        }


        public static Type GenericTargetTypeArgumentDeep(this SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetType().GenericTypeArgumentDeep();
        }

        public static Type GetValueType(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return typeof(int);
                case SerializedPropertyType.Boolean:
                    return typeof(bool);
                case SerializedPropertyType.Float:
                    return property.numericType == SerializedPropertyNumericType.Double
                        ? typeof(double)
                        : typeof(float);
                case SerializedPropertyType.String:
                    return typeof(string);
                case SerializedPropertyType.Color:
                    return typeof(Color);
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue?.GetType();
                case SerializedPropertyType.Vector2:
                    return typeof(Vector2);
                case SerializedPropertyType.Vector3:
                    return typeof(Vector3);
                case SerializedPropertyType.Vector4:
                    return typeof(Vector4);
                case SerializedPropertyType.Rect:
                    return typeof(Rect);
                case SerializedPropertyType.Quaternion:
                    return typeof(Quaternion);
                default:
                    return null;
            }
        }
    }
}