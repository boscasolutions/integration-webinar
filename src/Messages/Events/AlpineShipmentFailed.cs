namespace Messages.Events
{
    public class AlpineShipmentFailed
    {
        public string OrderId { get; set; }
        public string ResultMessage { get; set; }
    }
}