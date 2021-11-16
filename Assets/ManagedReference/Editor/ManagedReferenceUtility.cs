using System;
using System.Reflection;
using UnityEditor;
namespace ManagedReference
{
	public static class ManagedReferenceUtility
	{
		public static Type GetManagedReferenceType(this SerializedProperty property)
		{
			if (property.propertyType != SerializedPropertyType.ManagedReference)
			{
				throw SerializedPropertyTypeMustBeManagedReference(nameof(property));
			}
			return GetType(property.managedReferenceFullTypename);
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
			object obj = (type != null) ? Activator.CreateInstance(type) : null;
			property.managedReferenceValue = obj;
			return obj;
		}

		static Type GetType(string typeName)
		{
			int splitIndex = typeName.IndexOf(' ');
			var assembly = Assembly.Load(typeName.Substring(0, splitIndex));
			return assembly.GetType(typeName.Substring(splitIndex + 1));
		}

		static ArgumentException SerializedPropertyTypeMustBeManagedReference(string paramName)
		{
			return new ArgumentException("The serialized property type must be SerializedPropertyType.ManagedReference.", paramName);
		}

	}
}