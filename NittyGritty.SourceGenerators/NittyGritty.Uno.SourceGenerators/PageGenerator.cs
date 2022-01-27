using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NittyGritty.SourceGenerators.Annotations;
using NittyGritty.SourceGenerators.Helpers;
using NittyGritty.Uno.SourceGenerators.Receivers;

namespace NittyGritty.Uno.SourceGenerators
{
    [Generator]
    internal class PageGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new PageContextReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                context.AnalyzerConfigOptions.GlobalOptions.IsTrue("build_property.NG_DevelopmentMode") &&
                context.AnalyzerConfigOptions.GlobalOptions.IsTrue("build_property.NG_DebugPageGenerator"))
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
            if (!(context.SyntaxContextReceiver is PageContextReceiver receiver))
            {
                return;
            }

            foreach (var item in receiver.Pages.Distinct(SymbolEqualityComparer.Default).Cast<INamedTypeSymbol>())
            {
                if (!item.IsDerivedFromType("Windows.UI.Xaml.Controls.Page"))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor("NGPG001", "Class marked with PageAttribute must be derived from Windows.UI.Xaml.Controls.Page", "Class '{0}' does not derive from Windows.UI.Xaml.Controls.Page.", "NittyGritty", DiagnosticSeverity.Error, true),
                        item.Locations.FirstOrDefault(), item.Name));
                    continue;
                }

                var fileName = $"{item.ToDisplayString()}.g.cs";
                var codeWriter = new CodeWriter(typeof(PageGenerator));
                codeWriter.AppendLine("using System;");
                codeWriter.AppendLine();

                using (codeWriter.BeginScope($"namespace {item.ContainingNamespace.ToDisplayString()}"))
                {
                    using (codeWriter.BeginScope($"public partial class {item.Name}"))
                    {
                        var key = item.GetPageKey();
                        if (receiver.ViewModelKeys.TryGetValue(key, out var viewModel))
                        {
                            codeWriter.AppendLine($"public {viewModel.ToDisplayString()} ViewModel => DataContext as {viewModel.ToDisplayString()};");
                        }
                        else
                        {
                            var pageAttribute = item.GetAttribute<PageAttribute>();
                            var vm = pageAttribute.GetTypeValue("ViewModel");
                            if (vm != null)
                            {
                                codeWriter.AppendLine($"public {vm} ViewModel => DataContext as {vm};");
                            }
                        }
                    }
                }

                context.AddSource(fileName, codeWriter.ToString());
            }
        }

    }
}
