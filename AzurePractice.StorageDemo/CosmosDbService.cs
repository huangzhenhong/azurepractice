using Microsoft.Azure.Cosmos;
using AzurePractice.StorageDemo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePractice.StorageDemo
{
    public static class CosmosDbService
    {
        private static readonly string _connectionString = "AccountEndpoint=https://mycosmosdb100.documents.azure.com:443/;AccountKey=Z5RDlvsNoKmbw29Z4evbZ54g3aiGHKKBzW9MOk7daXy8ebesrZoKviF0ovZexcnRea8EcAhgWOKbwZ6mPUB3rA==;";
        private static readonly string _databaseName = "appdb";
        private static readonly string _containerName = "Course";
        private static readonly string _partitionKey = "/courseid";

        public static Database CreateDb() {
            CosmosClient client = new CosmosClient(_connectionString);
            client.CreateDatabaseIfNotExistsAsync(_databaseName).GetAwaiter().GetResult();
            Console.WriteLine("Database has been created");
            return client.GetDatabase(_databaseName);
        }

        public static Container GetOrCreateContainer() {
            var _database = CreateDb();
            var response = _database.CreateContainerIfNotExistsAsync(_containerName, _partitionKey).GetAwaiter().GetResult();
            Console.WriteLine("Container has been created");
            return _database.GetContainer(_containerName);
        }

        public static void Add(Course course) {
            var container = GetOrCreateContainer();
            container.CreateItemAsync<Course>(course, new PartitionKey(course.courseid)).GetAwaiter().GetResult();
            Console.WriteLine("Item created");
        }

        public static void BulkInsert(List<Course> courses) {
            CosmosClient client = new CosmosClient(_connectionString, new CosmosClientOptions { AllowBulkExecution = true });
            var container = client.GetContainer(_databaseName, _containerName);

            List<Task> tasks = new List<Task>();
            foreach(var course in courses)
            {
                tasks.Add(container.CreateItemAsync<Course>(course, new PartitionKey(course.courseid)));
            }
            Task.WhenAll(tasks).GetAwaiter().GetResult();
            Console.WriteLine("Bulk insert completed");
        }

        public static void GetCourse(string courseId) {
            var query = $"select * from c where c.courseid = '{courseId}'";
            QueryDefinition queryDefinition = new QueryDefinition(query);
            var container = GetOrCreateContainer();
            FeedIterator<Course> _iterator = container.GetItemQueryIterator<Course>(queryDefinition);

            while (_iterator.HasMoreResults) {
                FeedResponse<Course> _response = _iterator.ReadNextAsync().GetAwaiter().GetResult();
                foreach (var course in _response) {
                    Console.WriteLine($"Id is {course.id}");
                    Console.WriteLine($"CourseId is { course.courseid }");
                    Console.WriteLine($"CourseName is { course.coursename }");
                    Console.WriteLine($"Rating is { course.rating }");
                }
            }
        }

        public static void Update(string id, string partitionKey) {
            var container = GetOrCreateContainer();
            ItemResponse<Course> itemResponse = container.ReadItemAsync<Course>(id, new PartitionKey(partitionKey)).GetAwaiter().GetResult();
            Course course = itemResponse.Resource;
            course.rating = 3.0m;
            container.ReplaceItemAsync<Course>(course, id, new PartitionKey(partitionKey));
            Console.WriteLine("Item has been updated");
        }

        public static void Delete(string id, string partitionKey)
        {
            var container = GetOrCreateContainer();
            container.DeleteItemAsync<Course>(id, new PartitionKey(partitionKey));
            Console.WriteLine("Item has been Deleted");
        }

        public static void Embed(Course course)
        {
            var container = GetOrCreateContainer();
            container.CreateItemAsync<Course>(course, new PartitionKey(course.courseid)).GetAwaiter().GetResult();
            Console.WriteLine("Item created");
        }

        public static void CallStoredProcedure() {
            var container = GetOrCreateContainer();
            string result = container.Scripts.ExecuteStoredProcedureAsync<string>("demo", new PartitionKey(string.Empty), null).GetAwaiter().GetResult();
            Console.WriteLine(result);
        }

        public static void SendDataToCosmosDb(string containerName) {
            FileStream _fs = new FileStream(System.Environment.CurrentDirectory + @"\Data\QueryResult.json", FileMode.Open, FileAccess.Read);
            StreamReader _reader = new StreamReader(_fs);
            JsonTextReader _json_reader = new JsonTextReader(_reader);

            CosmosClient _client = new CosmosClient(_connectionString);
            Container _container = _client.GetContainer(_databaseName, containerName);

            while (_json_reader.Read())
            {
                if (_json_reader.TokenType == JsonToken.StartObject)
                {
                    JObject _object = JObject.Load(_json_reader);
                    Activity _activity = _object.ToObject<Activity>();
                    _activity.id = Guid.NewGuid().ToString();
                    _container.CreateItemAsync<Activity>(_activity, new PartitionKey(_activity.Operationname)).GetAwaiter().GetResult();
                    Console.WriteLine($"Adding item {_activity.Correlationid}");
                }
            }

            Console.WriteLine("Written data to Azure Cosmos DB");
        }
    }
}
