using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace ApiMongoDB.Models
{
    [Collection("shipment")]
    public class Shipment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public DateTime ShipmentDate { get; set; }
    }
}
