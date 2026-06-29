using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyStore.Contracts.Enums
{
    public enum OrderStatus
    {
        Pending,
        PaymentProcessing,
        Paid,
        PaymentFailed,
        Cancelled,
        InventoryReserved,
        Shipped,
        Completed
    }
}
