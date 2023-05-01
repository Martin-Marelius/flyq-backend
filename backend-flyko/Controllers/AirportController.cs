using backend_flyko.Base.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Xml;

namespace backend_flyko.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AirportController : ControllerBase
    {

        private readonly ILogger<AirportController> _logger;
        private readonly IData data;

        private static HttpClient sharedClient => new();

        public AirportController(ILogger<AirportController> logger, IData data)
        {

            _logger = logger;
            this.data = data;
        }

        [HttpGet(Name = "GetAirportBusyness/{airport}")]
        public async Task<ActionResult<string>> Get([FromHeader] string airport)
        {
            FlightsData airportData = new FlightsData();

            var venue = data.Venues.Where(v => v.Tag == airport).FirstOrDefault();

            var flights = await airportData.GetNumberOfFlights(sharedClient, venue.Tag);
            var flightsNext = await airportData.GetNumberOfFlightsNext3Hours(sharedClient, venue.Tag);
            var scheduleTimes = await airportData.GetFlightScheduleTimes(sharedClient, venue.Tag);
            var dayRaw = await GetDayRaw(sharedClient, venue);
            var liveBusyness = await GetLiveBusyness(sharedClient, venue);

            // TODO fill in DayRaw and LiveForecast
            var response = new ResponseItem() 
            {
                Tag = venue.Tag,
                Flights = flights,
                FlightsNext = flightsNext,
                ScheduleTimes = scheduleTimes,
                DayRaw = dayRaw,
                LiveForecast = liveBusyness
            };

            var toReturn = JsonConvert.SerializeObject(response);
            return Ok(toReturn);
        }

        private static Dictionary<string, Tuple<int[], DateTime>> dayRawCache = new Dictionary<string, Tuple<int[], DateTime>>();
        private static Dictionary<string, Tuple<int, DateTime>> liveBusynessCache = new Dictionary<string, Tuple<int, DateTime>>();

        private async Task<int[]> GetDayRaw(HttpClient httpClient, Data.Airport venue)
        {
            if (!dayRawCache.ContainsKey(venue.Tag))
            {
                dayRawCache[venue.Tag] = Tuple.Create(new int[24], DateTime.Now.AddDays(-1));
            }

            // Check if it has been 1 hour since the value was last updated
            if (DateTime.Now - dayRawCache[venue.Tag].Item2 > TimeSpan.FromHours(1))
            {
                var queryParams = HttpUtility.ParseQueryString(string.Empty);
                queryParams["api_key_private"] = "pri_2aac4112dc504e249a6477dbf023a470";
                queryParams["venue_id"] = venue.Id;

                var baseUrl = "https://besttime.app/api/v1/forecasts";
                var requestUrl = $"{baseUrl}?{queryParams}";

                var response = await httpClient.PostAsync(requestUrl, null);
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = Newtonsoft.Json.Linq.JObject.Parse(responseContent);

                if (responseJson["status"].ToString() != "OK")
                {
                    throw new Exception("API returned non-OK status");
                }

                var dayRawArray = responseJson["analysis"][0]["day_raw"];
                var dayRaw = dayRawArray.ToObject<int[]>();

                dayRawCache[venue.Tag] = Tuple.Create(dayRaw, DateTime.Now);

                return dayRaw;
            }
            else
            {
                // The value has been updated within the last hour
                return dayRawCache[venue.Tag].Item1;
            }
        }

        private async Task<int> GetLiveBusyness(HttpClient httpClient, Data.Airport venue)
        {
            if (!liveBusynessCache.ContainsKey(venue.Tag))
            {
                liveBusynessCache[venue.Tag] = Tuple.Create(0, DateTime.Now.AddDays(-1));
            }

            // Check if it has been 1 hour since the value was last updated
            if (DateTime.Now - liveBusynessCache[venue.Tag].Item2 > TimeSpan.FromHours(1))
            {
                var queryParams = HttpUtility.ParseQueryString(string.Empty);
                queryParams["api_key_private"] = "pri_2aac4112dc504e249a6477dbf023a470";
                queryParams["venue_id"] = venue.Id;

                var baseUrl = "https://besttime.app/api/v1/forecasts/live";
                var requestUrl = $"{baseUrl}?{queryParams}";

                var response = await httpClient.PostAsync(requestUrl, null);
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = Newtonsoft.Json.Linq.JObject.Parse(responseContent);

                if (responseJson["status"].ToString() != "OK")
                {
                    throw new Exception("API returned non-OK status");
                }

                var dayRawArray = responseJson["analysis"]["venue_forecasted_busyness"];
                var dayRaw = dayRawArray.ToObject<int>();

                liveBusynessCache[venue.Tag] = Tuple.Create(dayRaw, DateTime.Now);

                return dayRaw;
            }
            else
            {
                // The value has been updated within the last hour
                return liveBusynessCache[venue.Tag].Item1;
            }
        }

        public class ResponseItem
        {
            public string Tag;
            public int[] DayRaw;
            public int LiveForecast;
            public int Flights;
            public int FlightsNext;
            public List<string> ScheduleTimes;
        }
    }
}