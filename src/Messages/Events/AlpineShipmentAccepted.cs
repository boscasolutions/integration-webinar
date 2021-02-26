namespace Messages.Events
{
    public class AlpineShipmentAccepted
    {
        public string OrderId { get; set; }
        public string TrackingNumber { get; set; }
    }
}