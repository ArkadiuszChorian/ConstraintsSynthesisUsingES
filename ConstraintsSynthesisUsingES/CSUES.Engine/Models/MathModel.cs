using System.Collections.Generic;
using CSUES.Engine.Benchmarks;
using CSUES.Engine.Models.Constraints;
using CSUES.Engine.Utils;

namespace CSUES.Engine.Models
{
    public class MathModel
    {
        public MathModel()
        {           
        }

        public MathModel(IList<Constraint> synthesizedModel, IBenchmark benchmark)
        {
            SynthesizedModel = synthesizedModel;
            ReferenceModel = benchmark.Constraints;
            Domains = benchmark.Domains;

            SynthesizedModelInLpFormat = SynthesizedModel.ToLpFormat(benchmark.Domains);
            SynthesizedModelInLpFormatSimplified = SynthesizedModel.ToLpFormatSimplified(benchmark.Domains);
            ReferenceModelInLpFormat = ReferenceModel.ToLpFormat(benchmark.Domains);
            ReferenceModelInLpFormatSimplified = ReferenceModel.ToLpFormatSimplified(benchmark.Domains);
        }

        public string SynthesizedModelInLpFormat { get; set; }
        public string SynthesizedModelInLpFormatSimplified { get; set; }
        public string ReferenceModelInLpFormat { get; set; }
        public string ReferenceModelInLpFormatSimplified { get; set; }
        public IList<Constraint> SynthesizedModel { get; set; }
        public IList<Constraint> ReferenceModel { get; set; }
        public IList<Domain> Domains { get; set; }
    }
}
