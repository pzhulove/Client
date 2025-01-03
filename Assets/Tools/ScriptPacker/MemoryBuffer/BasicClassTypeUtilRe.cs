using System;

namespace MemoryWriteReaderAnimation
{
	public class BasicClassTypeUtilRe
	{
		public static object CreateObject<T>()
		{
			return BasicClassTypeUtilRe.CreateObject(typeof(T));
		}

		public static object CreateObject(Type type)
		{
			if (type.ToString() == "System.String")
			{
				return string.Empty;
			}
			return Activator.CreateInstance(type);
		}

		public static object CreateListItem(Type typeList)
		{
			Type[] genericArguments = typeList.GetGenericArguments();
			if (genericArguments != null && genericArguments.Length != 0)
			{
				return BasicClassTypeUtilRe.CreateObject(genericArguments[0]);
			}
			return null;
		}
	}
}
