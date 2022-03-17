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
using Microsoft.Azure.Documents.Linq;


namespace Estelle.Function
{
    public static class PostCar
    {
        [FunctionName("PostCar")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createcar")] HttpRequest req, [CosmosDB(databaseName: "db", collectionName: "db-container",
            ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut, ILogger log)
        {
            string id = req.Query["id"];
            string year = req.Query["year"];
            string brand = req.Query["brand"];
            string model = req.Query["model"];
            string engineType = req.Query["engine type"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            id = id ?? data?.id;
            year = year ?? data?.year;
            brand = brand ?? data?.brand;
            model = model ?? data?.model;
            engineType = engineType ?? data?.engineType;

            if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(brand) && !string.IsNullOrEmpty(model) && !string.IsNullOrEmpty(engineType))
            {
                await documentsOut.AddAsync(new
                {
                    id = System.Guid.NewGuid().ToString(),
                    year = year,
                    brand = brand,
                    model = model,
                    engineType = engineType
                });
            }

            string responseMessage = (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(brand) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(engineType))
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Succeed";

            return new OkObjectResult(responseMessage);
        }
    }

    // search year
    public static class SearchYear
    {
        [FunctionName("SearchYear")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "car/search/year/{year}")] HttpRequest req, string year, [CosmosDB("db", "db-container", ConnectionStringSetting = "CosmosDbConnectionString", SqlQuery = "SELECT * FROM c WHERE c.year={year}")] IEnumerable<Car> Result, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            List<Car> newCar = new List<Car>();
            foreach (Car car in Result)
            {
                newCar.Add(car);
            }
            var searchYear = Newtonsoft.Json.JsonConvert.SerializeObject(newCar);
            return searchYear;
        }
    }
    public static class SearchBrand
    {
        [FunctionName("SearchBrand")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "car/search/brand/{brand}")] HttpRequest req, string brand, [CosmosDB("db", "db-container", ConnectionStringSetting = "CosmosDbConnectionString", SqlQuery = "SELECT * FROM c WHERE c.brand={brand}")] IEnumerable<Car> Result, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            List<Car> newCar = new List<Car>();
            foreach (Car car in Result)
            {
                newCar.Add(car);
            }
            var searchBrand = Newtonsoft.Json.JsonConvert.SerializeObject(newCar);
            return searchBrand;
        }
    }
    // 

    public static class GetAllCars
    {
        [FunctionName("GetAllCars")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "cars")]HttpRequest req,
            [CosmosDB("db", "db-container",
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c")]
                IEnumerable<Car> Result,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //var
            var carList = new List<Car>();
            // Console.WriteLine(Result);
            foreach (Car car in Result)
            {
                carList.Add(car);
            }
            var getCars = Newtonsoft.Json.JsonConvert.SerializeObject(carList);
            return getCars;
        }
    }

    public static class GetCar
    {
        [FunctionName("GetCar")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "car/{id}")] HttpRequest req,
            [CosmosDB("db", "db-container",
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.id={id}")]
                IEnumerable<Car> Result,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            List<Car> newCar = new List<Car>();

            foreach (Car car in Result)
            {
                newCar.Add(car);
            }
            var getCar = Newtonsoft.Json.JsonConvert.SerializeObject(newCar);
            return getCar;
        }
    }

    public static class DeleteCar
    {
        [FunctionName("DeleteCar")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "deletecar/{id}")] HttpRequest req, [CosmosDB(ConnectionStringSetting = "CosmosDbConnectionString", SqlQuery = "SELECT * FROM c WHERE c.id={id}")] DocumentClient client, ILogger log, string id)
        {
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var collectionUri = UriFactory.CreateDocumentCollectionUri("db", "db-container");

            var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == id).AsEnumerable().FirstOrDefault();

            await client.DeleteDocumentAsync(document.SelfLink, new RequestOptions { PartitionKey = new PartitionKey(document.Id) });
            return new OkResult();
        }
    }

    public static class UpdateCar
    {
        [FunctionName("UpdateCar")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "updatecar/{id}")] HttpRequest req, [CosmosDB(ConnectionStringSetting = "CosmosDbConnectionString", SqlQuery = "SELECT * FROM c WHERE c.id={id}")] DocumentClient client, ILogger log, string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Console.WriteLine(requestBody);

            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var collectionUri = UriFactory.CreateDocumentCollectionUri("db", "db-container");

            var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == id).AsEnumerable().FirstOrDefault();

            if (document == null)
            {
                return new NotFoundResult();
            }
            document.SetPropertyValue("id", data.id);
            document.SetPropertyValue("year", data.year);
            document.SetPropertyValue("brand", data.brand);
            document.SetPropertyValue("model", data.model);
            document.SetPropertyValue("engineType", data.engineType);

            await client.ReplaceDocumentAsync(document);

            return new OkResult();
        }
    }
}
