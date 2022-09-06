using System;
using UnityEditor;

namespace ManagedReference.Editor
{
    public static class TypeExtensions
    {
        public static Type GenericTypeArgumentDeep(this Type type)
        {
            if (type == null)
                return null;

            return type.GenericTypeArguments.Length > 0
                ? type.GenericTypeArguments[0]
                : GenericTypeArgumentDeep(type.BaseType);
        }
        
        public static Type GenericInterfaceTypeArgumentDeep(this Type type)
        {
            //else check interfaces
            foreach (var i in type.GetInterfaces())
            {
                if (i.GenericTypeArguments.Length > 0)
                    return i.GenericTypeArguments[0];
            }

            //else check base class
            return type.GenericTypeArgumentDeep();
        }
        
        
        public static Type GenericTargetTypeArgumentDeep(this SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetType().GenericTypeArgumentDeep();
        }
    }
}