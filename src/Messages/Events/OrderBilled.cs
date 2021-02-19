namespace Messages.Events
{
    public class OrderBilled
    {
        public string CustomerId { get; set; }
        public string OrderId { get; set; }
        public decimal OrderValue { get; set; }
    }
}