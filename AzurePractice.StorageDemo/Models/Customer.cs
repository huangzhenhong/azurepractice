using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePractice.StorageDemo.Models
{
    public class Customer: TableEntity
    {
        public string CustomerName { get; set; }
        public Customer(string customerName, string city, string customerId) {
            PartitionKey = city;
            RowKey = customerId;
            CustomerName = customerName;
        }
    }
}
