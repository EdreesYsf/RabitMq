using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Shared.Messaging.Configuration
{
    //exchange kinds
    public enum MessagingExchangeType
    {
        Direct,
        Topic,
        Fanout,
        Headers
    }
}
