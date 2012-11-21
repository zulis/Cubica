using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ComponentFramework.Tools
{
    // Source : http://www.codeproject.com/KB/cs/EnumComparer.aspx

    public class EnumComparer<TEnum> : IEqualityComparer<TEnum> 
        where TEnum : struct, IComparable, IConvertible, IFormattable
    {
        static readonly Func<TEnum, TEnum, bool> EqualsMethod = GenerateEquals();
        static readonly Func<TEnum, int> GetHashCodeMethod = GenerateGetHashCode();

        public static readonly EnumComparer<TEnum> Instance = new EnumComparer<TEnum>();

        private EnumComparer()
        {
            if (!typeof(TEnum).IsEnum)
                throw new NotSupportedException("The type parameter " + typeof(TEnum) + " is not an enum.");
        }

        static Func<TEnum, TEnum, bool> GenerateEquals()
        {
            var xParam = Expression.Parameter(typeof(TEnum), "x");
            var yParam = Expression.Parameter(typeof(TEnum), "y");
            var equalExpression = Expression.Equal(xParam, yParam);
            return Expression.Lambda<Func<TEnum, TEnum, bool>>(equalExpression, xParam, yParam).Compile();
        }

        static Func<TEnum, int> GenerateGetHashCode()
        {
            var objParam = Expression.Parameter(typeof(TEnum), "obj");
            var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
            var convertExpression = Expression.Convert(objParam, underlyingType);
            var getHashCodeMethod = underlyingType.GetMethod("GetHashCode");
            var getHashCodeExpression = Expression.Call(convertExpression, getHashCodeMethod);
            return Expression.Lambda<Func<TEnum, int>>(getHashCodeExpression, objParam).Compile();
        }

        public bool Equals(TEnum x, TEnum y)
        {
            return EqualsMethod(x, y);
        }

        public int GetHashCode(TEnum obj)
        {
            return GetHashCodeMethod(obj);
        }
    }
}
