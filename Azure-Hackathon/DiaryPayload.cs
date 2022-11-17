using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using Newtonsoft.Json.Serialization;

namespace Azure_Hackathon
{
    [OpenApiExample(typeof(TaskPayloadExample))]
    public class DiaryPayload
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("diaryDate")]
        public string DiaryDate { get; set; }


        public static DiaryPayload ConvertTo(string value) => JsonSerializer.Deserialize<DiaryPayload>(value);

        public override string ToString()
        {
            var options = new JsonSerializerOptions { WriteIndented = false };
            return JsonSerializer.Serialize(this, options);
        }
    }

    public class TaskPayloadExample : OpenApiExample<DiaryPayload>
    {
        public override IOpenApiExample<DiaryPayload> Build(NamingStrategy namingStrategy = null)
        {
            this.Examples.Add(
                 OpenApiExampleResolver.Resolve(
                     "BookRequestExample",
                     new DiaryPayload()
                     {
                         Content = "今日はいい天気だった。",
                         Name = "hoge",
                         ImageUrl = "",
                         UserId = "123456789",
                         DiaryDate = "2022-11-06"
                     },
                     namingStrategy
                 ));
            return this;
        }
    }
}
