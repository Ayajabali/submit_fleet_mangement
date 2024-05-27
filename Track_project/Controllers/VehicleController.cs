using Track_project.Data;
using Track_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using FPro;

[Route("api/[controller]")]
[ApiController]
public class VehicleController : ControllerBase
{
    private readonly DemoContext2 _context;

    public VehicleController(DemoContext2 context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<GVAR>> GetAllVehicles()
    {
        try
        {
            // Get all vehicles
            var vehicles = await _context.Vehicles
                .Include(v => v.Vehiclesinformations)
                .Include(v => v.Routehistories)
                .ToListAsync();

            var gvar = new GVAR();
            DataTable dataTable = ConvertVehiclesToDataTable(vehicles);
            gvar.DicOfDT.TryAdd("Vehicles", dataTable);

            return Ok(new { STS = 1, gvar });
        }
        catch (Exception)
        {
            return StatusCode(500, new { STS = 0 });
        }
    }

    private DataTable ConvertVehiclesToDataTable(IEnumerable<Vehicle> vehicles)
    {
        DataTable dt = new DataTable("Vehicles");

        dt.Columns.Add("VehicleID", typeof(long));
        dt.Columns.Add("VehicleNumber", typeof(long));
        dt.Columns.Add("VehicleType", typeof(string));
        dt.Columns.Add("LastDirection", typeof(int));
        dt.Columns.Add("LastStatus", typeof(char));
        dt.Columns.Add("LastAddress", typeof(string));
        dt.Columns.Add("LastLatitude", typeof(float));
        dt.Columns.Add("LastLongitude", typeof(float));

        foreach (var vehicle in vehicles)
        {
            var latestRoute = vehicle.Routehistories?.OrderByDescending(r => r.Epoch).FirstOrDefault();
            dt.Rows.Add(
                vehicle.Vehicleid,
                vehicle.Vehiclenumber,
                vehicle.Vehicletype,
                latestRoute?.Vehicledirection,
                latestRoute?.Status,
                latestRoute?.Address,
                latestRoute?.Latitude,
                latestRoute?.Longitude
            );
        }

        return dt;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GVAR>> GetVehicle(long id)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Vehiclesinformations)
            .Include(v => v.Routehistories)
            .FirstOrDefaultAsync(v => v.Vehicleid == id);

        if (vehicle == null)
        {
            return NotFound(new { STS = 0 });
        }

        var gvar = new GVAR();

        var latestRoute = vehicle.Routehistories?.OrderByDescending(r => r.Epoch).FirstOrDefault();
        var vehicleDetails = new ConcurrentDictionary<string, string>
        {
            ["VehicleID"] = vehicle.Vehicleid.ToString(),
            ["VehicleNumber"] = vehicle.Vehiclenumber.ToString(),
            ["VehicleType"] = vehicle.Vehicletype.ToString(),
            ["LastDirection"] = latestRoute?.Vehicledirection.ToString(),
            ["LastStatus"] = latestRoute?.Status.ToString(),
            ["LastAddress"] = latestRoute?.Address,
            ["LastLatitude"] = latestRoute?.Latitude.ToString(),
            ["LastLongitude"] = latestRoute?.Longitude.ToString()
        };

        gvar.DicOfDic["Vehicle"] = vehicleDetails;

        return Ok(new { STS = 1, gvar });
    }

    [HttpPost]
    public async Task<ActionResult<GVAR>> AddVehicle([FromBody] dynamic jsonData)
    {
        var tags = jsonData.DicOfDic.Tags;
        var vehicleNumber = tags["VehicleNumber"];
        var vehicleType = tags["VehicleType"];

        var gvar = new GVAR();
        var vehicleDetails = new ConcurrentDictionary<string, string>
        {
            ["VehicleNumber"] = vehicleNumber,
            ["VehicleType"] = vehicleType
        };
        gvar.DicOfDic.TryAdd("Vehicle", vehicleDetails);

        var newVehicle = new Vehicle
        {
            Vehiclenumber = vehicleNumber,
            Vehicletype = vehicleType
        };

        try
        {
            _context.Vehicles.Add(newVehicle);
            await _context.SaveChangesAsync();

            var newId = newVehicle.Vehicleid;

            var locationUri = Url.Action(nameof(GetVehicle), new { id = newId });

            return Ok(new { STS = 1 });
        }
        catch (Exception)
        {
            return StatusCode(500, new { STS = 0 });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(long id, [FromBody] dynamic jsonData)
    {
        try
        {
            var tags = jsonData.DicOfDic.Tags;
            var vehicleID = (long)tags["vehicleID"];
            var vehicleNumber = (int)tags["vehicleNumber"];
            var vehicleType = (string)tags["vehicleType"];

            if (id != vehicleID)
            {
                return BadRequest(new { STS = 0 });
            }

            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                return NotFound(new { STS = 0 });
            }

            vehicle.Vehiclenumber = vehicleNumber;
            vehicle.Vehicletype = vehicleType;

            _context.Entry(vehicle).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var gvar = new GVAR();

            var vehicleDetails = new ConcurrentDictionary<string, string>
            {
                ["VehicleID"] = vehicle.Vehicleid.ToString(),
                ["VehicleNumber"] = vehicle.Vehiclenumber.ToString(),
                ["VehicleType"] = vehicle.Vehicletype
            };
            gvar.DicOfDic.TryAdd("Vehicle", vehicleDetails);

            return Ok(new { STS = 1 });
        }
        catch (Exception)
        {
            return StatusCode(500, new { STS = 0 });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound(new { STS = 0 });
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return Ok(new { STS = 1 });
    }
}
