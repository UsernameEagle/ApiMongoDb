using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiMongoDB.Models
{
    [Collection("students")]
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public List<string> Courses { get; set; } = new List<string>();

        [BsonIgnore]
        //[JsonIgnore]
        public List<Course>? CourseList { get; set; } = new List<Course>();
    }
}
