using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using NittyGritty.SourceGenerators.Annotations;
using NittyGritty.SourceGenerators.Helpers;
using NittyGritty.Uno.SourceGenerators.Receivers;

namespace NittyGritty.Uno.SourceGenerators
{
    [Generator]
    internal class DialogGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new DialogContextReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                context.AnalyzerConfigOptions.GlobalOptions.IsTrue("build_property.NG_DevelopmentMode") &&
                context.AnalyzerConfigOptions.GlobalOptions.IsTrue("build_property.NG_DebugDialogGenerator"))
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
            if (!(context.SyntaxContextReceiver is DialogContextReceiver receiver))
            {
                return;
            }

            foreach (var item in receiver.Dialogs.Distinct(SymbolEqualityComparer.Default).Cast<INamedTypeSymbol>())
            {
                if (!item.IsDerivedFromType("Windows.UI.Xaml.Controls.ContentDialog"))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor("NGDG001", "Class marked with DialogAttribute must be derived from Windows.UI.Xaml.Controls.ContentDialog", "Class '{0}' does not derive from Windows.UI.Xaml.Controls.ContentDialog.", "NittyGritty", DiagnosticSeverity.Error, true),
                        item.Locations.FirstOrDefault(), item.Name));
                    continue;
                }

                var fileName = $"{item.ToDisplayString()}.g.cs";
                var codeWriter = new CodeWriter(typeof(DialogGenerator));
                codeWriter.AppendLine("using System;");
                codeWriter.AppendLine();

                using (codeWriter.BeginScope($"namespace {item.ContainingNamespace.ToDisplayString()}"))
                {
                    using (codeWriter.BeginScope($"public partial class {item.Name}"))
                    {
                        var key = item.GetDialogKey();
                        if (receiver.DialogKeys.TryGetValue(key, out var viewModel))
                        {
                            codeWriter.AppendLine($"public {viewModel.ToDisplayString()} ViewModel => DataContext as {viewModel.ToDisplayString()};");
                        }
                        else
                        {
                            var dialogAttribute = item.GetAttribute<DialogAttribute>();
                            var vm = dialogAttribute.GetTypeValue("ViewModel");
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
