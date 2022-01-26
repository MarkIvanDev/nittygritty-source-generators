using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NittyGritty.SourceGenerators.Annotations;
using NittyGritty.SourceGenerators.Helpers;

namespace NittyGritty.Uno.SourceGenerators.Receivers
{
    internal class DialogContextReceiver : ISyntaxContextReceiver
    {
        public List<INamedTypeSymbol> Dialogs { get; } = new();

        public Dictionary<string, INamedTypeSymbol> DialogKeys { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax vm &&
                context.SemanticModel.GetDeclaredSymbol(vm) is INamedTypeSymbol symbol)
            {
                if (symbol.GetAttribute<DialogAttribute>() != null)
                {
                    Dialogs.Add(symbol);
                }
                if (symbol.GetAttribute<DialogKeyAttribute>() != null)
                {
                    DialogKeys.Add(symbol.GetDialogKey(), symbol);
                }
            }
        }
    }
}
