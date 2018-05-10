
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using System.Collections.Generic;

namespace codecoolkrakow
{
    public static class HttpRetrieveOrder
    {
        [FunctionName("HttpRetrieveOrder")]
        public static async System.Threading.Tasks.Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, 
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
                string[] resulotions = firstElement.Resolutions.Split(',');
                List<PictureResizeRequest> requests = new List<PictureResizeRequest>();

                foreach (var resolution in resulotions)
                {
                    string[] resParams = resolution.Split('x');
                    requests.Add(new PictureResizeRequest()
                    {
                        FileName = firstElement.FileName,
                        RequiredWidth = System.Int32.Parse(resParams[0]),
                        RequiredHeight = System.Int32.Parse(resParams[1])
                    });
                }
                return new JsonResult(new { requests, firstElement.CustomerEmail });
            }

            return new NotFoundResult();
        }
    }
}
