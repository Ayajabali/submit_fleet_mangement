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

namespace Track_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly DemoContext2 _context;

        public DriverController(DemoContext2 context)
        {
            _context = context;
        }

        // GET: api/Driver
        [HttpGet]
        public async Task<ActionResult<GVAR>> GetAllDrivers()
        {
            try
            {
                var drivers = await _context.Drivers.ToListAsync();
                if (drivers == null || !drivers.Any())
                {
                    return NotFound("No drivers found.");
                }

                var gvar = new GVAR();

                DataTable dataTable = ConvertToDataTable(drivers);

                gvar.DicOfDT.TryAdd("Drivers", dataTable);

                return Ok(new { STS = 1, gvar});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }
        private DataTable ConvertToDataTable(IEnumerable<Driver> drivers)
        {
            DataTable dt = new DataTable("Drivers");

            dt.Columns.Add("DriverID", typeof(int));
            dt.Columns.Add("DriverName", typeof(string));
            dt.Columns.Add("PhoneNumber", typeof(long));

            foreach (var driver in drivers)
            {
                var phoneNumber = driver.Phonenumber.HasValue ? driver.Phonenumber.Value : (object)DBNull.Value;
                dt.Rows.Add(driver.Driverid, driver.Drivername, phoneNumber);
            }
            return dt;
        }


        // GET: api/Driver/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GVAR>> GetDriver(long id)
        {
            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
            {
                return NotFound(new { STS = 0 });
            }

            var gvar = new GVAR();
            gvar.DicOfDic["Driver"] = new ConcurrentDictionary<string, string>
            {
                ["DriverID"] = driver.Driverid.ToString(),
                ["DriverName"] = driver.Drivername,
                ["PhoneNumber"] = driver.Phonenumber.HasValue ? driver.Phonenumber.Value.ToString() : ""
            };

            return Ok(new { STS = 1, gvar});
        }

        // POST: api/Driver
        [HttpPost]
        public async Task<ActionResult<GVAR>> AddDriver([FromBody] GVAR jsonData)
        {
            try
            {
                var tags = jsonData.DicOfDic["Tags"];
                var driverName = tags["DriverName"];
                var phoneNumber = long.Parse(tags["PhoneNumber"]);

                var gvar = new GVAR();
                var driverDetails = new ConcurrentDictionary<string, string>
                {
                    ["DriverName"] = driverName,
                    ["PhoneNumber"] = phoneNumber.ToString()
                };
                gvar.DicOfDic = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
                gvar.DicOfDic.TryAdd("Driver", driverDetails);

                var newDriver = new Driver
                {
                    Drivername = driverName,
                    Phonenumber = phoneNumber
                };

                _context.Drivers.Add(newDriver);
                await _context.SaveChangesAsync();

                var newId = newDriver.Driverid; // Assuming Driverid is the ID property of your Driver entity
                var locationUri = Url.Action(nameof(GetDriver), new { id = newId });

                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutDriver(long id, [FromBody] dynamic jsonData)
        {
            try
            {
                // Extract the Tags dictionary from the jsonData
                var tags = jsonData?.DicOfDic?.Tags;

                if (tags == null || id != (long)tags.Driverid)
                {
                    return BadRequest(new { STS = 0, Message = "Invalid driver ID or data format." });
                }

                // Find the existing driver in the database
                var existingDriver = await _context.Drivers.FindAsync(id);
                if (existingDriver == null)
                {
                    return NotFound(new { STS = 0, Message = "Driver not found." });
                }

                // Update the existing driver's properties
                existingDriver.Drivername = tags.Drivername;
                existingDriver.Phonenumber = long.TryParse(tags.Phonenumber.ToString(), out long phoneNumber) ? (long?)phoneNumber : null;

                // Mark the entity as modified
                _context.Entry(existingDriver).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Prepare the response
                var gvar = new GVAR();
                var driverDetails = new ConcurrentDictionary<string, string>
                {
                    ["DriverID"] = existingDriver.Driverid.ToString(),
                    ["Drivername"] = existingDriver.Drivername,
                    ["Phonenumber"] = existingDriver.Phonenumber.HasValue ? existingDriver.Phonenumber.Value.ToString() : ""
                };
                gvar.DicOfDic.TryAdd("Driver", driverDetails);

                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0, Message = "An error occurred while updating the driver." });
            }
        }



        // DELETE: api/Driver/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(long id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound(new { STS = 0 });
            }

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();

            return Ok(new { STS = 1 });
        }
    }
}
