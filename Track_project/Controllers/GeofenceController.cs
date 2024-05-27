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
    public class GeofenceController : ControllerBase
    {
        private readonly DemoContext2 _context;

        public GeofenceController(DemoContext2 context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<GVAR>> GetAllGeofences()
        {
            try
            {
                var geofences = await _context.Geofences.ToListAsync();
                if (geofences == null || !geofences.Any())
                {
                    return NotFound(new { STS = 0 });
                }

                var gvar = new GVAR();
                var dataTable = ConvertToDataTable(geofences);
                gvar.DicOfDT.TryAdd("Geofences", dataTable);

                return Ok(new { STS = 1, gvar });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GVAR>> GetGeofence(long id)
        {
            try
            {
                var geofence = await _context.Geofences.FindAsync(id);

                if (geofence == null)
                {
                    return NotFound(new { STS = 0 });
                }

                var gvar = new GVAR();
                var geofenceDetails = new ConcurrentDictionary<string, string>
                {
                    ["GeofenceID"] = geofence.Geofenceid.ToString(),
                    ["GeofenceType"] = geofence.Geofencetype,
                    ["AddedDate"] = geofence.Addeddate.HasValue
                                    ? DateTimeOffset.FromUnixTimeMilliseconds(geofence.Addeddate.Value).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                                    : "N/A",
                    ["StrokeColor"] = geofence.Strokecolor,
                    ["StrokeOpacity"] = geofence.Strokeopacity.ToString(),
                    ["StrokeWeight"] = geofence.Strokeweight.ToString(),
                    ["FillColor"] = geofence.Fillcolor,
                    ["FillOpacity"] = geofence.Fillopacity.ToString()
                };

                gvar.DicOfDic.TryAdd("Geofence", geofenceDetails);

                return Ok(new { STS = 1, gvar });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        [HttpPost]
        public async Task<ActionResult<GVAR>> AddGeofence([FromBody] dynamic jsonData)
        {
            try
            {
                var tags = jsonData.DicOfDic.Tags;
                var geofenceType = tags["Geofencetype"];
                var addedDateMillis = (long)tags["Addeddate"];
                var addedDate = DateTimeOffset.FromUnixTimeMilliseconds(addedDateMillis).UtcDateTime;
                var strokeColor = tags["Strokecolor"];
                var strokeOpacity = float.Parse(tags["Strokeopacity"].ToString());
                var strokeWidth = float.Parse(tags["Strokeweight"].ToString());
                var fillColor = tags["Fillcolor"];
                var fillOpacity = float.Parse(tags["Fillopacity"].ToString());

                var gvar = new GVAR();
                var geofenceDetails = new ConcurrentDictionary<string, string>
                {
                    ["GeofenceType"] = geofenceType,
                    ["AddedDate"] = addedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ["StrokeColor"] = strokeColor,
                    ["StrokeOpacity"] = strokeOpacity.ToString(),
                    ["StrokeWeight"] = strokeWidth.ToString(),
                    ["FillColor"] = fillColor,
                    ["FillOpacity"] = fillOpacity.ToString()
                };
                gvar.DicOfDic.TryAdd("Geofence", geofenceDetails);

                var geofence = new Geofence
                {
                    Geofencetype = geofenceType,
                    Addeddate = addedDateMillis,
                    Strokecolor = strokeColor,
                    Strokeopacity = strokeOpacity,
                    Strokeweight = strokeWidth,
                    Fillcolor = fillColor,
                    Fillopacity = fillOpacity
                };

                _context.Geofences.Add(geofence);
                await _context.SaveChangesAsync();

                var locationUri = Url.Action(nameof(GetGeofence), new { id = geofence.Geofenceid });

                return Created(locationUri, new { STS = 1, gvar });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGeofence(long id)
        {
            var geofence = await _context.Geofences.FindAsync(id);
            if (geofence == null)
            {
                return NotFound(new { STS = 0 });
            }

            _context.Geofences.Remove(geofence);
            await _context.SaveChangesAsync();
            return Ok(new { STS = 1 });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGeofence(long id, [FromBody] dynamic jsonData)
        {
            try
            {
                var tags = jsonData.DicOfDic.Tags;

                if (tags == null || id != (long)tags["GeofenceID"])
                {
                    return BadRequest(new { STS = 0 });
                }

                var existingGeofence = await _context.Geofences.FindAsync(id);
                if (existingGeofence == null)
                {
                    return NotFound(new { STS = 0 });
                }

                existingGeofence.Geofencetype = tags["Geofencetype"];
                existingGeofence.Addeddate = (long)tags["Addeddate"];
                existingGeofence.Strokecolor = tags["Strokecolor"];
                existingGeofence.Strokeopacity = float.Parse(tags["Strokeopacity"].ToString());
                existingGeofence.Strokeweight = float.Parse(tags["Strokeweight"].ToString());
                existingGeofence.Fillcolor = tags["Fillcolor"];
                existingGeofence.Fillopacity = float.Parse(tags["Fillopacity"].ToString());

                _context.Entry(existingGeofence).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var gvar = new GVAR();
                var geofenceDetails = new ConcurrentDictionary<string, string>
                {
                    ["GeofenceID"] = existingGeofence.Geofenceid.ToString(),
                    ["GeofenceType"] = existingGeofence.Geofencetype,
                    ["AddedDate"] = DateTimeOffset.FromUnixTimeMilliseconds(existingGeofence.Addeddate.Value).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ["StrokeColor"] = existingGeofence.Strokecolor,
                    ["StrokeOpacity"] = existingGeofence.Strokeopacity.ToString(),
                    ["StrokeWeight"] = existingGeofence.Strokeweight.ToString(),
                    ["FillColor"] = existingGeofence.Fillcolor,
                    ["FillOpacity"] = existingGeofence.Fillopacity.ToString()
                };

                gvar.DicOfDic.TryAdd("Geofence", geofenceDetails);

                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }


        private DataTable ConvertToDataTable(IEnumerable<Geofence> geofences)
        {
            DataTable dt = new DataTable("Geofences");

            dt.Columns.Add("GeofenceID", typeof(long));
            dt.Columns.Add("GeofenceType", typeof(string));
            dt.Columns.Add("AddedDate", typeof(long));
            dt.Columns.Add("StrokeColor", typeof(string));
            dt.Columns.Add("StrokeOpacity", typeof(float));
            dt.Columns.Add("StrokeWeight", typeof(float));
            dt.Columns.Add("FillColor", typeof(string));
            dt.Columns.Add("FillOpacity", typeof(float));

            foreach (var geofence in geofences)
            {
                dt.Rows.Add(
                    geofence.Geofenceid,
                    geofence.Geofencetype,
                    geofence.Addeddate ?? 0, // Default to 0 if null
                    geofence.Strokecolor,
                    geofence.Strokeopacity,
                    geofence.Strokeweight,
                    geofence.Fillcolor,
                    geofence.Fillopacity
                );
            }
            return dt;
        }
    }
}
