using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Net;
using System.Linq;
using Microsoft.Extensions.Primitives;


namespace Estelle.Function
{

    public static class PostUser
    {
        [FunctionName("PostUser")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "update", Route = "createcar")] HttpRequest req,
            [CosmosDB(databaseName: "db", collectionName: "db-container",
            ConnectionStringSetting = "CosmosDbConnectionString"
            )]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            // string id = req.Query["id"];
            string year = req.Query["year"];
            string brand = req.Query["brand"];
            string model = req.Query["model"];
            string engineType = req.Query["engine type"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // id = id ?? data?.id;
            year = year ?? data?.year;
            brand = brand ?? data?.brand;
            model = model ?? data?.model;
            engineType = engineType ?? data?.engineType;

            // Add a JSON document to the output container.
            await documentsOut.AddAsync(new
            {
                // id = id,
                year = year,
                brand = brand,
                model = model,
                engineType = engineType
            });

            string responseMessage = string.IsNullOrEmpty(year)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Succeed";

            return new OkObjectResult(responseMessage);
        }
    }




    public static class GetAllUser
    {
        [FunctionName("GetAllUser")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "cars")]HttpRequest req,
            [CosmosDB("db", "db-container",
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c")]
                IEnumerable<Car> Result,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.->getall");

            //var
            var userList = new List<Car>();
            // Console.WriteLine(Result);
            foreach (Car user in Result)
            {
                userList.Add(user);
            }
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(userList);
            return jsonString;
        }
    }

    public static class GetSpecificUser
    {
        [FunctionName("GetSpecificUser")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "car/{id}")]HttpRequest req,
            [CosmosDB("db", "db-container",
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.id={id}")]
                IEnumerable<Car> Result,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.->get");

            List<Car> newUser = new List<Car>();

            foreach (Car user in Result)
            {
                newUser.Add(user);
            }
            var jsonString1 = Newtonsoft.Json.JsonConvert.SerializeObject(newUser);
            return jsonString1;
        }
    }




    public static class DeleteUser
    {
        [FunctionName("DeleteUser")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete",
                Route = "deletecar/{id}")]HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.id={id}")]
                 DocumentClient client, ILogger log, string id)
        {
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var collectionUri = UriFactory.CreateDocumentCollectionUri("db", "db-container");
            // Console.WriteLine(id);
            var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == id)
                    .AsEnumerable().FirstOrDefault();
            // Console.WriteLine(document);

            if (document == null)
            {
                return "no such document";
            }
            client.DeleteDocumentAsync(document.SelfLink, new RequestOptions { PartitionKey = new PartitionKey(document.Id) });
            return "Deleted";
        }
    }


}
