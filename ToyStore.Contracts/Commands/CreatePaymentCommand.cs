using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyStore.Contracts.Common;

namespace ToyStore.Contracts.Commands
{
    public class CreatePaymentCommand: BaseMessage
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CustomerName { get; set; }
    }
}
