using System;
using System.Collections.Generic;
using System.Text;

namespace AcraUtils.Configuration
{
    public class RabbitMQConfiguration
    {
        public string Address { get; set; }
        public string CBVirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CBQueue { get; set; }
        public string CBExchange { get; set; }
        public string CBBinding { get; set; }
        public string CBA001Queue { get; set; }

        // Temporary
        public string CBP001Binding { get; set; }
        public string CBP001Queue { get; set; }
        public int ConcurrentConsumerCount { get; set; }
        public ushort ConcurrentChannelPrefetchCount { get; set; }
    }
}
