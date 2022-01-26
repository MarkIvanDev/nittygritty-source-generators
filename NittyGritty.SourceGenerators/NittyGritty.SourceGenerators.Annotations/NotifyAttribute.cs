using System;

namespace NittyGritty.SourceGenerators.Annotations
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class NotifyAttribute : Attribute
    {
        public string Name { get; set; }

        public AccessLevel Getter { get; set; }

        public AccessLevel Setter { get; set; }
    }

}
