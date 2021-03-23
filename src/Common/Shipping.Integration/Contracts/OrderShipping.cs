using System;

namespace Shipping.Integration.Contracts
{
    public class OrderShipping
    {
        public OrderShipping()
        {
            CreatedAt = DateTime.Now;
        }

        public string OrderId { get; set; }

        public string TrackingNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedDateTimeUtc { get; set; }

        public string State { get; set; }

        public override string ToString()
        {
            return $"{OrderId}, {CreatedAt:dd/MM/yyyy HH:mm}.";
        }
    }
}