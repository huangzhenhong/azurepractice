using Microsoft.Azure.Cosmos.Table;
using AzurePractice.StorageDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePractice.StorageDemo
{
    public static class AzTableService
    {
        private static string connection_string = "DefaultEndpointsProtocol=https;AccountName=azstore10000;AccountKey=sQ5FVHkOwXOi6kqDh7YpnS3s088ILKRL1EiCNVs1c+MfSJ/lYyW9dHZXYcUIB1AydphCWIzCIfC5QBt+pClSBA==;EndpointSuffix=core.windows.net";
        
        public static CloudTable GetOrCreateTable(string tableName) {
            CloudStorageAccount account = CloudStorageAccount.Parse(connection_string);
            CloudTableClient tableClient = account.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();

            return table;
        }

        public static Customer Get(string partitionKey, string rowKey, string tableName)
        {
            var table = GetOrCreateTable(tableName);
            TableOperation operation = TableOperation.Retrieve(partitionKey, rowKey);
            TableResult result = table.Execute(operation);

            return result.Result as Customer;
        }

        public static TableResult Insert(Customer customer, string tableName) {
            var table = GetOrCreateTable(tableName);
            TableOperation operation = TableOperation.Insert(customer);
            TableResult result = table.Execute(operation);
            return result;
        }

        public static TableResult Update(Customer customer, string tableName)
        {
            var table = GetOrCreateTable(tableName);
            TableOperation operation = TableOperation.InsertOrMerge(customer);
            TableResult result = table.Execute(operation);
            return result;
        }

        public static TableResult Delete(Customer customer, string tableName)
        {
            var table = GetOrCreateTable(tableName);
            TableOperation operation = TableOperation.Delete(customer);
            TableResult result = table.Execute(operation);
            return result;
        }

        public static TableBatchResult InsertMultiple(List<Customer> customers, string tableName)
        {
            var table = GetOrCreateTable(tableName);
            TableBatchOperation operation = new TableBatchOperation();
            foreach(var cust in customers)
            {
                operation.Insert(cust);
            }
            TableBatchResult result = table.ExecuteBatch(operation);
            return result;
        }


    }
}
