using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Coupon.API.Infrastructure.Models
{
    public class Coupon
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int Discount { get; set; }

        public string Code { get; set; }

        public bool Consumed { get; set; }

        public int OrderId { get; set; }
    }
}
