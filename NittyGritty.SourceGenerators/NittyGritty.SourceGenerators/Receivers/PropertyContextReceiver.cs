using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NittyGritty.SourceGenerators.Annotations;
using NittyGritty.SourceGenerators.Helpers;

namespace NittyGritty.SourceGenerators.Receivers
{
    internal class PropertyContextReceiver : ISyntaxContextReceiver
    {
        public List<IFieldSymbol> Fields { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is FieldDeclarationSyntax field)
            {
                foreach (var item in field.Declaration.Variables)
                {
                    if (context.SemanticModel.GetDeclaredSymbol(item) is IFieldSymbol fieldSymbol &&
                        (fieldSymbol.GetAttribute<NotifyAttribute>() != null || fieldSymbol.GetAttribute<AlsoNotifyAttribute>() != null))
                    {
                        Fields.Add(fieldSymbol);
                    }
                }
            }
        }
    }
}
