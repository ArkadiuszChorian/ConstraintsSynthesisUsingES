using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CSUES.Engine.Models;
using ES.Core.Models;
using ExperimentDatabase;
using Statistics = CSUES.Engine.Models.Statistics;
using EvolutionStatistics = ES.Core.Models.Statistics;

namespace CSUES.Common
{
    public class DatabaseContext
    {
        private readonly Database _database;
        private readonly DatabaseEngine _databaseEngine;

        private readonly DataSet _experiments;
        private readonly DataSet _versions;
        private readonly DataSet _experimentParameters;
        private readonly DataSet _evolutionParameters;
        private readonly DataSet _mathModels;
        private readonly DataSet _statistics;       
        private readonly DataSet _errors;

        private static readonly string ExperimentsTableName = nameof(_experiments).Replace("_", string.Empty);
        private static readonly string VersionsTableName = nameof(_versions).Replace("_", string.Empty);
        private static readonly string ExperimentParametersTableName = nameof(_experimentParameters).Replace("_", string.Empty);
        private static readonly string EvolutionParametersTableName = nameof(_evolutionParameters).Replace("_", string.Empty);
        private static readonly string MathModelsTableName = nameof(_mathModels).Replace("_", string.Empty);
        private static readonly string StatisticsTableName = nameof(_statistics).Replace("_", string.Empty);
        private static readonly string ErrorsTableName = nameof(_errors).Replace("_", string.Empty);

        private bool _anyErrors;

        public const string DbFilename = "database1.db";

        public DatabaseContext(string dbFilePath)
        {
            _database = new Database(dbFilePath);
            _databaseEngine = new DatabaseEngine(dbFilePath);
            
            _experiments = _database.NewExperiment();
            _versions = _experiments.NewChildDataSet(VersionsTableName);
            _experimentParameters = _experiments.NewChildDataSet(ExperimentParametersTableName);
            _evolutionParameters = _experiments.NewChildDataSet(EvolutionParametersTableName);
            _mathModels = _experiments.NewChildDataSet(MathModelsTableName);
            _statistics = _experiments.NewChildDataSet(StatisticsTableName);
            _errors = _experiments.NewChildDataSet(ErrorsTableName);

            _anyErrors = false;
        }
        
        public void Insert(Version version)
        {
            _experiments.Add(nameof(Version.StartDateTime), version.StartDateTime);
            _experiments.Add(nameof(Version.ImplementationVersion), Version.ImplementationVersion);

            _versions.Add(nameof(Version.StartDateTime), version.StartDateTime);
            _versions.Add(nameof(Version.ImplementationVersion), Version.ImplementationVersion);
        }

        public void Insert(ExperimentParameters experimentParameters)
        {
            Insert(experimentParameters, _experiments, _experimentParameters);   
        }

        public void Insert(EvolutionParameters evolutionParameters)
        {
            Insert(evolutionParameters, _experiments, _evolutionParameters);
        }

        public void Insert(Statistics statistics)
        {
            Insert(statistics, _experiments, _statistics);
        }

        public void Insert(MathModel mathModel)
        {
            Insert(mathModel, _experiments, _mathModels);
            //_experiments.Add(nameof(MathModel.SynthesizedModelInLpFormat), mathModel.SynthesizedModelInLpFormat);
            //_experiments.Add(nameof(MathModel.ReferenceModelInLpFormat), mathModel.ReferenceModelInLpFormat);

            //_mathModels.Add(nameof(MathModel.SynthesizedModelInLpFormat), mathModel.SynthesizedModelInLpFormat);
            //_mathModels.Add(nameof(MathModel.ReferenceModelInLpFormat), mathModel.ReferenceModelInLpFormat);
        }

        public void Insert(Exception exception)
        {
            Insert(exception, _experiments, _errors);
            //_errors.Add(nameof(Exception.HResult), exception.HResult);
            //_errors.Add(nameof(Exception.Message), exception.Message);
            //_errors.Add(nameof(Exception.StackTrace), exception.StackTrace);

            _anyErrors = true;
        }

        public void Insert(string log)
        {
            _experiments.Add("EvolutionLog", log);
        }

        public bool ExperimentShouldBePrepared(ExperimentParameters experimentParameters)
        {
            if (HasTables(experimentParameters) == false)
                return true;

            if (IsSameVersion(experimentParameters) == false)
                return true;

            //var experimentParametersPredicates = GetPredicates(experimentParameters, ExperimentParametersTableName);
            //var evolutionParametersPredicates = GetPredicates(experimentParameters.EvolutionParameters, EvolutionParametersTableName);

            //var getIdQuery = $"SELECT {ExperimentParametersTableName}.parent FROM {ExperimentParametersTableName} INNER JOIN {EvolutionParametersTableName} " +
            //                 $"ON {ExperimentParametersTableName}.parent = {EvolutionParametersTableName}.parent " +
            //                 $"WHERE {experimentParametersPredicates} AND {evolutionParametersPredicates}";

            var experimentParametersPredicates = GetPredicates(experimentParameters, ExperimentsTableName);
            var evolutionParametersPredicates = GetPredicates(experimentParameters.EvolutionParameters, ExperimentsTableName);

            var getIdQuery = $"SELECT {ExperimentsTableName}.id FROM {ExperimentsTableName} " +
                             $"WHERE {experimentParametersPredicates} AND {evolutionParametersPredicates}";

            var id = _databaseEngine.PrepareStatement(getIdQuery).ExecuteScalar();

            if (id == null)
                return true;

            //var reader = _databaseEngine.PrepareStatement(getIdQuery).ExecuteReader();

            //if (!reader.HasRows)
            //    return true;

            try
            {
                //var errorQuery = $"SELECT {ErrorsTableName}.{nameof(Exception.HResult)} FROM {ErrorsTableName} WHERE {ErrorsTableName}.parent = '{id}'";
                var errorQuery = $"SELECT {ExperimentsTableName}.{nameof(Exception.HResult)} FROM {ExperimentsTableName}";
                var result = _databaseEngine.PrepareStatement(errorQuery).ExecuteScalar();

                if (result == null)
                    return false; 

                var hasError = string.IsNullOrEmpty(result.ToString()) == false;

                return hasError;
            }
            catch (Exception exception)
            {
                return !exception.Message.Contains("no such column");
            }           
        }

        public bool HasTables(ExperimentParameters experimentParameters)
        {
            var controlQuery = $"SELECT name FROM sqlite_master WHERE name = '{VersionsTableName}'";
            var result = _databaseEngine.PrepareStatement(controlQuery).ExecuteReader();

            return result.HasRows;
        }

        public bool IsSameVersion(ExperimentParameters experimentParameters)
        {
            var implementationQuery = $"SELECT * FROM {VersionsTableName} " +
                $"WHERE {nameof(Version.ImplementationVersion)} = '{Version.ImplementationVersion}'";
            var result = _databaseEngine.PrepareStatement(implementationQuery).ExecuteReader();

            return result.HasRows;
        }

        public bool ExistsOld(ExperimentParameters experimentParameters)
        {
            var controlQuery = $"SELECT name FROM sqlite_master WHERE name = '{VersionsTableName}'";

            var result = _databaseEngine.PrepareStatement(controlQuery).ExecuteReader();

            if (!result.HasRows) return false;
            
            var implementationQuery = $"SELECT * FROM {VersionsTableName} " +
                $"WHERE {nameof(Version.ImplementationVersion)} = '{Version.ImplementationVersion}'";

            result = _databaseEngine.PrepareStatement(implementationQuery).ExecuteReader();

            if (!result.HasRows) return false;   
                    
            return Exists(experimentParameters.EvolutionParameters, EvolutionParametersTableName) 
                && Exists(experimentParameters, ExperimentParametersTableName);
        }

        private bool Exists<T>(T obj, string tableName)
        {            
            var predicates = GetPredicates(obj, tableName);
            var query = $"SELECT * FROM {tableName} WHERE {predicates}";
            
            var result = _databaseEngine.PrepareStatement(query).ExecuteReader();

            return result.HasRows;
        }

        private static string GetPredicates<T>(T obj, string tableName)
        {
            var predicates = new StringBuilder();
            var propertyInfos = GetDbSerializableProperties(obj).ToList();

            for (var i = 0; i < propertyInfos.Count; i++)
            {
                var pi = propertyInfos[i];

                predicates.Append($"{tableName}.{pi.Name} = '{pi.GetValue(obj, null)}'");

                if (i != propertyInfos.Count - 1)
                    predicates.Append(" AND ");
            }

            return predicates.ToString();
        }

        public void Save()
        {
            _experiments.Save();
            //_versions.Save();
            //_experimentParameters.Save();
            //_mathModels.Save();
            //_statistics.Save();

            //if (!_anyErrors) return;

            //_errors.Save();
            //_anyErrors = false;
        }

        public void Dispose()
        {
            _databaseEngine.Dispose();
            _versions.Dispose();
            _experimentParameters.Dispose();
            _mathModels.Dispose();
            _statistics.Dispose();
            _errors.Dispose();
            _experiments.Dispose();
            _database.Dispose();            
        }        

        private static void Insert<T>(T objectToInsert, params DataSet[] dataSetsToInsertIn)
        {
            var propertyInfos = GetDbSerializableProperties(objectToInsert);

            foreach (var propertyInfo in propertyInfos)
            {
                var valueToInsert = propertyInfo.GetValue(objectToInsert, null)?.ToString();

                if (propertyInfo.PropertyType == typeof(EvolutionParameters))
                {
                    var obj = propertyInfo.GetValue(objectToInsert, null);
                    Insert(obj, dataSetsToInsertIn);
                    continue;                    
                }

                if (propertyInfo.PropertyType == typeof(EvolutionStatistics))
                {
                    var obj = propertyInfo.GetValue(objectToInsert, null);
                    Insert(obj, dataSetsToInsertIn);
                    continue;
                }

                if (propertyInfo.PropertyType.IsEnum)
                {
                    var numericValue = propertyInfo.GetValue(objectToInsert, null);
                    var stringValue = Enum.GetName(propertyInfo.PropertyType, numericValue);

                    valueToInsert = stringValue;
                }

                if (propertyInfo.PropertyType == typeof(TimeSpan))
                {
                    var timeSpan = propertyInfo.GetValue(objectToInsert, null);

                    valueToInsert = ((TimeSpan) timeSpan).Milliseconds.ToString();
                }

                foreach (var dataSet in dataSetsToInsertIn)
                {
                    try
                    {
                        dataSet.Add(propertyInfo.Name, valueToInsert);
                    }
                    catch (ArgumentException)
                    {
                    }                  
                }                                                
            }
        }

        private static readonly Type[] SerializableTypes = {
            typeof(ValueType),
            typeof(string),           
            typeof(Enum),
            typeof(TimeSpan),
            typeof(EvolutionStatistics)
        };

        private static IEnumerable<PropertyInfo> GetDbSerializableProperties<T>(T obj)
        {
            return obj.GetType().GetProperties().Where(pi => SerializableTypes.Contains(pi.PropertyType) || SerializableTypes.Contains(pi.PropertyType.BaseType));
        }
    }
}

