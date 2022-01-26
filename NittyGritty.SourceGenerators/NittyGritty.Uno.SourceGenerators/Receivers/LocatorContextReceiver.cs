using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NittyGritty.SourceGenerators.Annotations;
using NittyGritty.SourceGenerators.Helpers;

namespace NittyGritty.Uno.SourceGenerators.Receivers
{
    internal class LocatorContextReceiver : ISyntaxContextReceiver
    {
        public List<INamedTypeSymbol> ViewModelKeys { get; } = new();

        public List<INamedTypeSymbol> Locators { get; } = new();

        public List<INamedTypeSymbol> Instances { get; } = new();

        public List<INamedTypeSymbol> Pages { get; } = new();

        public List<INamedTypeSymbol> Dialogs { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax vm &&
                context.SemanticModel.GetDeclaredSymbol(vm) is INamedTypeSymbol symbol)
            {
                if (symbol.GetAttribute<ViewModelKeyAttribute>() != null)
                {
                    ViewModelKeys.Add(symbol);
                }
                if (symbol.GetAttribute<LocatorAttribute>() != null)
                {
                    Locators.Add(symbol);
                }
                if (symbol.GetAttribute<InstanceAttribute>() != null)
                {
                    Instances.Add(symbol);
                }
                if (symbol.GetAttribute<PageAttribute>() != null)
                {
                    Pages.Add(symbol);
                }
                if (symbol.GetAttribute<DialogAttribute>() != null)
                {
                    Dialogs.Add(symbol);
                }
            }
        }
    }
}
