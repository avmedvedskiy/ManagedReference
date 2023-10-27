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
            /*
            if (!string.IsNullOrEmpty(managedAttribute.typeFromOtherProperty))
            {
                Type searchType = null;
                string searchPropName = managedAttribute.typeFromOtherProperty;
                var iterator = property.serializedObject.GetIterator();
                while (iterator.NextVisible(true))
                {
                    Debug.Log($"{searchPropName} == {iterator.name} && {property.depth} == {iterator.depth}");
                    if (iterator.name == searchPropName && iterator.depth == property.depth - 1)
                    {
                        searchType = iterator.objectReferenceValue?.GetType();
                        break;
                    }

                    //if(iterator.depth > property.depth)
                    //    break;
                }
                
                var nextProperty = property.Copy();
                nextProperty.NextVisible(false);
                //searchType = nextProperty.objectReferenceValue?.GetType();
                if (searchType != null)
                {
                    var targetType = property.GetManagedReferenceFieldType();
                    InitTypes(targetType, searchType);
                }
                nextProperty.Dispose();
                return;
            }
            */

            if (managedAttribute.customAttribute == null)
            {
                Type targetType = property.GetManagedReferenceFieldType();
                if (managedAttribute.genericAttribute)
                {
                    InitTypes(targetType,
                        managedAttribute.genericType ??
                        property.GenericTargetTypeArgumentDeep(managedAttribute.genericAttributeOrder));
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