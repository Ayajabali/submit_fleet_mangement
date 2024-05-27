using FPro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Track_project.Data;
using Track_project.Models;

namespace Track_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CirclegeofenceController : ControllerBase
    {
        private readonly DemoContext2 _context;

        public CirclegeofenceController(DemoContext2 context)
        {
            _context = context;
        }

        // GET: api/Circlegeofence
        [HttpGet]
        public async Task<ActionResult<GVAR>> GetCirclegeofences()
        {
            try
            {
                var circlegeofences = await _context.Circlegeofences.ToListAsync();
                if (circlegeofences == null || !circlegeofences.Any())
                {
                    return NotFound("No circlegeofences found.");
                }

                var gvar = new GVAR();
                var dataTable = ConvertToDataTable(circlegeofences);
                gvar.DicOfDT.TryAdd("Circlegeofences", dataTable);

                return Ok(new { STS = 1, gvar });
            }
            catch (Exception)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        // GET: api/Circlegeofence/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GVAR>> GetCirclegeofence(long id)
        {
            var circlegeofence = await _context.Circlegeofences.FindAsync(id);

            if (circlegeofence == null)
            {
                return NotFound(new { STS = 0 });
            }

            var gvar = new GVAR();
            var circlegeofenceDetails = new ConcurrentDictionary<string, string>
            {
                ["Id"] = circlegeofence.Id.ToString(),
                ["Radius"] = circlegeofence.Radius.ToString(),
                ["Latitude"] = circlegeofence.Latitude.ToString(),
                ["Longitude"] = circlegeofence.Longitude.ToString(),
                ["GeofenceID"] = circlegeofence.Geofenceid.ToString()
            };

            gvar.DicOfDic["Circlegeofence"] = circlegeofenceDetails;

            return Ok(new { STS = 1, gvar });
        }

        // POST: api/Circlegeofence
        [HttpPost]
        public async Task<ActionResult<GVAR>> PostCirclegeofence([FromBody] dynamic jsonData)
        {
            try
            {
                var tags = jsonData.DicOfDic.Tags;
                var radius = (long)tags["Radius"];
                var latitude = (float)tags["Latitude"];
                var longitude = (float)tags["Longitude"];
                var geofenceId = (long)tags["GeofenceID"];

                var newCirclegeofence = new Circlegeofence
                {
                    Radius = radius,
                    Latitude = latitude,
                    Longitude = longitude,
                    Geofenceid = geofenceId
                };

                _context.Circlegeofences.Add(newCirclegeofence);
                await _context.SaveChangesAsync();

                var gvar = new GVAR();
                var circlegeofenceDetails = new ConcurrentDictionary<string, string>
                {
                    ["Id"] = newCirclegeofence.Id.ToString(),
                    ["Radius"] = newCirclegeofence.Radius.ToString(),
                    ["Latitude"] = newCirclegeofence.Latitude.ToString(),
                    ["Longitude"] = newCirclegeofence.Longitude.ToString(),
                    ["GeofenceID"] = newCirclegeofence.Geofenceid.ToString()
                };

                gvar.DicOfDic.TryAdd("Circlegeofence", circlegeofenceDetails);

                var locationUri = Url.Action(nameof(GetCirclegeofence), new { id = newCirclegeofence.Id });

                return Created(locationUri, new { STS = 1, gvar });
            }
            catch (Exception)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        // PUT: api/Circlegeofence/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCirclegeofence(long id, [FromBody] dynamic jsonData)
        {
            try
            {
                var tags = jsonData.DicOfDic.Tags;
                var radius = (long)tags["Radius"];
                var latitude = (float)tags["Latitude"];
                var longitude = (float)tags["Longitude"];
                var geofenceId = (long)tags["GeofenceID"];

                var existingCirclegeofence = await _context.Circlegeofences.FindAsync(id);
                if (existingCirclegeofence == null)
                {
                    return NotFound(new { STS = 0 });
                }

                existingCirclegeofence.Radius = radius;
                existingCirclegeofence.Latitude = latitude;
                existingCirclegeofence.Longitude = longitude;
                existingCirclegeofence.Geofenceid = geofenceId;

                _context.Entry(existingCirclegeofence).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var gvar = new GVAR();
                var circlegeofenceDetails = new ConcurrentDictionary<string, string>
                {
                    ["Id"] = existingCirclegeofence.Id.ToString(),
                    ["Radius"] = existingCirclegeofence.Radius.ToString(),
                    ["Latitude"] = existingCirclegeofence.Latitude.ToString(),
                    ["Longitude"] = existingCirclegeofence.Longitude.ToString(),
                    ["GeofenceID"] = existingCirclegeofence.Geofenceid.ToString()
                };

                gvar.DicOfDic.TryAdd("Circlegeofence", circlegeofenceDetails);

                return Ok(new { STS = 1 });
            }
            catch (Exception)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        // DELETE: api/Circlegeofence/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCirclegeofence(long id)
        {
            var circlegeofence = await _context.Circlegeofences.FindAsync(id);
            if (circlegeofence == null)
            {
                return NotFound(new { STS = 0 });
            }

            _context.Circlegeofences.Remove(circlegeofence);
            await _context.SaveChangesAsync();
            return Ok(new { STS = 1 });
        }

        private bool CirclegeofenceExists(long id)
        {
            return _context.Circlegeofences.Any(e => e.Id == id);
        }

        private System.Data.DataTable ConvertToDataTable(IEnumerable<Circlegeofence> circlegeofences)
        {
            var dt = new System.Data.DataTable("Circlegeofences");
            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("Radius", typeof(long));
            dt.Columns.Add("Latitude", typeof(float));
            dt.Columns.Add("Longitude", typeof(float));
            dt.Columns.Add("GeofenceID", typeof(long));

            foreach (var circlegeofence in circlegeofences)
            {
                dt.Rows.Add(circlegeofence.Id, circlegeofence.Radius, circlegeofence.Latitude, circlegeofence.Longitude, circlegeofence.Geofenceid);
            }

            return dt;
        }
    }
}
