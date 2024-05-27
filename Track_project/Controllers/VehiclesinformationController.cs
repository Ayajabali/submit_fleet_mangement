using FPro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Track_project.Data;
using Track_project.Models;
using Track_project.Services;

namespace Track_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesinformationController : ControllerBase
    {
        private readonly DemoContext2 _context;
        private readonly WebSocketManagerService _webSocketManagerService;
        public VehiclesinformationController(DemoContext2 context, WebSocketManagerService webSocketManagerService)

        {
            _context = context;
            _webSocketManagerService = webSocketManagerService;

        }

        // GET: api/Vehiclesinformation
        [HttpGet]
        public async Task<ActionResult<GVAR>> GetVehiclesinformations()
        {
            try
            {
                var vehiclesinformations = await _context.Vehiclesinformations.ToListAsync();
                if (vehiclesinformations == null || !vehiclesinformations.Any())
                {
                    return NotFound("No vehicles information found.");
                }

                var gvar = new GVAR();

                gvar.DicOfDT.TryAdd("VehiclesInformations", ConvertToDataTable(vehiclesinformations));

                return Ok(new { STS = 1, gvar});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GVAR>> GetVehiclesInformation(long id)
        {
            // Use 'FirstOrDefaultAsync' instead of 'FirstAsync' to avoid exceptions if no data is found.
            var vehiclesInformation = await _context.Vehiclesinformations
                                                    .Include(vi => vi.Vehicle) // Assuming Vehicle relationship is set
                                                    .Include(vi => vi.Driver)  // Assuming Driver relationship is set
                                                    .FirstOrDefaultAsync(i => i.Vehicleid == id);

            // Check if the requested VehiclesInformation exists
            if (vehiclesInformation == null)
            {
                return NotFound(new { STS = 0 });
            }

            // Fetch the most recent route history
            var history = await _context.Routehistories
                                        .Where(r => r.Vehicleid == id)
                                        .OrderByDescending(e => e.Epoch)
                                        .FirstOrDefaultAsync();

            var gvar = new GVAR();

            // Build the details dictionary
            var vehiclesInformationsDetails = new ConcurrentDictionary<string, string>
            {
                ["VehicleNumber"] = vehiclesInformation.Vehicle?.Vehiclenumber?.ToString() ?? "N/A",
                ["VehicleType"] = vehiclesInformation.Vehicle?.Vehicletype ?? "N/A",
                ["DriverName"] = vehiclesInformation.Driver?.Drivername ?? "N/A",
                ["PhoneNumber"] = vehiclesInformation.Driver?.Phonenumber.ToString() ?? "N/A",
                ["LastPosition"] = history != null ? $"{history.Latitude}, {history.Longitude}" : "N/A",
                ["VehicleMake"] = vehiclesInformation.Vehiclemake ?? "N/A",
                ["VehicleModel"] = vehiclesInformation.Vehiclemodel ?? "N/A",
                ["LastGPSTime"] = history?.Epoch.ToString() ?? "N/A",
                ["LastGPSSpeed"] = history?.Vehiclespeed ?? "N/A",
                ["LastAddress"] = history?.Address ?? "N/A"
            };

            gvar.DicOfDic["VehiclesInformation"] = vehiclesInformationsDetails;

            return Ok(new { STS = 1, gvar });
        }



        // POST: api/Vehiclesinformation
        [HttpPost]
        public async Task<ActionResult<GVAR>> PostVehiclesinformation([FromBody] dynamic jsonData)
        {
            var tags = jsonData.DicOfDic.Tags;
            var vehicleId = tags["VehicleId"];
            var driverId = tags["DriverId"];
            var vehicleMake = tags["VehicleMake"];
            var vehicleModel = tags["VehicleModel"];
            var purchaseDate = tags["PurchaseDate"];

            var gvar = new GVAR();
            var vehicleInformationDetails = new System.Collections.Concurrent.ConcurrentDictionary<string, string>
            {
                ["VehicleId"] = vehicleId,
                ["DriverId"] = driverId,
                ["VehicleMake"] = vehicleMake,
                ["VehicleModel"] = vehicleModel,
                ["PurchaseDate"] = purchaseDate
            };
            gvar.DicOfDic.TryAdd("VehiclesInformation", vehicleInformationDetails);

            var newVehicleInformation = new Vehiclesinformation
            {
                Vehicleid = Convert.ToInt64(vehicleId),
                Driverid = Convert.ToInt64(driverId),
                Vehiclemake = vehicleMake,
                Vehiclemodel = vehicleModel,
                Purchasedate = Convert.ToInt64(purchaseDate)
            };

            try
            {
                _context.Vehiclesinformations.Add(newVehicleInformation);
                await _context.SaveChangesAsync();

                var newId = newVehicleInformation.Id;

                var locationUri = Url.Action(nameof(GetVehiclesinformations), new { id = newId });
                _webSocketManagerService.Broadcast("New vehicle information added");
                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        // PUT: api/Vehiclesinformation/5
        [HttpPut("{id}")]
        public async Task<ActionResult<GVAR>> PutVehiclesinformation(long id, [FromBody] dynamic jsonData)
        {
            try
            {
                var tags = jsonData.DicOfDic.Tags;
                var vehicleId = tags["VehicleId"];
                var driverId = tags["DriverId"];
                var vehicleMake = tags["VehicleMake"];
                var vehicleModel = tags["VehicleModel"];
                var purchaseDate = tags["PurchaseDate"];

                var vehiclesinformation = await _context.Vehiclesinformations.FindAsync(id);
                if (vehiclesinformation == null)
                {
                    return NotFound(new { STS = 0 });
                }

                vehiclesinformation.Vehicleid = Convert.ToInt64(vehicleId);
                vehiclesinformation.Driverid = Convert.ToInt64(driverId);
                vehiclesinformation.Vehiclemake = vehicleMake;
                vehiclesinformation.Vehiclemodel = vehicleModel;
                vehiclesinformation.Purchasedate = Convert.ToInt64(purchaseDate);

                await _context.SaveChangesAsync();

                var gvar = new GVAR();
                var vehicleInformationDetails = new ConcurrentDictionary<string, string>
                {
                    ["Id"] = id.ToString(),
                    ["VehicleId"] = vehicleId,
                    ["DriverId"] = driverId,
                    ["VehicleMake"] = vehicleMake,
                    ["VehicleModel"] = vehicleModel,
                    ["PurchaseDate"] = purchaseDate
                };
                gvar.DicOfDic.TryAdd("VehiclesInformation", vehicleInformationDetails);
                _webSocketManagerService.Broadcast("New vehicle information updated");

                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }



        // DELETE: api/Vehiclesinformation/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GVAR>> DeleteVehiclesinformation(long id)
        {
            var vehiclesinformation = await _context.Vehiclesinformations.FindAsync(id);
            if (vehiclesinformation == null)
            {
                return NotFound(new { STS = 0 });
            }

            _context.Vehiclesinformations.Remove(vehiclesinformation);
            await _context.SaveChangesAsync();

            return Ok(new { STS = 1 });
        }

        private bool VehiclesinformationExists(long id)
        {
            return _context.Vehiclesinformations.Any(e => e.Id == id);
        }

        private System.Data.DataTable ConvertToDataTable(IEnumerable<Vehiclesinformation> vehiclesinformations)
        {
            System.Data.DataTable dt = new System.Data.DataTable("VehiclesInformations");

            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("VehicleId", typeof(long));
            dt.Columns.Add("DriverId", typeof(long));
            dt.Columns.Add("VehicleMake", typeof(string));
            dt.Columns.Add("VehicleModel", typeof(string));
            dt.Columns.Add("PurchaseDate", typeof(long));

            foreach (var vehiclesinformation in vehiclesinformations)
            {
                dt.Rows.Add(vehiclesinformation.Id, vehiclesinformation.Vehicleid, vehiclesinformation.Driverid, vehiclesinformation.Vehiclemake, vehiclesinformation.Vehiclemodel, vehiclesinformation.Purchasedate);
            }

            return dt;
        }
    }
}