using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyStore.Contracts.Common
{
    public class BaseMessage
    {
        public Guid MessageId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CorrelationId { get; set; }
    }
}
