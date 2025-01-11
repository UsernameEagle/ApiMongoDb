namespace ApiMongoDB.Models
{
    public class OperationRequest
    {
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
