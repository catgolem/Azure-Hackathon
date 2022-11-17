using Azure;
using Azure.Data.Tables;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITableEntity = Azure.Data.Tables.ITableEntity;

namespace Azure_Hackathon
{
    public class DiaryEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
        public string DiaryDate { get; set; }
        DateTimeOffset? ITableEntity.Timestamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        ETag ITableEntity.ETag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
