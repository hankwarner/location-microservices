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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Dictionary<string, LocationData>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(BadRequestObjectResult))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(NotFoundObjectResult))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(StatusCodeResult))]
        public static IActionResult GetLogonLocationData(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "location/{branchNumber}")] HttpRequest req,
            string branchNumber, 
            ILogger log)
        {
            try
            {
                var _services = new LocationServices(log);

                var logonLocationData = _services.RequestLogonLocationData(branchNumber);

                if (logonLocationData != null)
                    return new OkObjectResult("");

                var message = $"No logon locations found for branch number: {branchNumber}";
                log.LogWarning(message);

                return new NotFoundObjectResult("Invalid branch number")
                {
                    Value = message,
                    StatusCode = 404
                };
            }
            catch(Exception ex)
            {
                var title = "Exception in GetLogonLocationData";
                log.LogError(ex, title);
                var teamsMessage = new TeamsMessage(title, $"Error: {ex.Message}. Stacktrace: {ex.StackTrace}", "red", errorLogsUrl);
                teamsMessage.LogToTeams(teamsMessage);

                return new StatusCodeResult(500);
            }
        }
    }
}
