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
                var title = "SqlException in RequestLogonLocationData";
                _logger.LogWarning(@"{0}. Retrying.... {1}", title, ex);
                if (count == 3)
                {
#if !DEBUG
                    var teamsMessage = new TeamsMessage(errorMessage, $"Error: {ex.Message}. Stacktrace: {ex.StackTrace}", "red", LocationFunctions.errorLogsUrl);
                    teamsMessage.LogToTeams(teamsMessage);
#endif
                    _logger.LogError(@"{0}: {1}", title, ex);
                }
            });

            return retryPolicy.Execute(() =>
            {
                var connString = Environment.GetEnvironmentVariable("DIST_DB_CONN");

                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    var query = @"
                        SELECT BranchNumber, Address1, City, State, Zip, OverpackCapable, ProcessingTime, 
                                WarehouseManagementSoftware, BranchLocation, DCLocation, SODLocation, Logon, ShipHub, FedExESTCutoffTimes 
                        FROM Data.DistributionCenter
                        WHERE 
                            Logon in ('D98 DISTRIBUTION CENTERS', (SELECT Logon FROM Data.DistributionCenter WHERE BranchNumber = '58')) 
                            AND Active = 1 AND Zip != '0' AND BranchNumber != '39'";

                    var results = conn.Query<LocationData>(query, new { branch }, commandTimeout: 6).ToList();

                    var logonLocationData = results.ToDictionary(locationData => locationData.BranchNumber);

                    conn.Close();

                    return logonLocationData;
                }
            });
        }
    }
}
