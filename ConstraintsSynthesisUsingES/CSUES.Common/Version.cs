using System;

namespace CSUES.Common
{
    public class Version
    {
        public const int ImplementationVersion = 1;

        public Version(DateTime startDateTime)
        {
            StartDateTime = startDateTime;
        }

        public DateTime StartDateTime { get; set; }
    }
}
