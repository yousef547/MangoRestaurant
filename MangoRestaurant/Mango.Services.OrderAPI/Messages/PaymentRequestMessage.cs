using Mango.MessageBus;

namespace Mango.Services.OrderAPI.Messages
{
    public class PaymentRequestMessage:BaseMessage
    {
        public int OrderId { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryMothYear { get; set; }
        public double OrderTotle { get; set; }
        public string Email { get; set; }
    }
}
