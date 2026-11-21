using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Shared.Messaging.Publishers
{
    public enum MessagePriority : byte
    {
        Low = 1,  
        Normal = 3,
        Medium = 5,
        High = 8,
        Critical = 10
    }
}
