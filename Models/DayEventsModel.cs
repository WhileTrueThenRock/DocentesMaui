using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Models
{
    class DayEventsModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string EventDate { get; set; }
        public ObservableCollection<EventModel> Events { get; set; } //mismo nombre que en la api

        public DayEventsModel()
        {
            Id = ObjectId.GenerateNewId().ToString();
            Events = new ObservableCollection<EventModel>();
            EventDate = DateTime.Now.ToString();
        }
    }
}
