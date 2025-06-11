using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using R.Tools.Requests.Contracts;
using R.Tools.Requests.Entities;

namespace R.Tools.Requests
{
    internal class MongoRequestStorage : IRequestStorage
    {
        private readonly ILogger<MongoRequestStorage> log;
        private IMongoClient client;
        private IMongoDatabase db;

        public MongoRequestStorage(ILogger<MongoRequestStorage> log, StorageOptions options)
        {
            this.log = log;

            var mongoUrl = MongoUrl.Create(options.ConnectionString);
            client = new MongoClient(mongoUrl);
            db = client.GetDatabase(mongoUrl.DatabaseName);

            BsonClassMap.RegisterClassMap<RequestData>(m =>
            {
                m.AutoMap();
                m.UnmapMember(o => o.ContentObj);
            });
        }


        public void Send(RequestData data)
        {
            var key = Guid.NewGuid();
            log.LogInformation($"Start saving {key}");
            var requestBson = data.ToBsonDocument();
            if (data.ContentObj != null)
            {
                var contentBson = data.ContentObj.ToBsonDocument(new ObjectSerializer(ObjectSerializer.AllAllowedTypes));
                var id = contentBson["_id"];
                db.GetCollection<BsonDocument>("content").InsertOne(contentBson);
                requestBson.Add("contentId", contentBson["_id"].AsObjectId);
            }
            db.GetCollection<BsonDocument>("header").InsertOne(requestBson);
        }
    }
}
