using System;
using System.Collections.Generic;

namespace TestApi.Models
{
    public class OrderShipping
    {
        public string OrderId { get; set; }

        public string TrackingNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedDateTimeUtc { get; internal set; }
        public string State { get; internal set; }

        public OrderShipping()
        {
            CreatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{OrderId}, {CreatedAt:dd/MM/yyyy HH:mm}.";
        }

        internal bool IsValid(out IEnumerable<string> errors)
        {
            errors = new List<string>();
            return true;
        }
    }
}