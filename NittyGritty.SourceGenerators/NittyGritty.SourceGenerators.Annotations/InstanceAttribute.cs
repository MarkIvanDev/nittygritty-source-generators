using System;
using System.Collections.Generic;
using System.Text;

namespace NittyGritty.SourceGenerators.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InstanceAttribute : Attribute
    {

        public Type Parent { get; set; }

        public InstanceLifetime Lifetime { get; set; }

        public string Name { get; set; }
    }
}
