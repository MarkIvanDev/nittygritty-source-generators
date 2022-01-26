using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NittyGritty.SourceGenerators.Annotations;
using NittyGritty.SourceGenerators.Helpers;

namespace NittyGritty.SourceGenerators.Receivers
{
    internal class DialogKeyContextReceiver : ISyntaxContextReceiver
    {
        public List<INamedTypeSymbol> Classes { get; } = new List<INamedTypeSymbol>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax vm)
            {
                if (context.SemanticModel.GetDeclaredSymbol(vm) is INamedTypeSymbol vmSymbol &&
                    vmSymbol.GetAttribute<DialogKeyAttribute>() != null)
                {
                    Classes.Add(vmSymbol);
                }
            }
        }
    }
}
