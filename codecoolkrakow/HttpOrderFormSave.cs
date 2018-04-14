
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace codecoolkrakow
{
    public static class HttpOrderFormSave
    {
        [FunctionName("HttpOrderFormSave")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req,
            [Table("Orders", Connection = "StarageConnection")]ICollector<PhotoOrder> orderTable, TraceWriter log)
        {
           try
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                PhotoOrder orderData = JsonConvert.DeserializeObject<PhotoOrder>(requestBody);
                orderData.PartitionKey = System.DateTime.UtcNow.DayOfYear.ToString();
                orderData.RowKey = orderData.FileName;
                orderTable.Add(orderData);
            }
            catch (System.Exception ex)
            {
                log.Error("Something went wrong", ex);
                return new BadRequestObjectResult("Something went wrong");
            }
            return (ActionResult)new OkObjectResult($"Order processed");
        }
    }
}
