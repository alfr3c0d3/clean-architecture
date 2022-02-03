using Serilog;
using System;

namespace CleanArchitecture.Application.Common.Extensions
{
    public static class NumericExtensions
    {
        public static int ToInteger(this string source) => source.ToNumericTypeOf<int>();

        public static double ToDouble(this string source) => source.ToNumericTypeOf<double>();

        public static decimal ToDecimal(this string source) => source.ToNumericTypeOf<decimal>();

        public static float ToFloat(this string source) => source.ToNumericTypeOf<float>();

        private static T ToNumericTypeOf<T>(this string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return default;

            try
            {
                var result = (T)Convert.ChangeType(source, typeof(T));
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return default;
            }
        }
    }
}