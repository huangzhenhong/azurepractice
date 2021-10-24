using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using System;
using System.IO;

namespace AzurePractice.StorageDemo
{
    public static class BlobService
    {
        private static string _containerName = "mydata";
        private static string _connectionStr = "DefaultEndpointsProtocol=https;AccountName=azstore10000;AccountKey=zDb8ipb0xTBns+nSUd4+Hflp8WUOZdbETYqjh8VkV+TCrUL6ZtA7nf86JhLQuXs/S+4qaOjgQlnU4IJLvKkw1Q==;EndpointSuffix=core.windows.net";
        private static string _downloadPath = "C:\\tenp";

        public static void BlobLeaseTest()
        {
            var uri = GenerateSAS();
            BlobContainerClient containerClient = new BlobContainerClient(uri);
            BlobClient blobClient = containerClient.GetBlobClient("test.txt");

            MemoryStream stream = new MemoryStream();
            blobClient.DownloadTo(stream);
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            Console.WriteLine(reader.ReadToEnd());

            BlobLeaseClient leaseClient = blobClient.GetBlobLeaseClient();
            BlobLease lease = leaseClient.Acquire(TimeSpan.FromSeconds(30));
            Console.WriteLine($"The lease id is: {lease.LeaseId}");

            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("This is a third line");
            writer.Flush();

            BlobUploadOptions options = new BlobUploadOptions()
            {
                Conditions = new BlobRequestConditions()
                {
                    LeaseId = lease.LeaseId
                }
            };

            stream.Position = 0;
            blobClient.Upload(stream, options);
            leaseClient.Release();

            Console.ReadKey();
        }

        public async static void ReadBlob()
        {
            var uri = GenerateSAS();
            BlobContainerClient containerClient = new BlobContainerClient(uri);
            BlobClient blobClient = containerClient.GetBlobClient("index.html");
            //await blobClient.DownloadToAsync(_downloadPath);

            // read metadata
            BlobProperties blobProperties = blobClient.GetProperties();
            foreach (var item in blobProperties.Metadata)
            {
                Console.WriteLine(item.Key);
                Console.WriteLine(item.Value);
            }
        }

        public static Uri GenerateSAS()
        {

            BlobServiceClient _client = new BlobServiceClient(_connectionStr);
            BlobContainerClient _blobContainerClient = _client.GetBlobContainerClient(_containerName);

            BlobSasBuilder _builder = new BlobSasBuilder()
            {
                BlobContainerName = _containerName
            };

            _builder.SetPermissions(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.List | BlobContainerSasPermissions.Add | BlobContainerSasPermissions.Create | BlobContainerSasPermissions.Write);
            _builder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);

            return _blobContainerClient.GenerateSasUri(_builder);
        }
    }
}
