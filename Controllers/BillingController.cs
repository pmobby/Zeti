using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ZetiTest.Models;
using ZetiTest.ViewModels;

namespace ZetiTest.Controllers
{
    [Route("[controller]/[action]")]
    public class BillingController : Controller
    {
        private const double CostPerMile = 0.207;

        [HttpGet]
        public async Task<IActionResult> GetVehicles()
        {
            List<Vehicles> listOfVehicles = new List<Vehicles>();            
            List<Vehicles> listOfVehicleHistoryStart = new List<Vehicles>();
            List<Vehicles> listOfVehicleHistoryEnd = new List<Vehicles>();
            List<CustomerBillModel> bills = new List<CustomerBillModel>();

            string baseendpoint = @"https://funczetiinterviewtest.azurewebsites.net"; 
            

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseendpoint);

                client.DefaultRequestHeaders.Clear();

                //request data format for json 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //request for endpoint data for vehicle and its history
                HttpResponseMessage vehicles = await client.GetAsync("api/vehicles");

                HttpResponseMessage vehiclehistorystart = await client.GetAsync("/api/vehicles/history/2021-02-01T00:00:00Z");

                HttpResponseMessage vehiclehistoryend = await client.GetAsync("/api/vehicles/history/2021-02-28T23:59:00Z");


                if (vehicles.IsSuccessStatusCode)
                {
                    if (vehiclehistorystart.IsSuccessStatusCode)
                    {
                        if (vehiclehistoryend.IsSuccessStatusCode)
                        {
                            var vehiclesdata = vehicles.Content.ReadAsStringAsync().Result;
                            var vehicleshistorystartdata = vehiclehistorystart.Content.ReadAsStringAsync().Result;
                            var vehicleshistoryenddata = vehiclehistoryend.Content.ReadAsStringAsync().Result;

                            listOfVehicles = JsonConvert.DeserializeObject<List<Vehicles>>(vehiclesdata);
                            listOfVehicleHistoryStart = JsonConvert.DeserializeObject<List<Vehicles>>(vehicleshistorystartdata);
                            listOfVehicleHistoryEnd = JsonConvert.DeserializeObject<List<Vehicles>>(vehicleshistoryenddata);
                        }
                    }
                    
                }

                bills = CalculateMileage(listOfVehicleHistoryStart, listOfVehicleHistoryEnd);            
            }
            BillingViewModel billingmodel = new BillingViewModel
            {
                VehiclesWithBills = bills,
                VehiclesHistoryStart = listOfVehicleHistoryStart,
                VehiclesHistoryEnd = listOfVehicleHistoryEnd
            };

            return View(billingmodel);
        }

        private List<CustomerBillModel> CalculateMileage(List<Vehicles> vehiclehistorystart, List<Vehicles> vehiclehistoryend)
        {
            var customerbill = new List<CustomerBillModel>();

            
            double odometer1 = 0;
            double odometer2 = 0;
            int i = 0;
            foreach (var ve in vehiclehistoryend)
            {
                int j = 0;
                foreach(var vs in vehiclehistorystart)
                {
                    if (j == 0)
                    {
                        odometer1 = vs.State.OdometerInMeters;
                    } 
                    else
                    {
                        odometer2 = vs.State.OdometerInMeters;
                    }
                    j++;                 
                }
                if(i == 0)
                {
                    var miles = ve.State.OdometerInMeters - odometer1;
                    double mileageCost = miles * CostPerMile;
                    customerbill.Add(new CustomerBillModel { Vin = ve.Vin, LicensePlate = ve.LicensePlate, Make = ve.Make, Model = ve.Model, MilesTravelled = miles, CostOfUsage = mileageCost });
                }
                else
                {
                    var miles = ve.State.OdometerInMeters - odometer2;
                    double mileageCost = miles * CostPerMile;
                    customerbill.Add(new CustomerBillModel { Vin = ve.Vin, LicensePlate = ve.LicensePlate, Make = ve.Make, Model = ve.Model, MilesTravelled = miles, CostOfUsage = mileageCost });
                }
               
                i++;
            }

            return customerbill;
        }
    }
}
