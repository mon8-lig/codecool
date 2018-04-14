
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace codecoolkrakow
{
    public static class HttpRetrieveOrder
    {
        [FunctionName("HttpRetrieveOrder")]
        public static async System.Threading.Tasks.Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, 
            [Table("Orders", Connection = "StorageConnection")]CloudTable orderTable, TraceWriter log)
        {
            string fileName = req.Query["fileName"];
            if (string.IsNullOrWhiteSpace(fileName))
                return new BadRequestResult();
            TableQuery<PhotoOrder> query = new TableQuery<PhotoOrder>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, fileName));
            TableQuerySegment<PhotoOrder> tableQueryResult = await orderTable.ExecuteQuerySegmentedAsync(query, null);
            var resultList = tableQueryResult.Results;

            if (resultList.Any())
            {
                var firstElement = resultList.First();
                return new JsonResult(new
                {
                    firstElement.CustomerEmail,
                    firstElement.FileName,
                    firstElement.RequiredHeight,
                    firstElement.RequiredWidth
                });
            }

            return new NotFoundResult();
        }
    }
}
