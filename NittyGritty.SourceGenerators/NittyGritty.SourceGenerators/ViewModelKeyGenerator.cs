using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using NittyGritty.SourceGenerators.Helpers;
using NittyGritty.SourceGenerators.Receivers;

namespace NittyGritty.SourceGenerators
{
    [Generator]
    internal class ViewModelKeyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ViewModelKeyContextReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                context.AnalyzerConfigOptions.GlobalOptions.IsTrue("build_property.NG_DevelopmentMode") &&
                context.AnalyzerConfigOptions.GlobalOptions.IsTrue("build_property.NG_DebugViewModelKeyGenerator"))
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
            if (!(context.SyntaxContextReceiver is ViewModelKeyContextReceiver receiver))
            {
                return;
            }

            var fileName = "ViewModelKeys.g.cs";
            var codeWriter = new CodeWriter(typeof(ViewModelKeyGenerator));
            codeWriter.AppendLine("using System;");
            codeWriter.AppendLine();

            var vmNamespace = context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.NG_ViewModelKeysNamespace", out var ns) && !string.IsNullOrWhiteSpace(ns) ?
                ns : "NittyGritty.Generated";
            using (codeWriter.BeginScope($"namespace {vmNamespace}"))
            {
                using (codeWriter.BeginScope($"public static class ViewModelKeys"))
                {
                    foreach (var item in receiver.Classes)
                    {
                        var key = item.GetViewModelKey();
                        if (key != null)
                        {
                            codeWriter.AppendLine($"public static string {key} {{ get; }} = nameof({key});");
                        }
                    }
                }
            }

            context.AddSource(fileName, codeWriter.ToString());
        }

    }
}
