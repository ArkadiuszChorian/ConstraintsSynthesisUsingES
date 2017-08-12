﻿using System.Collections.Generic;
using ES.Core.Benchmarks;
using ES.Core.Constraints;
using ES.Core.Utils;

namespace ES.Core.Models
{
    public class MathModel
    {
        public MathModel(IList<Constraint> synthesizedModel, IBenchmark benchmark)
        {
            SynthesizedModel = synthesizedModel;
            ReferenceModel = benchmark.Constraints;
            Domains = benchmark.Domains;

            SynthesizedModelInLpFormat = SynthesizedModel.ToLpFormat(benchmark.Domains);
            ReferenceModelInLpFormat = ReferenceModel.ToLpFormat(benchmark.Domains);
        }

        public string SynthesizedModelInLpFormat { get; set; }
        public string ReferenceModelInLpFormat { get; set; }
        public IList<Constraint> SynthesizedModel { get; set; }
        public IList<Constraint> ReferenceModel { get; set; }
        public IList<Domain> Domains { get; set; }
    }
}