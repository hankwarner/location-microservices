using System;

namespace LocationMicroservices.Models
{
    public class LocationData
    {
        public string BranchNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool WarehouseManagementSoftware { get; set; }
        public bool BranchLocation { get; set; }
        public bool DCLocation { get; set; }
        public bool SODLocation { get; set; }
        public string Logon { get; set; }
        public bool ShipHub { get; set; }
        public bool OverpackCapable { get; set; }
        public TimeSpan FedExESTCutoffTimes { get; set; }
        public int ProcessingTime { get; set; }
    }
}
