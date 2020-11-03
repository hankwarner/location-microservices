using Dapper;
using LocationMicroservices.Models;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LocationMicroservices.Services
{
    public class LocationServices
    {
        public ILogger _logger { get; set; }

        public LocationServices(ILogger log)
        {
            _logger = log;
        }


        public Dictionary<string, LocationData> RequestLogonLocationData(string branch)
        {
            var retryPolicy = Policy.Handle<SqlException>().Retry(3, (ex, count) =>
            {
                const string errorMessage = "SqlException in RequestLogonLocationData";
                _logger.LogWarning(ex, $"{errorMessage} . Retrying...");
                if (count == 3)
                {
                    var teamsMessage = new TeamsMessage(errorMessage, $"Error: {ex.Message}. Stacktrace: {ex.StackTrace}", "red", LocationFunctions.errorLogsUrl);
                    teamsMessage.LogToTeams(teamsMessage);
                    _logger.LogError(ex, errorMessage);
                }
            });

            return retryPolicy.Execute(() =>
            {
                var connString = Environment.GetEnvironmentVariable("DIST_DB_CONN");

                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    var query = @"
                        SELECT BranchNumber, Address1, Address2, City, State, Zip, OverpackCapable, ProcessingTime, 
                               WarehouseManagementSoftware, BranchLocation, DCLocation, SODLocation, Logon, ShipHub, FedExESTCutoffTimes 
                        FROM [FergusonIntegration].[sourcing].[DistributionCenter] 
                        WHERE 
                            Logon in ('D98 DISTRIBUTION CENTERS', (SELECT Logon FROM [FergusonIntegration].[sourcing].[DistributionCenter] WHERE BranchNumber = @branch)) 
                            AND Active = 1 AND Zip != '0' AND BranchNumber != '39'";

                    var results = conn.Query<LocationData>(query, new { branch }, commandTimeout: 3).ToList();

                    var logonLocationData = results.ToDictionary(locationData => locationData.BranchNumber);

                    conn.Close();

                    return logonLocationData;
                }
            });
        }
    }
}
