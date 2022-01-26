using System;
using System.Collections.Generic;
using System.Text;

namespace NittyGritty.SourceGenerators.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DialogKeyAttribute : Attribute
    {
        public string Key { get; set; }
    }
}
