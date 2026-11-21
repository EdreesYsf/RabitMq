using System;

namespace Company.Shared.Messaging.Consumers;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class QueueNameAttribute : Attribute
{
    public string Name { get; }
    public byte? MaxPriority { get; }
    public QueueNameAttribute(string name, byte maxPriority)
    {
        Name = name;
        MaxPriority = maxPriority;
    }
    public QueueNameAttribute(string name)
    {
        Name = name;
        MaxPriority = null;
    }
}