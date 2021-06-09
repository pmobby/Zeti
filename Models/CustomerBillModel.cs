using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZetiTest.Models
{
    public class CustomerBillModel
    {
        public string Vin { get; set; }
        public string LicensePlate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }       
        public double MilesTravelled { get; set; }
        public double CostOfUsage { get; set; }
    }
}
