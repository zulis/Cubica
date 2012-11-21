
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ComponentFramework.Tools
{
    public static class ReflectionHelper
    {
        public const BindingFlags PublicInstanceMembers = BindingFlags.Public | BindingFlags.Instance;

        public delegate object MethodHandler(object target, params object[] args);

        public static IEnumerable<MemberInfo> GetSerializableMembers(Type type)
        {
            return type.GetProperties(PublicInstanceMembers | BindingFlags.FlattenHierarchy)
                .Where(p => p.GetGetMethod() != null && p.GetSetMethod() != null && p.GetGetMethod().GetParameters().Length == 0)
                .Cast<MemberInfo>()
                .Union(type.GetFields(PublicInstanceMembers | BindingFlags.FlattenHierarchy).Cast<MemberInfo>());
        }

        public static object Instantiate(Type type)
        {
            if (type.IsArray)
                return Array.CreateInstance(type.GetElementType(), 0);
            return Activator.CreateInstance(type);
        }

        public static object GetValue(MemberInfo member, object instance)
        {
            if (member is PropertyInfo)
                return (member as PropertyInfo).GetGetMethod().Invoke(instance, null);
            if (member is FieldInfo)
                return (member as FieldInfo).GetValue(instance);
            throw new NotImplementedException();
        }

        public static void SetValue(MemberInfo member, object instance, object value)
        {
            if (member is PropertyInfo)
                (member as PropertyInfo).GetSetMethod().Invoke(instance, new[] { value });
            else if (member is FieldInfo)
                (member as FieldInfo).SetValue(instance, value);
            else throw new NotImplementedException();
        }

        public static MethodHandler GetDelegate(MethodBase method)
        {
            return (instance, args) => method.Invoke(instance, args);
        }

        public static T GetAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            return (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }

        public static IEnumerable<MethodInfo> GetMethodsByAttribute<T>(Type type) where T : Attribute
        {
            return type.GetMethods(PublicInstanceMembers).Where(m => GetAttribute<T>(m) != null);
        }
    }
}
