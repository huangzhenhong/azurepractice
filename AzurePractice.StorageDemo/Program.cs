using AzurePractice.StorageDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace AzurePractice.StorageDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            #region CosmosDb

            //CosmosDbService.CreateDb();
            // Add new item
            {
                //var course = new Course()
                //{
                //    id = "1",
                //    courseid = "C00100",
                //    coursename = "az-204",
                //    rating = 4.5m
                //};
                //CosmosDbService.Add(course);
            }
            // Bulk insert
            {
                //var courses = new List<Course>() {
                //    new Course()
                //    {
                //        id = "2",
                //        courseid = "C00200",
                //        coursename = "az-104",
                //        rating = 4.6m
                //    },
                //    new Course()
                //    {
                //        id = "3",
                //        courseid = "C00300",
                //        coursename = "az-304",
                //        rating = 4.7m
                //    },
                //    new Course()
                //    {
                //        id = "4",
                //        courseid = "C00400",
                //        coursename = "az-404",
                //        rating = 4.8m
                //    }
                //};

                //CosmosDbService.BulkInsert(courses);
            }
            // Read Update Delete Item
            {
                //CosmosDbService.GetCourse("C00100");

                //CosmosDbService.Update("2", "C00200");

                //CosmosDbService.Delete("2", "C00200");
            }

            // Embedding Data
            {
                //var course = new Course()
                //{
                //    id = "2",
                //    courseid = "C00200",
                //    coursename = "az-204",
                //    rating = 4.5m,
                //    orders = new List<Order>() { 
                //        new Order() { orderid = "001", price = 25 },
                //        new Order() { orderid = "002", price = 35 }
                //    }
                //};
                //CosmosDbService.Add(course);
            }

            // Executing a stored procedure
            {
                //CosmosDbService.CallStoredProcedure();
            }

            // 
            CosmosDbService.SendDataToCosmosDb("activity");
            #endregion

            #region Table Storage
            {
                /// Create a table
                //var tableName = "test1";
                //AzTableService.CreateTable(tableName);
                //Console.WriteLine($"Table {tableName} created!");

                // Insert Entity
                //Customer customer = new Customer("UserA", "Chicago", "C1");
                //AzTableService.Insert(customer, tableName);
                //Console.WriteLine($"UserA inserted!");

                // Batch Operation
                //List<Customer> customers = new List<Customer>() { 
                //    new Customer("UserB", "Chicago", "C2"),
                //    new Customer("UserC", "Chicago", "C3"),
                //    new Customer("UserD", "Chicago", "C4"),
                //};
                //AzTableService.InsertMultiple(customers, tableName);
                //Console.WriteLine($"Multiple Users inserted!");

                //Read Data
                //var cust = AzTableService.Get("Chicago", "C1", tableName);
                //Console.WriteLine($"{cust.CustomerName}");
            }
            #endregion

            Console.ReadLine();
        }
    }
}
