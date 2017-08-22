using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CSUES.Engine.Models;

namespace CSUES.WinApplication
{
    public static class Extensions
    {
        public static IList<Point> Reduce(this IList<Point> points, int maximumNumberOfPoints = 10000)
        {
            if (points.Count > maximumNumberOfPoints)
                points = points.Take(maximumNumberOfPoints).ToList();

            return points;
        }

        public static string GetPrintableString<T>(this T objectToPrint)
        {
            var sb = new StringBuilder();
            var propertyInfos = GetPrintableProperties(objectToPrint);

            foreach (var propertyInfo in propertyInfos)
            {
                var valueToPrint = propertyInfo.GetValue(objectToPrint, null);

                if (propertyInfo.PropertyType.IsEnum)
                {
                    var numericValue = propertyInfo.GetValue(objectToPrint, null);
                    var stringValue = Enum.GetName(propertyInfo.PropertyType, numericValue);

                    valueToPrint = stringValue;
                }

                if (propertyInfo.PropertyType == typeof(TimeSpan))
                {
                    var timeSpan = propertyInfo.GetValue(objectToPrint, null);

                    valueToPrint = ((TimeSpan)timeSpan).Milliseconds + " [ms]";
                }

                sb.AppendFormat("{0} = {1}\n", propertyInfo.Name, valueToPrint);
            }

            return sb.ToString();
        }

        public static readonly Type[] TypesToPrint = {
            typeof(bool).BaseType,
            typeof(int).BaseType,
            typeof(long).BaseType,
            typeof(double).BaseType,
            typeof(decimal).BaseType,
            typeof(float).BaseType,
            typeof(Enum),
            typeof(TimeSpan)
        };

        public static IEnumerable<PropertyInfo> GetPrintableProperties<T>(T obj)
        {
            return obj.GetType().GetProperties().Where(pi => TypesToPrint.Contains(pi.PropertyType.BaseType));
        }
    }
}