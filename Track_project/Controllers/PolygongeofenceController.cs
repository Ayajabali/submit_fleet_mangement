using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FPro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Track_project.Data;
using Track_project.Models;

namespace Track_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolygongeofenceController : ControllerBase
    {
        private readonly DemoContext2 _context;

        public PolygongeofenceController(DemoContext2 context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<GVAR>> GetAllPolygongeofences()
        {
            try
            {
                var polygongeofences = await _context.Polygongeofences.ToListAsync();
                if (polygongeofences == null || !polygongeofences.Any())
                {
                    return NotFound("No polygongeofences found.");
                }

                var gvar = new GVAR();
                gvar.DicOfDT.TryAdd("Polygongeofences", ConvertToDataTable(polygongeofences));

                return Ok(new { STS = 1, gvar});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GVAR>> GetPolygongeofence(long id)
        {
            var polygongeofence = await _context.Polygongeofences.FindAsync(id);

            if (polygongeofence == null)
            {
                return NotFound(new { STS = 0 });
            }

            var gvar = new GVAR();
            var polygongeofenceDetails = new ConcurrentDictionary<string, string>
            {
                ["Id"] = polygongeofence.Id.ToString(),
                ["GeofenceId"] = polygongeofence.Geofenceid?.ToString() ?? "N/A",
                ["Latitude"] = polygongeofence.Latitude?.ToString() ?? "N/A",
                ["Longitude"] = polygongeofence.Longitude?.ToString() ?? "N/A"
            };

            gvar.DicOfDic.TryAdd("Polygongeofence", polygongeofenceDetails);

            return Ok(new { STS = 1, gvar });
        }

        [HttpPost]
        public async Task<ActionResult<GVAR>> AddPolygongeofence([FromBody] dynamic jsonData)
        {
            var tags = jsonData.DicOfDic.Tags;
            var geofenceId = tags["GeofenceId"];
            var latitude = tags["Latitude"];
            var longitude = tags["Longitude"];

            var gvar = new GVAR();
            var polygongeofenceDetails = new ConcurrentDictionary<string, string>
            {
                ["GeofenceId"] = geofenceId,
                ["Latitude"] = latitude,
                ["Longitude"] = longitude
            };
            gvar.DicOfDic.TryAdd("Polygongeofence", polygongeofenceDetails);

            try
            {
                var newPolygongeofence = new Polygongeofence
                {
                    Geofenceid = geofenceId,
                    Latitude = latitude,
                    Longitude = longitude
                };

                _context.Polygongeofences.Add(newPolygongeofence);
                await _context.SaveChangesAsync();

                var locationUri = Url.Action(nameof(GetPolygongeofence), new { id = newPolygongeofence.Id });

                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePolygongeofence(long id, [FromBody] dynamic jsonData)
        {
            try
            {
                var tags = jsonData.DicOfDic.Tags;

                if (tags == null || id != (long)tags["Id"])
                {
                    return BadRequest(new { STS = 0 });
                }

                var existingPolygongeofence = await _context.Polygongeofences.FindAsync(id);
                if (existingPolygongeofence == null)
                {
                    return NotFound(new { STS = 0 });
                }

                existingPolygongeofence.Geofenceid = tags["GeofenceId"];
                existingPolygongeofence.Latitude = tags["Latitude"];
                existingPolygongeofence.Longitude = tags["Longitude"];

                _context.Entry(existingPolygongeofence).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var gvar = new GVAR();
                var polygongeofenceDetails = new ConcurrentDictionary<string, string>
                {
                    ["Id"] = existingPolygongeofence.Id.ToString(),
                    ["GeofenceId"] = existingPolygongeofence.Geofenceid.ToString(),
                    ["Latitude"] = existingPolygongeofence.Latitude.ToString(),
                    ["Longitude"] = existingPolygongeofence.Longitude.ToString()
                };

                gvar.DicOfDic.TryAdd("Polygongeofence", polygongeofenceDetails);

                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePolygongeofence(long id)
        {
            var polygongeofence = await _context.Polygongeofences.FindAsync(id);
            if (polygongeofence == null)
            {
                return NotFound(new { STS = 0 });
            }

            try
            {
                _context.Polygongeofences.Remove(polygongeofence);
                await _context.SaveChangesAsync();
                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }


        private DataTable ConvertToDataTable(IEnumerable<Polygongeofence> polygongeofences)
        {
            DataTable dt = new DataTable("Polygongeofences");

            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("GeofenceId", typeof(long));
            dt.Columns.Add("Latitude", typeof(float));
            dt.Columns.Add("Longitude", typeof(float));

            foreach (var polygongeofence in polygongeofences)
            {
                dt.Rows.Add(polygongeofence.Id, polygongeofence.Geofenceid ?? 0, polygongeofence.Latitude ?? 0, polygongeofence.Longitude ?? 0);
            }
            return dt;
        }
    }
}
