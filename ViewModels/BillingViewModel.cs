using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZetiTest.Models;

namespace ZetiTest.ViewModels
{
    public class BillingViewModel
    {
        public List<CustomerBillModel> VehiclesWithBills { get; set; }
        public List<Vehicles> VehiclesHistoryStart { get; set; }
        public List<Vehicles> VehiclesHistoryEnd { get; set; }
    }
}
