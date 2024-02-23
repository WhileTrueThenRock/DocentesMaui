using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Models
{
    internal class MessageChatModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {  get; set; }
        public string UserId { get; set; }
        public object Content {  get; set; }
        public string Purpose { get; set; }
        public string TargetUserID { get; set; }
        public DateTime SendAt {  get; set; }
        public string Imagen { get; set; }
        public string Pdf { get; set; }

        public MessageChatModel() 
        {
            Id = ObjectId.GenerateNewId().ToString();
            SendAt = DateTime.Now;
        }

    }
}
