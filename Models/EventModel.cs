using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Models
{
    class EventModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public TimeSpan StartAt { get; set; }
        public TimeSpan EndAt { get; set; }

        public EventModel()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
    }
}
