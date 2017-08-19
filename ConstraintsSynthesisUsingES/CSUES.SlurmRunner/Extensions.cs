using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CSUES.Engine.Models;

namespace CSUES.SlurmRunner
{
    public static class Extensions
    {
        public static string ToConsoleArgumentsString(this ExperimentParameters experimentParameters)
        {
            var sb = new StringBuilder();
            var propertyInfos = GetDbSerializableProperties(experimentParameters).ToList();

            foreach (var pi in propertyInfos)
                sb.Append($"{pi.Name}={pi.GetValue(experimentParameters, null)} ");

            return sb.ToString();
        }

        private static readonly Type[] SerializableTypes = {
            typeof(ValueType),
            typeof(string),
            typeof(Enum),
            typeof(TimeSpan)
        };

        private static IEnumerable<PropertyInfo> GetDbSerializableProperties<T>(T obj)
        {
            return obj.GetType().GetProperties().Where(pi => SerializableTypes.Contains(pi.PropertyType) || SerializableTypes.Contains(pi.PropertyType.BaseType));
        }
    }
}