using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Shared.Messaging.Configuration
{
    public class MessagingOptions
    {
        public const string SectionName = "Messaging";

        /// <summary>
        /// الحد الأقصى لأولوية الرسائل في RabbitMQ (x-max-priority).
        /// القيمة الافتراضية 10. اجعلها null لتعطيل خاصية الأولوية على الطوابير.
        /// </summary>
        public byte? MaxPriority { get; set; } = 10;
    }
}
