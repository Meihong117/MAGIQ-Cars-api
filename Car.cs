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

public class Car
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("year")]
    public string Year { get; set; }

    [JsonProperty("brand")]
    public string Brand { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }
    [JsonProperty("engineType")]
    public string EngineType { get; set; }

}