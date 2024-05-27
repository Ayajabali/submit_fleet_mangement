using FPro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
    public class RoutehistoryController : ControllerBase
    {
        private readonly DemoContext2 _context;
        private readonly WebSocketManagerService _webSocketManagerService;



        public RoutehistoryController(DemoContext2 context, WebSocketManagerService webSocketManagerService)
        {
            _context = context;
            _webSocketManagerService = webSocketManagerService;

        }
     

        [HttpGet]
        public async Task<ActionResult<GVAR>> GetAllRouteHistory()
        {
            try
            {
                var routeHistories = await _context.Routehistories.ToListAsync();
                if (routeHistories == null || !routeHistories.Any())
                {
                    return NotFound("No route histories found.");
                }

                var gvar = new GVAR();
                var dataTable = ConvertToDataTable(routeHistories);
                gvar.DicOfDT.TryAdd("RouteHistories", dataTable);

                return Ok(new { STS = 1, gvar });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }


        [HttpGet("{vehicleId}/{startTimeEpoch}/{endTimeEpoch}")]
        public async Task<ActionResult<Dictionary<string, DataTable>>> RetrieveRouteHistory(long vehicleId, long startTimeEpoch, long endTimeEpoch)
        {
            try
            {var historyEntries = await _context.Routehistories .Include(r => r.Vehicle).Where(r => r.Vehicleid == vehicleId && r.Epoch >= startTimeEpoch && r.Epoch <= endTimeEpoch).ToListAsync();
                if (historyEntries == null || !historyEntries.Any()) { return NotFound(new { sts = 0 }); }
                var routeHistoryTable = CreateRouteHistoryTable();
                foreach (var entry in historyEntries){AddRouteHistoryRow(routeHistoryTable, entry);}
                var gvar = new GVAR();
                gvar.DicOfDT["RouteHistory"] = routeHistoryTable;
                return Ok(new { STS = 1, gvar }); }
            catch (Exception ex) {return StatusCode(500, new { STS = 0 }); } }
        private DataTable CreateRouteHistoryTable()
        {var dataTable = new DataTable("RouteHistory");
            dataTable.Columns.Add("VehicleID");
            dataTable.Columns.Add("VehicleNumber");
            dataTable.Columns.Add("Address");
            dataTable.Columns.Add("Status");
            dataTable.Columns.Add("Latitude");
            dataTable.Columns.Add("Longitude");
            dataTable.Columns.Add("VehicleDirection");
            dataTable.Columns.Add("GPSSpeed");
            dataTable.Columns.Add("GPSTime");
            return dataTable;}
        private void AddRouteHistoryRow(DataTable table, Routehistory entry)
        {
            table.Rows.Add(entry.Vehicleid, entry.Vehicle.Vehiclenumber, entry.Address, entry.Status,
                           entry.Latitude, entry.Longitude, entry.Vehicledirection,
                           entry.Vehiclespeed, entry.Epoch);
        }








        [HttpPost]
        public async Task<ActionResult<Routehistory>> PostRoutehistory(GVAR routehistoryObj)
        {
            try
            {
                var routeHistoryJson = JsonConvert.SerializeObject(routehistoryObj.DicOfDic["Tags"]);
                var routeHistory = JsonConvert.DeserializeObject<Routehistory>(routeHistoryJson);
                _context.Routehistories.Add(routeHistory);
                var broadcastMessage = JsonConvert.SerializeObject(new { Status = "Added", RouteHistory = routeHistory });
                await _context.SaveChangesAsync();
                _webSocketManagerService.Broadcast(broadcastMessage);  // Broadcast to all connected clients



                return CreatedAtAction("GetRoutehistory", new { id = routeHistory.Routehistoryid }, new { sts = 1 });

            }
            catch
            {
                return BadRequest(new { sts = 0 });
            }




        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRouteHistory(long id)
        {
            var routeHistory = await _context.Routehistories.FindAsync(id);
            if (routeHistory == null)
            {
                return NotFound(new { STS = 0 });
            }

            _context.Routehistories.Remove(routeHistory);
            await _context.SaveChangesAsync();
            return Ok(new { STS = 1 });
        }



        // PUT: api/Routehistory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRouteHistory(long id, [FromBody] dynamic jsonData)
        {
            try
            {
                var tags = jsonData.DicOfDic.Tags;
                var routeHistory = await _context.Routehistories.FindAsync(id);
                if (routeHistory == null)
                {
                    return NotFound(new { STS = 0 });
                }

                routeHistory.Vehicleid = Convert.ToInt64(tags["Vehicleid"]);
                routeHistory.Vehicledirection = Convert.ToInt32(tags["Vehicledirection"]);
                routeHistory.Status = tags["Status"];
                routeHistory.Vehiclespeed = tags["Vehiclespeed"];
                routeHistory.Epoch = Convert.ToInt64(tags["Epoch"]);
                routeHistory.Address = tags["Address"];
                routeHistory.Latitude = Convert.ToSingle(tags["Latitude"]);
                routeHistory.Longitude = Convert.ToSingle(tags["Longitude"]);

                _context.Entry(routeHistory).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var gvar = new GVAR();
                var routeHistoryDetails = new ConcurrentDictionary<string, string>
                {
                    ["RouteHistoryID"] = routeHistory.Routehistoryid.ToString(),
                    ["VehicleID"] = routeHistory.Vehicleid.ToString(),
                    ["VehicleDirection"] = routeHistory.Vehicledirection.ToString(),
                    ["Status"] = routeHistory.Status.ToString(),
                    ["VehicleSpeed"] = routeHistory.Vehiclespeed,
                    ["Epoch"] = routeHistory.Epoch.ToString(),
                    ["Address"] = routeHistory.Address,
                    ["Latitude"] = routeHistory.Latitude.ToString(),
                    ["Longitude"] = routeHistory.Longitude.ToString()
                };

                gvar.DicOfDic.TryAdd("RouteHistory", routeHistoryDetails);

                return Ok(new { STS = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { STS = 0 });
            }
        }



        private DataTable ConvertToDataTable(IEnumerable<Routehistory> routeHistories)
        {
            DataTable dt = new DataTable("RouteHistories");

            dt.Columns.Add("RouteHistoryID", typeof(long));
            dt.Columns.Add("VehicleID", typeof(long));
            dt.Columns.Add("VehicleDirection", typeof(int));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("VehicleSpeed", typeof(string));
            dt.Columns.Add("Epoch", typeof(long));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("Latitude", typeof(float));
            dt.Columns.Add("Longitude", typeof(float));

            foreach (var routeHistory in routeHistories)
            {
                dt.Rows.Add(routeHistory.Routehistoryid, routeHistory.Vehicleid, routeHistory.Vehicledirection, routeHistory.Status, routeHistory.Vehiclespeed, routeHistory.Epoch, routeHistory.Address, routeHistory.Latitude, routeHistory.Longitude);
            }

            return dt;
        }
    }
}