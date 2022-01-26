using System;

namespace NittyGritty.SourceGenerators.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CommandAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
