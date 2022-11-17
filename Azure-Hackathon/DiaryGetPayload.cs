using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Azure_Hackathon
{
    [OpenApiExample(typeof(DiaryGetPayloadExample))]
    public class DiaryGetPayload
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        public static DiaryGetPayload ConvertTo(string value) => JsonSerializer.Deserialize<DiaryGetPayload>(value);
    }

    public class DiaryGetPayloadExample : OpenApiExample<DiaryGetPayload>
    {
        public override IOpenApiExample<DiaryGetPayload> Build(NamingStrategy namingStrategy = null)
        {
            this.Examples.Add(
                 OpenApiExampleResolver.Resolve(
                     "BookRequestExample",
                     new DiaryGetPayload()
                     {
                         UserId = "123456789",
                     },
                     namingStrategy
                 ));
            return this;
        }
    }
}
