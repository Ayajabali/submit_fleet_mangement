using FPro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Track_project.Data;
using Track_project.Models;

namespace Track_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RectanglegeofenceController : ControllerBase
    {
        private readonly DemoContext2 _context;

        public RectanglegeofenceController(DemoContext2 context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<GVAR>> Get()
        {
            try
            {
                var rectanglegeofences = await _context.Rectanglegeofences.ToListAsync();
                if (rectanglegeofences == null || !rectanglegeofences.Any())
                {
                    return NotFound("No rectangle geofences found.");
                }

                var gvar = new GVAR();
                gvar.DicOfDT.TryAdd("RectangleGeofences", ConvertToDataTable(rectanglegeofences));

                return Ok(new { STS = 1, gvar });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GVAR>> GetRectanglegeofence(long id)
        {
            try
            {
                var rectanglegeofence = await _context.Rectanglegeofences.FindAsync(id);
                if (rectanglegeofence == null)
                {
                    return NotFound(new { STS = 0 });
                }

                var gvar = new GVAR();
                var rectanglegeofenceDetails = new ConcurrentDictionary<string, string>
                {
                    ["Id"] = rectanglegeofence.Id.ToString(),
                    ["GeofenceId"] = rectanglegeofence.Geofenceid.ToString(),
                    ["North"] = rectanglegeofence.North.ToString(),
                    ["East"] = rectanglegeofence.East.ToString(),
                    ["West"] = rectanglegeofence.West.ToString(),
                    ["South"] = rectanglegeofence.South.ToString()
                };

                gvar.DicOfDic.TryAdd("RectangleGeofence", rectanglegeofenceDetails);

                return Ok(new { STS = 1, gvar});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] dynamic jsonData)
        {
            try
            {
                var tags = jsonData.DicOfDic.Tags;

                var rectanglegeofence = new Rectanglegeofence
                {
                    Id = id,
                    Geofenceid = Convert.ToInt64(tags["geofenceid"]),
                    North = Convert.ToSingle(tags["north"]),
                    East = Convert.ToSingle(tags["east"]),
                    West = Convert.ToSingle(tags["west"]),
                    South = Convert.ToSingle(tags["south"])
                };

                _context.Entry(rectanglegeofence).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var gvar = new GVAR();
                var rectanglegeofenceDetails = new ConcurrentDictionary<string, string>
                {
                    ["Id"] = rectanglegeofence.Id.ToString(),
                    ["GeofenceId"] = rectanglegeofence.Geofenceid.ToString(),
                    ["North"] = rectanglegeofence.North.ToString(),
                    ["East"] = rectanglegeofence.East.ToString(),
                    ["West"] = rectanglegeofence.West.ToString(),
                    ["South"] = rectanglegeofence.South.ToString()
                };

                gvar.DicOfDic.TryAdd("RectangleGeofence", rectanglegeofenceDetails);

                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }







        [HttpPost]
        public async Task<ActionResult<GVAR>> AddRectanglegeofence([FromBody] dynamic jsonData)
        {
            var tags = jsonData.DicOfDic.Tags;
            var geofenceId = tags["GeofenceId"];
            var north = tags["North"];
            var east = tags["East"];
            var west = tags["West"];
            var south = tags["South"];

            var gvar = new GVAR();
            var rectanglegeofenceDetails = new ConcurrentDictionary<string, string>
            {
                ["GeofenceId"] = geofenceId,
                ["North"] = north,
                ["East"] = east,
                ["West"] = west,
                ["South"] = south
            };
            gvar.DicOfDic.TryAdd("RectangleGeofence", rectanglegeofenceDetails);

            var newRectanglegeofence = new Rectanglegeofence
            {
                Geofenceid = Convert.ToInt64(geofenceId),
                North = Convert.ToSingle(north),
                East = Convert.ToSingle(east),
                West = Convert.ToSingle(west),
                South = Convert.ToSingle(south)
            };

            try
            {
                _context.Rectanglegeofences.Add(newRectanglegeofence);
                await _context.SaveChangesAsync();

                var newId = newRectanglegeofence.Id;

                var locationUri = Url.Action(nameof(GetRectanglegeofence), new { id = newId });

                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRectanglegeofence(long id)
        {
            try
            {
                var rectanglegeofence = await _context.Rectanglegeofences.FindAsync(id);
                if (rectanglegeofence == null)
                {
                    return NotFound(new { STS = 0 });
                }

                _context.Rectanglegeofences.Remove(rectanglegeofence);
                await _context.SaveChangesAsync();

                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }



        private DataTable ConvertToDataTable(IEnumerable<Rectanglegeofence> rectanglegeofences)
        {
            DataTable dt = new DataTable("RectangleGeofences");

            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("GeofenceId", typeof(int));
            dt.Columns.Add("North", typeof(float));
            dt.Columns.Add("East", typeof(float));
            dt.Columns.Add("West", typeof(float));
            dt.Columns.Add("South", typeof(float));

            foreach (var rectanglegeofence in rectanglegeofences)
            {
                dt.Rows.Add(rectanglegeofence.Id, rectanglegeofence.Geofenceid, rectanglegeofence.North, rectanglegeofence.East, rectanglegeofence.West, rectanglegeofence.South);
            }

            return dt;
        }
    }
}
