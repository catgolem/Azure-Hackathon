using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using TableAttribute = Microsoft.Azure.WebJobs.TableAttribute;

namespace Azure_Hackathon
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> log)
        {
            _logger = log;
        }

        [FunctionName("Function1")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }


        [OpenApiOperation(operationId: "Run", tags: new[] { "diarys" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(DiaryEntity), Description = "The OK response")]
        [FunctionName("TableClientInput")]
        public static async Task<IActionResult> TableClientInputRun(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("diary"/*, Connection = "MyTableService"*/)] TableClient tableClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string userId = req.Query["userId"];
            Console.WriteLine($"PartitionKey eq {userId}");
            AsyncPageable<DiaryEntity> queryResults = tableClient.QueryAsync<DiaryEntity>(filter: $"UserId eq '{userId}'");
            Console.WriteLine(queryResults);
            //await foreach (TaskEntity entity in queryResults)
            //{
            //    log.LogInformation($"{entity.PartitionKey}\t{entity.RowKey}\t{entity.Content}\t{entity.Name}");
            //    rowKeys.Add(entity.RowKey);
            //}

            return new OkObjectResult(queryResults);
        }


        [OpenApiOperation(operationId: "Run", tags: new[] { "diarys" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(DiaryPayload), Description = "The **Json** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DiaryEntity), Description = "The OK response")]
        [FunctionName("TableClientOutput")]
        public static async Task<IActionResult> TableClientOutputRun(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Table("diary"/*, Connection = "MyTableService"*/)] TableClient tableClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            DiaryPayload payload = DiaryPayload.ConvertTo(requestBody);
            Console.WriteLine(payload);
            Console.WriteLine(payload.ImageUrl);
            string[] words = payload.DiaryDate.Split('-');
            var taskEntity = new DiaryEntity()
            {
                PartitionKey = words[0] + "-" + words[1],
                RowKey = payload.UserId + "-" + payload.DiaryDate,
                Content = payload.Content,
                Name = payload.Name,
                ImageUrl = string.IsNullOrEmpty(payload.ImageUrl) ? "" : payload.ImageUrl,
                UserId = payload.UserId,
                DiaryDate = payload.DiaryDate,
            };

            await tableClient.AddEntityAsync(taskEntity);
            return new OkObjectResult(taskEntity);
        }


        [OpenApiOperation(operationId: "Run", tags: new[] { "diarys" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(DiaryPayload), Description = "The **Json** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DiaryEntity), Description = "The OK response")]
        [FunctionName("TableClientUpdate")]
        public static async Task<IActionResult> TableClientUpdate(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
            [Table("diary"/*, Connection = "MyTableService"*/)] TableClient tableClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            DiaryPayload payload = DiaryPayload.ConvertTo(requestBody);
            string[] words = payload.DiaryDate.Split('-');
            DiaryEntity entity = tableClient.GetEntity<DiaryEntity>(words[0] + "-" + words[1], payload.UserId + "-" + payload.DiaryDate);
            entity.Content = payload.Content;
            await tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace);
            return new OkObjectResult(entity);
        }
    }
}

