using System;
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
                    if (searchedType == i.GenericTypeArguments[order])
                        return true;
                }
            }

            //else check base class
            return type.GenericTypeArgumentDeep() == searchedType;
        }
        
        
        public static Type GenericTargetTypeArgumentDeep(this SerializedProperty property, int order = 0)
        {
            return property.serializedObject.targetObject.GetType().GenericTypeArgumentDeep(order);
        }
    }
}