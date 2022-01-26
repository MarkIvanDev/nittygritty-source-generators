using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using NittyGritty.SourceGenerators.Annotations;

namespace NittyGritty.SourceGenerators.Helpers
{
    public static class KeyHelpers
    {
        public static string GetViewModelKey(this INamedTypeSymbol symbol)
        {
            var vmKeyAttribute = symbol?.GetAttribute<ViewModelKeyAttribute>();
            if (vmKeyAttribute != null)
            {
                var key = vmKeyAttribute.GetValue<string>("Key");
                return key ?? symbol.Name.TrimEnd("ViewModel");
            }
            else
            {
                return null;
            }
        }

        public static string GetPageKey(this INamedTypeSymbol symbol)
        {
            var pageAttribute = symbol?.GetAttribute<PageAttribute>();
            if (pageAttribute != null)
            {
                var key = pageAttribute.GetValue<string>("Key");
                return key ?? symbol.Name.TrimEnd("Page", "View");
            }
            else
            {
                return null;
            }
        }

        public static string GetDialogKey(this INamedTypeSymbol symbol)
        {
            var dialogAttribute = symbol?.GetAttribute<DialogAttribute>();
            if (dialogAttribute != null)
            {
                var key = dialogAttribute.GetValue<string>("Key");
                return key ?? symbol.Name.TrimEnd("Dialog", "View");
            }
            else
            {
                return null;
            }
        }

        public static string TrimEnd(this string name, params string[] endings)
        {
            if (name is null) return name;

            foreach (var item in endings ?? Array.Empty<string>())
            {
                if (name.EndsWith(item, StringComparison.OrdinalIgnoreCase))
                {
                    return name.Substring(0, name.Length - item.Length);
                }
            }
            return name;
        }

    }
}
