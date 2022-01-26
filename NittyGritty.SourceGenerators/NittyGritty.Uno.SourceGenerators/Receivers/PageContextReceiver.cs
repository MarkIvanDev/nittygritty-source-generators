using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NittyGritty.SourceGenerators.Annotations;
using NittyGritty.SourceGenerators.Helpers;

namespace NittyGritty.Uno.SourceGenerators.Receivers
{
    internal class PageContextReceiver : ISyntaxContextReceiver
    {
        public List<INamedTypeSymbol> Pages { get; } = new();

        public Dictionary<string, INamedTypeSymbol> ViewModelKeys { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax vm &&
                context.SemanticModel.GetDeclaredSymbol(vm) is INamedTypeSymbol symbol)
            {
                if (symbol.GetAttribute<PageAttribute>() != null)
                {
                    Pages.Add(symbol);
                }
                if (symbol.GetAttribute<ViewModelKeyAttribute>() != null)
                {
                    ViewModelKeys.Add(symbol.GetViewModelKey(), symbol);
                }
            }
        }
    }
}
