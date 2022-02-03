using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanArchitecture.Application.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToStringSequence<TModel>(this ICollection<TModel> list, Func<TModel, string> expression,
            string delimiter = ", ") => list != null && list.Any()
            ? list.Select(expression).Aggregate((current, next) => $"{current}{delimiter}{next}")
            : "";
    }
}