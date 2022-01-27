using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using NittyGritty.SourceGenerators.Annotations;
using NittyGritty.SourceGenerators.Helpers;
using NittyGritty.Uno.SourceGenerators.Receivers;

namespace NittyGritty.Uno.SourceGenerators
{
    [Generator]
    public class LocatorGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new LocatorContextReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                context.AnalyzerConfigOptions.GlobalOptions.IsTrue("build_property.NG_DevelopmentMode") &&
                context.AnalyzerConfigOptions.GlobalOptions.IsTrue("build_property.NG_DebugLocatorGenerator"))
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
            if (!(context.SyntaxContextReceiver is LocatorContextReceiver receiver))
            {
                return;
            }

            if (receiver.Locators.Count > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("NGL0001", "Only 1 class can be marked with LocatorAttribute", "Multiple classes '{0}' are marked with LocatorAttribute. Only 1 class can be marked with LocatorAttribute.", "NittyGritty", DiagnosticSeverity.Error, true),
                    receiver.Locators[0].Locations.FirstOrDefault(), receiver.Locators.Skip(1).Select(i => i.Locations.FirstOrDefault()), receiver.Locators.Select(i => i.Name)));
                return;
            }

            if (receiver.Locators.Count == 0 &&
                (receiver.Instances.Count > 0 || receiver.Pages.Count > 0 || receiver.Dialogs.Count > 0))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("NGL0002", "One class must be marked with LocatorAttribute", "No class is marked with LocatorAttribute.", "NittyGritty", DiagnosticSeverity.Error, true),
                    null));
                return;
            }

            var fileName = "Locator.g.cs";
            var codeWriter = new CodeWriter(typeof(LocatorGenerator));
            codeWriter.AppendLine("using System;");
            codeWriter.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            codeWriter.AppendLine("using NittyGritty.Services.Core;");
            codeWriter.AppendLine("using NittyGritty.Uno.Services;");
            codeWriter.AppendLine();
            var locator = receiver.Locators.FirstOrDefault();
            var locatorAttribute = locator.GetAttribute<LocatorAttribute>();

            using (codeWriter.BeginScope($"namespace {locator.ContainingNamespace.ToDisplayString()}"))
            {
                using (codeWriter.BeginScope($"public partial class {locator.Name}"))
                {
                    codeWriter.AppendLine("private readonly ServiceProvider provider;");
                    codeWriter.AppendLine();

                    using (codeWriter.BeginScope($"public {locator.Name}()"))
                    {
                        codeWriter.AppendLine("var services = new ServiceCollection();");
                        codeWriter.AppendLine();

                        if (receiver.Pages.Count > 0)
                        {
                            var lifetime = locatorAttribute.GetValue<InstanceLifetime>("NavigationServiceLifetime");
                            using (codeWriter.BeginScope($"services.Add{lifetime}<INavigationService>(isp =>"))
                            {
                                codeWriter.AppendLine("var navigationService = new NavigationService();");
                                foreach (var item in receiver.Pages.Distinct(SymbolEqualityComparer.Default).Cast<INamedTypeSymbol>())
                                {
                                    if (!item.IsDerivedFromType("Windows.UI.Xaml.Controls.Page"))
                                    {
                                        context.ReportDiagnostic(Diagnostic.Create(
                                            new DiagnosticDescriptor("NGL0003", "Class marked with PageAttribute must be derived from Windows.UI.Xaml.Controls.Page", "Class '{0}' does not derive from Windows.UI.Xaml.Controls.Page.", "NittyGritty", DiagnosticSeverity.Error, true),
                                            item.Locations.FirstOrDefault(), item.Name));
                                        continue;
                                    }

                                    var key = item.GetPageKey();
                                    if (key != null)
                                    {
                                        codeWriter.AppendLine($"navigationService.Configure(\"{key}\", typeof({item.ToDisplayString()}));");
                                    }
                                }
                                codeWriter.AppendLine("return navigationService;");
                            }
                            codeWriter.AppendLine(");");
                            codeWriter.AppendLine();
                        }

                        if (receiver.Dialogs.Count > 0)
                        {
                            using (codeWriter.BeginScope("services.AddSingleton<IDialogService>(isp =>"))
                            {
                                codeWriter.AppendLine("var dialogService = new DialogService();");
                                foreach (var item in receiver.Dialogs.Distinct(SymbolEqualityComparer.Default).Cast<INamedTypeSymbol>())
                                {
                                    if (!item.IsDerivedFromType("Windows.UI.Xaml.Controls.ContentDialog"))
                                    {
                                        context.ReportDiagnostic(Diagnostic.Create(
                                            new DiagnosticDescriptor("NGL0004", "Class marked with DialogAttribute must be derived from Windows.UI.Xaml.Controls.ContentDialog", "Class '{0}' does not derive from Windows.UI.Xaml.Controls.ContentDialog.", "NittyGritty", DiagnosticSeverity.Error, true),
                                            item.Locations.FirstOrDefault(), item.Name));
                                        continue;
                                    }

                                    var key = item.GetDialogKey();
                                    if (key != null)
                                    {
                                        codeWriter.AppendLine($"dialogService.Configure(\"{key}\", typeof({item.ToDisplayString()}));");
                                    }
                                }
                                codeWriter.AppendLine("return dialogService;");
                            }
                            codeWriter.AppendLine(");");
                            codeWriter.AppendLine();
                        }

                        foreach (var item in receiver.Instances)
                        {
                            var att = item.GetAttribute<InstanceAttribute>();
                            var parent = att.GetTypeValue("Parent");
                            if (parent != null && !item.IsDerivedFromType(parent) && !item.ImplementsInterface(parent))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(
                                    new DiagnosticDescriptor("NGL0005", "Class marked with InstanceAttribute must be derived from Parent property if Property is set", "Class '{0}' does not derive from '{1}'.", "NittyGritty", DiagnosticSeverity.Error, true),
                                    item.Locations.FirstOrDefault(), item.Name, parent));
                                continue;
                            }

                            var lifetime = att.GetValue<InstanceLifetime>("Lifetime");
                            if (parent != null)
                            {
                                codeWriter.AppendLine($"services.Add{lifetime}<{parent}, {item.ToDisplayString()}>();");
                            }
                            else
                            {
                                codeWriter.AppendLine($"services.Add{lifetime}<{item.ToDisplayString()}>();");
                            }
                        }
                        codeWriter.AppendLine();

                        codeWriter.AppendLine("Configure(services);");
                        codeWriter.AppendLine("provider = services.BuildServiceProvider();");
                    }
                    codeWriter.AppendLine();

                    codeWriter.AppendLine("partial void Configure(ServiceCollection services);");
                    codeWriter.AppendLine();

                    foreach (var item in receiver.Instances)
                    {
                        var instanceAttribute = item.GetAttribute<InstanceAttribute>();
                        var lifetime = instanceAttribute.GetValue<InstanceLifetime>("Lifetime");
                        if (lifetime == InstanceLifetime.Singleton)
                        {
                            var parent = instanceAttribute.GetTypeValue("Parent") ?? item.ToDisplayString();
                            var name = instanceAttribute.GetValue<string>("Name") ??
                                receiver.ViewModelKeys.FirstOrDefault(i => i.ToDisplayString() == item.ToDisplayString()).GetViewModelKey() ??
                                item.Name;
                            codeWriter.AppendLine($"public {parent} {name} => provider.GetService<{parent}>();");
                            codeWriter.AppendLine();
                        }
                    }
                }
            }

            context.AddSource(fileName, codeWriter.ToString());
        }

    }
}
