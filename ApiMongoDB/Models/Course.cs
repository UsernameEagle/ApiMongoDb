using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ApiMongoDB.Models
{
    [Collection("courses")]
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } //= ObjectId.GenerateNewId().ToString();

        [Required(ErrorMessage = "Course name is required")]
        public required string Name { get; set; } = null!;

        [Required(ErrorMessage = "Course code is required")]
        public required string Code { get; set; } = null!;
    }
}
