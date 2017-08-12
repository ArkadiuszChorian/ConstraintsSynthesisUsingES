using System;
using System.Linq;
using ES.Core.Models;
using ExperimentDatabase;
using Version = ES.Core.Models.Version;

namespace ES.Core.Utils
{
    public class DatabaseContext
    {
        private readonly Database _database;
        private readonly DatabaseEngine _databaseEngine;

        private readonly DataSet _experiments;
        private readonly DataSet _versions;
        private readonly DataSet _experimentParameters;
        private readonly DataSet _mathModels;
        private readonly DataSet _statistics;       
        private readonly DataSet _errors;
              
        private bool _anyErrors;

        public DatabaseContext(string dbFilePath)
        {
            _database = new Database(dbFilePath);
            _databaseEngine = new DatabaseEngine(dbFilePath);
            
            _experiments = _database.NewExperiment();
            _versions = _experiments.NewChildDataSet(nameof(_versions).Replace("_", string.Empty));
            _experimentParameters = _experiments.NewChildDataSet(nameof(_experimentParameters).Replace("_", string.Empty));
            _mathModels = _experiments.NewChildDataSet(nameof(_mathModels).Replace("_", string.Empty));
            _statistics = _experiments.NewChildDataSet(nameof(_statistics).Replace("_", string.Empty));
            _errors = _experiments.NewChildDataSet(nameof(_errors).Replace("_", string.Empty));

            _anyErrors = false;
        }
        
        public void Insert(Version version)
        {
            _experiments.Add(nameof(Version.StartDateTime), version.StartDateTime);
            _experiments.Add(nameof(Version.ImplementationVersion), Version.ImplementationVersion);
            _experiments.Add(nameof(Version.ExperimentParametersHashString), version.ExperimentParametersHashString);

            _versions.Add(nameof(Version.StartDateTime), version.StartDateTime);
            _versions.Add(nameof(Version.ImplementationVersion), Version.ImplementationVersion);
            _versions.Add(nameof(Version.ExperimentParametersHashString), version.ExperimentParametersHashString);
        }

        public void Insert(ExperimentParameters experimentParameters)
        {
            Insert(experimentParameters, _experiments, _experimentParameters);   
        }

        public void Insert(Statistics statistics)
        {
            Insert(statistics, _experiments, _statistics);
        }

        public void Insert(MathModel mathModel)
        {
            _experiments.Add(nameof(MathModel.SynthesizedModelInLpFormat), mathModel.SynthesizedModelInLpFormat);
            _experiments.Add(nameof(MathModel.ReferenceModelInLpFormat), mathModel.ReferenceModelInLpFormat);

            _mathModels.Add(nameof(MathModel.SynthesizedModelInLpFormat), mathModel.SynthesizedModelInLpFormat);
            _mathModels.Add(nameof(MathModel.ReferenceModelInLpFormat), mathModel.ReferenceModelInLpFormat);
        }

        public void Insert(Exception exception)
        {
            _errors.Add(nameof(Exception.HResult), exception.HResult);
            _errors.Add(nameof(Exception.Message), exception.Message);
            _errors.Add(nameof(Exception.StackTrace), exception.StackTrace);

            _anyErrors = true;
        }

        public bool Exists(ExperimentParameters experimentParameters)
        {
            var controlQuery = $"SELECT name FROM sqlite_master WHERE name = '{nameof(_versions).Replace("_", string.Empty)}'";
            
            var result = _databaseEngine.PrepareStatement(controlQuery).ExecuteReader();

            if (!result.HasRows) return false;

            var query = $"SELECT * FROM {nameof(_versions).Replace("_", string.Empty)} " +
                $"WHERE {nameof(Version.ExperimentParametersHashString)} = '{experimentParameters.GetHashString()}'";

            result = _databaseEngine.PrepareStatement(query).ExecuteReader();

            return result.HasRows;
        }

        public void Save()
        {
            _experiments.Save(false);
            _versions.Save();
            _experimentParameters.Save();
            _mathModels.Save();
            _statistics.Save();

            if (!_anyErrors) return;

            _errors.Save();
            _anyErrors = false;
        }

        public void Dispose()
        {
            _experiments.Dispose();
            _versions.Dispose();
            _experimentParameters.Dispose();
            _mathModels.Dispose();
            _statistics.Dispose();
            _errors.Dispose();
            _database.Dispose();
        }        

        private void Insert<T>(T objectToInsert, params DataSet[] dataSetsToInsertIn)
        {
            var propertyInfos = objectToInsert.GetDbSerializableProperties();

            foreach (var propertyInfo in propertyInfos)
            {
                var valueToInsert = propertyInfo.GetValue(objectToInsert, null);

                if (propertyInfo.PropertyType.IsEnum)
                {
                    var numericValue = propertyInfo.GetValue(objectToInsert, null);
                    var stringValue = Enum.GetName(propertyInfo.PropertyType, numericValue);

                    valueToInsert = stringValue;
                }

                if (propertyInfo.PropertyType == typeof(TimeSpan))
                {
                    var timeSpan = propertyInfo.GetValue(objectToInsert, null);

                    valueToInsert = ((TimeSpan) timeSpan).Milliseconds;
                }

                if (dataSetsToInsertIn.Any())
                {
                    foreach (var dataSet in dataSetsToInsertIn)
                    {
                        dataSet.Add(propertyInfo.Name, valueToInsert);
                    }
                }
                else
                {
                    _experiments.Add(propertyInfo.Name, valueToInsert);
                }                       
            }
        }

        public static readonly Type[] SerializableTypes = {
            typeof(bool).BaseType,
            typeof(int).BaseType,
            typeof(long).BaseType,
            typeof(double).BaseType,
            typeof(decimal).BaseType,
            typeof(float).BaseType,
            typeof(Enum),
            typeof(TimeSpan)
        };
    }
}

