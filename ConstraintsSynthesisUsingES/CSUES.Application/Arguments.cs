using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using CSUES.Engine.Models;
using ES.Core.Models;

namespace ES.Core.Utils
{
    public class Arguments
    {
        private static readonly Regex ParseExpression = new Regex(@"(?'name'[a-z][a-z0-9._]*)=(?'value'[a-z0-9._/\\\-]*|""[a-z0-9._/\\\- ]*"")", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        private static readonly Lazy<Arguments> LazyInstance = new Lazy<Arguments>(() => new Arguments(string.Join(" ", Environment.GetCommandLineArgs())), LazyThreadSafetyMode.PublicationOnly);
        private static readonly Lazy<string> LazyBaseDirectory = new Lazy<string>(() => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), LazyThreadSafetyMode.PublicationOnly);

        public static Arguments Instance => LazyInstance.Value;
        public static string BaseDirectory => LazyBaseDirectory.Value;

        private readonly IDictionary<string, string> _dictionary;
        
        private Arguments(string argument)
        {
            _dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (Match match in ParseExpression.Matches(argument))
            {
                Debug.Assert(match.Success);
                var name = match.Groups["name"].Value;
                var value = match.Groups["value"].Value;
                _dictionary[name] = value;
            }
        }

        public static bool HasAnyKeys()
        {
            return Instance._dictionary.Any();
        }

        public static bool HasKey(string key)
        {
            return Instance._dictionary.ContainsKey(key);
        }

        [DebuggerHidden]
        public static T Get<T>(string key) where T : struct
        {
            try
            {
                return (T)Convert.ChangeType(Instance._dictionary[key], typeof(T));
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("Key: " + key);
            }
        }

        public static T Get<T>(string key, T @default) where T : struct
        {
            string value;
            if (Instance._dictionary.TryGetValue(key, out value))
            {
                @default = (T)Convert.ChangeType(value, typeof(T));
            }
            return @default;
        }

        [DebuggerHidden]
        public static string Get(string key)
        {
            try
            {
                return Instance._dictionary[key];
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("Key: " + key);
            }
        }

        public static string Get(string key, string @default)
        {
            string value;
            return Instance._dictionary.TryGetValue(key, out value) ? value : @default;
        }

        public static T GetObject<T>(string key) where T : class
        {
            try
            {
                var name = Instance._dictionary[key];
                Type type = null;

                // FIXME: .NET Framework solution (correct):
                // var assemblies = AppDomain.CurrentDomain.GetAssemblies()

                // .NET Standard 1.5 solution (not all assemblies are searched):

                var assemblies = new HashSet<Assembly>();
                var entry = Assembly.GetEntryAssembly();
                var _this = typeof(Arguments).GetTypeInfo().Assembly;

                assemblies.Add(entry);
                assemblies.Add(_this);

                CollectAssemblies(assemblies, entry);


                foreach (var assembly in assemblies)
                {
                    type = assembly.GetType(name, false, true);
                    if (type != null)
                        break;
                }

                if (type == null)
                    throw new TypeLoadException($"Type {name} not found");

                var ctor = type.GetTypeInfo().GetConstructor(new Type[0]);
                return (T)ctor.Invoke(new object[0]);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("Key: " + key);
            }
        }

        private static void CollectAssemblies(ISet<Assembly> to, params Assembly[] assembliesToSearch)
        {
            foreach (var assembly in assembliesToSearch)
            {
                to.Add(assembly);
                foreach (var refAssembly in assembly.GetReferencedAssemblies())
                {
                    try
                    {
                        to.Add(Assembly.Load(refAssembly));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        public static T GetObject<T>(string key, T @default) where T : class
        {
            try
            {
                return GetObject<T>(key);
            }
            catch (KeyNotFoundException)
            {
                return @default;
            }
        }       

        public string this[string key] => Get(key);

        [SuppressMessage("ReSharper", "ArgumentsStyleOther")]
        [SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
        [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
        public static ExperimentParameters GetExperimentParameters()
        {
            var numberOfDimensions = Arguments.Get<int>(nameof(ExperimentParameters.NumberOfDimensions));
            var numberOfConstraints = Arguments.Get<int>(nameof(ExperimentParameters.MaximumNumberOfConstraints));

            return new ExperimentParameters(
                numberOfDimensions: numberOfDimensions,

                maximumNumberOfConstraints: numberOfConstraints,

                seed: Arguments.Get(nameof(ExperimentParameters.Seed), CSUES.Engine.Utils.Defaults.Seed),

                domainSamplingStep: Arguments.Get(nameof(ExperimentParameters.DomainSamplingStep), CSUES.Engine.Utils.Defaults.DomainSamplingStep),

                ballnBoundaryValue: Arguments.Get(nameof(ExperimentParameters.BallnBoundaryValue), CSUES.Engine.Utils.Defaults.BallnBoundaryValue),
                cubenBoundaryValue: Arguments.Get(nameof(ExperimentParameters.CubenBoundaryValue), CSUES.Engine.Utils.Defaults.CubenBoundaryValue),
                simplexnBoundaryValue: Arguments.Get(nameof(ExperimentParameters.SimplexnBoundaryValue), CSUES.Engine.Utils.Defaults.SimplexnBoundaryValue),

                globalLerningRate: Arguments.Get(nameof(ExperimentParameters.GlobalLearningRate), CSUES.Engine.Utils.Defaults.GlobalLerningRate(numberOfDimensions)),
                individualLearningRate: Arguments.Get(nameof(ExperimentParameters.IndividualLearningRate), CSUES.Engine.Utils.Defaults.IndividualLearningRate(numberOfDimensions)),
                stepThreshold: Arguments.Get(nameof(ExperimentParameters.StepThreshold), CSUES.Engine.Utils.Defaults.StepThreshold),
                rotationAngle: Arguments.Get(nameof(ExperimentParameters.RotationAngle), CSUES.Engine.Utils.Defaults.RotationAngle),
                typeOfMutation: Arguments.Get(nameof(ExperimentParameters.TypeOfMutation), CSUES.Engine.Utils.Defaults.TypeOfMutation),

                numberOfParentsSolutionsToSelect: Arguments.Get(nameof(ExperimentParameters.NumberOfParentsSolutionsToSelect), CSUES.Engine.Utils.Defaults.NumberOfParentsSolutionsToSelect),
                partOfSurvivorsSolutionsToSelect: Arguments.Get(nameof(ExperimentParameters.PartOfSurvivorsSolutionsToSelect), CSUES.Engine.Utils.Defaults.PartOfSurvivorsSolutionsToSelect),
                typeOfSurvivorsSelection: Arguments.Get(nameof(ExperimentParameters.TypeOfSurvivorsSelection), CSUES.Engine.Utils.Defaults.TypeOfSurvivorsSelection),

                numberOfPositivePoints: Arguments.Get(nameof(ExperimentParameters.NumberOfPositivePoints), CSUES.Engine.Utils.Defaults.NumberOfPositivePoints),
                numberOfNegativePoints: Arguments.Get(nameof(ExperimentParameters.NumberOfNegativePoints), CSUES.Engine.Utils.Defaults.NumberOfNegativePoints),
                defaultDomainLowerLimit: Arguments.Get(nameof(ExperimentParameters.DefaultDomainLowerLimit), CSUES.Engine.Utils.Defaults.DefaultDomainLowerLimit),
                defaultDomainUpperLimit: Arguments.Get(nameof(ExperimentParameters.DefaultDomainUpperLimit), CSUES.Engine.Utils.Defaults.DefaultDomainUpperLimit),
                usePointsGeneration: Arguments.Get(nameof(ExperimentParameters.UsePointsGeneration), CSUES.Engine.Utils.Defaults.UsePointsGeneration),
                maxNumberOfPointsInSingleArray: Arguments.Get(nameof(ExperimentParameters.MaxNumberOfPointsInSingleArray), CSUES.Engine.Utils.Defaults.MaxNumberOfPointsInSingleArray),

                basePopulationSize: Arguments.Get(nameof(ExperimentParameters.BasePopulationSize), CSUES.Engine.Utils.Defaults.BasePopulationSize),
                offspringPopulationSize: Arguments.Get(nameof(ExperimentParameters.OffspringPopulationSize), CSUES.Engine.Utils.Defaults.OffspringPopulationSize),
                numberOfGenerations: Arguments.Get(nameof(ExperimentParameters.NumberOfGenerations), CSUES.Engine.Utils.Defaults.NumberOfGenerations),
                oneFifthRuleCheckInterval: Arguments.Get(nameof(ExperimentParameters.OneFifthRuleCheckInterval), CSUES.Engine.Utils.Defaults.OneFifthRuleCheckInterval),
                oneFifthRuleScalingFactor: Arguments.Get(nameof(ExperimentParameters.OneFifthRuleScalingFactor), CSUES.Engine.Utils.Defaults.OneFifthRuleScalingFactor),

                useRecombination: Arguments.Get(nameof(ExperimentParameters.UseRecombination), CSUES.Engine.Utils.Defaults.UseRecombination),
                typeOfObjectsRecombiner: Arguments.Get(nameof(ExperimentParameters.TypeOfObjectsRecombiner), CSUES.Engine.Utils.Defaults.TypeOfObjectsRecombiner),
                typeOfStdDeviationsRecombiner: Arguments.Get(nameof(ExperimentParameters.TypeOfStdDeviationsRecombiner), CSUES.Engine.Utils.Defaults.TypeOfStdDeviationsRecombiner),
                typeOfRotationsRecombiner: Arguments.Get(nameof(ExperimentParameters.TypeOfRotationsRecombiner), CSUES.Engine.Utils.Defaults.TypeOfRotationsRecombiner),
                partOfPopulationToRecombine: Arguments.Get(nameof(ExperimentParameters.PartOfPopulationToRecombine), CSUES.Engine.Utils.Defaults.PartOfPopulationToRecombine),

                typeOfBenchmark: Arguments.Get(nameof(ExperimentParameters.TypeOfBenchmark), CSUES.Engine.Utils.Defaults.TypeOfBenchmark),
                constraintsForGenericBenchmark: CSUES.Engine.Utils.Defaults.ConstraintsToPointsGeneration);
        }
    }
}
