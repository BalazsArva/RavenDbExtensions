using System;
using System.Collections;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public static class ObjectHelper
    {
        public static bool IsNumeric(object value)
        {
            return
                IsIntegral(value) ||
                IsFloatingPointNumber(value) ||
                IsFixedPointNumber(value);
        }

        public static bool IsLogical(object value)
        {
            return value is bool;
        }

        public static bool IsStringLike(object value)
        {
            return value is string || value is char;
        }

        public static bool IsDateTimeLike(object value)
        {
            return value is DateTime || value is DateTimeOffset;
        }

        public static bool IsEnum(object value)
        {
            return value is Enum;
        }

        public static bool IsPrimitive(object value)
        {
            return
                IsNumeric(value) ||
                IsLogical(value) ||
                IsStringLike(value) ||
                IsEnum(value) ||
                value == null;
        }

        public static bool IsIntegral(object value)
        {
            return IsSignedIntegral(value) || IsUnsignedIntegral(value);
        }

        public static bool IsSignedIntegral(object value)
        {
            return
                value is sbyte ||
                value is short ||
                value is int ||
                value is long;
        }

        public static bool IsUnsignedIntegral(object value)
        {
            return
                value is byte ||
                value is ushort ||
                value is uint ||
                value is ulong;
        }

        public static bool IsFloatingPointNumber(object value)
        {
            return value is float || value is double;
        }

        public static bool IsFixedPointNumber(object value)
        {
            return value is decimal;
        }

        public static bool IsCollection(object value)
        {
            return value is ICollection;
        }
    }
}