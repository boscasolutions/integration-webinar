namespace Messages.Events
{
    public class OrderPlaced
    {
        public string CustomerId { get; set; }
        public string OrderId { get; set; }
    }
}