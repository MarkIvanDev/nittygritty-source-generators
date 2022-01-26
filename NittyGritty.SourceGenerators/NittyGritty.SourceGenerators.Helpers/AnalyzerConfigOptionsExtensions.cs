using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NittyGritty.SourceGenerators.Helpers
{
    public static class AnalyzerConfigOptionsExtensions
    {
        public static bool IsTrue(this AnalyzerConfigOptions options, string property)
        {
            return options.TryGetValue(property, out var value) &&
                bool.TryParse(value, out var result) &&
                result;
        }
    }
}
