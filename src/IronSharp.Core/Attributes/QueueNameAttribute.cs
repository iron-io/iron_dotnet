using System;

namespace IronSharp.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class QueueNameAttribute : Attribute
    {
        public QueueNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public static string GetName<T>()
        {
            Type type = typeof(T);
            object[] customAttributes = type.GetCustomAttributes(typeof(QueueNameAttribute), true);
            return customAttributes.Length > 0 ? ((QueueNameAttribute) customAttributes[0]).Name : type.FullName;
        }
    }
}
