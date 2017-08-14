using System;

namespace CSUES.Common
{
    public class Version
    {
        public const int ImplementationVersion = 1;

        public Version(DateTime startDateTime, string experimentParametersHashString)
        {
            StartDateTime = startDateTime;
            ExperimentParametersHashString = experimentParametersHashString;
        }

        public DateTime StartDateTime { get; set; }
        public string ExperimentParametersHashString { get; set; }
    }
}
