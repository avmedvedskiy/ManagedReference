using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace ManagedReference.Editor
{
    [CustomPropertyDrawer(typeof(ManagedReferenceAttribute))]
    public class ManagedReferenceAttributeDrawer : BaseReferenceAttributeDrawer
    {
        protected override void CacheTypes(SerializedProperty property)
        {
            if(Types != null)
                return;
            
            var managedAttribute = attribute as ManagedReferenceAttribute;
            if (managedAttribute.customAttribute == null)
            {
                Type targetType = property.GetManagedReferenceFieldType();
                if (managedAttribute.genericAttribute)
                {
                    Type type = null;
                    if (fieldInfo?.DeclaringType?.IsGenericType ?? false)
                    {
                        type = fieldInfo.DeclaringType.GenericTypeArguments[0];
                    }
                    InitTypes(targetType,
                        managedAttribute.genericType ??
                        type ?? property.GenericTargetTypeArgumentDeep());
                }
                else
                    InitTypes(targetType);
            }
            else
            {
                InitTypesByAttribute(managedAttribute.customAttribute);
            }
        }
    }
}