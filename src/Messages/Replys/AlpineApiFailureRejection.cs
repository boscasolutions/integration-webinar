namespace Messages.Replys
{
    public class AlpineApiFailureRejection
    {
        // 400 spectrum, authentication and permissions
        public string ResultMessage { get; set; }
        public string OrderId { get; set; }
    }
}