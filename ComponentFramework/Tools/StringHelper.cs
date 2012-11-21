using System;
using System.Collections.Generic;
using System.Text;

namespace ComponentFramework.Tools
{
    public static class StringHelper
    {
        public static bool ContainsIgnoreCase(this string source, string value)
        {
            int results = source.IndexOf(value, StringComparison.CurrentCultureIgnoreCase);
            return results == -1 ? false : true;
        }

        public static string DeepToString<T>(IEnumerable<T> collection) where T : class
        {
            return DeepToString(collection, false);
        }
        public static string DeepToString<T>(IEnumerable<T> collection, bool omitBrackets) where T : class
        {
            var builder = new StringBuilder(omitBrackets ? string.Empty : "{");

            foreach (T obj in collection)
            {
                builder.Append(obj == null ? string.Empty : obj.ToString());
                builder.Append(", ");
            }
            if (builder.Length != 0)
                builder.Remove(builder.Length - 2, 2);
            if (!omitBrackets)
                builder.Append("}");

            return builder.ToString();
        }

        public static string ReflectToString(object obj)
        {
            return ReflectToString(obj, false);
        }
        public static string ReflectToString(object obj, bool omitBrackets)
        {
            var builder = new StringBuilder(omitBrackets ? string.Empty : "{");

            foreach (var member in ReflectionHelper.GetSerializableMembers(obj.GetType()))
            {
                builder.AppendFormat("{0}:{1}", member.Name, ReflectionHelper.GetValue(member, obj));
                builder.Append(", ");
            }
            if (builder.Length != 0)
                builder.Remove(builder.Length - 2, 2);
            if (!omitBrackets)
                builder.Append("}");

            return builder.ToString();
        }
    }
}
