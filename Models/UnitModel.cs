using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Models
{
    class UnitModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Description { get; set; }
        public ObservableCollection<string> Pdfs { get; set; }
        public ObservableCollection<string> Images { get; set; }
        public ObservableCollection<string> Resources { get; set; }
        public ObservableCollection<ResourceModel> WebResources { get; set; }

        public UnitModel() {
            Id = ObjectId.GenerateNewId().ToString();
            Pdfs = new ObservableCollection<string>();
            Images = new ObservableCollection<string>();
            Resources = new ObservableCollection<string>();
            WebResources = new ObservableCollection<ResourceModel>();
        }
    }
}
