using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using LocationMicroservices.Models;
using LocationMicroservices.Services;
using System.Collections.Generic;
using System.Net;

namespace LocationMicroservices
{
    public static class LocationFunctions
    {
        public static string errorLogsUrl = Environment.GetEnvironmentVariable("ERROR_LOGS_URL");


        [FunctionName("GetLogonLocationData")]
        [ProducesResponseType(typeof(Dictionary<string, LocationData>), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
        [ProducesResponseType(typeof(ObjectResult), 500)]
        public static IActionResult GetLogonLocationData(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "location/{branchNumber}")] HttpRequest req,
            string branchNumber, 
            ILogger log)
        {
            try
            {
                var _services = new LocationServices(log);

                var logonLocationData = _services.RequestLogonLocationData(branchNumber);

                if (logonLocationData == null)
                {
                    var message = $"No logon locations found for branch number: {branchNumber}";
                    log.LogWarning(message);

                    return new NotFoundObjectResult("Invalid branch number") { Value = message, StatusCode = 404 };
                }

                return new OkObjectResult(logonLocationData);
            }
            catch(Exception ex)
            {
                var title = "Exception in GetLogonLocationData";
                log.LogError(@"{0}: {1}", title, ex);
#if !DEBUG
                var teamsMessage = new TeamsMessage(title, $"Error: {ex.Message}. Stacktrace: {ex.StackTrace}", "red", errorLogsUrl);
                teamsMessage.LogToTeams(teamsMessage);
#endif
                return new ObjectResult(ex.Message) { StatusCode = 500, Value = "Failure" };
            }
        }


        // GetBranchLogonID
    }
}
