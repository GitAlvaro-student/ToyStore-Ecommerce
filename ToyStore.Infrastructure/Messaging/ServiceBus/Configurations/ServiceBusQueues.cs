using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyStore.Infrastructure.Messaging.ServiceBus.Configurations
{
    /// <summary>
    /// Centraliza os nomes das filas do Azure Service Bus.
    /// Evita strings literais espalhadas pelo código.
    /// Adicionar novas filas aqui conforme o sistema evoluir.
    /// </summary>
    public static class ServiceBusQueues
    {
        public const string PaymentQueue = "payment-queue";
    }
}
