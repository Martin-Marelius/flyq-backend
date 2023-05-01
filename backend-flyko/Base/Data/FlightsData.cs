using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Linq;

namespace backend_flyko.Base.Data
{

    public class FlightsData
    {
        private static Dictionary<string, Tuple<int, DateTime>> flightsCache = new Dictionary<string, Tuple<int, DateTime>>();
        private static Dictionary<string, Tuple<int, DateTime>> next3HoursCache = new Dictionary<string, Tuple<int, DateTime>>();
        private static Dictionary<string, Tuple<List<string>, DateTime>> scheduleCache = new Dictionary<string, Tuple<List<string>, DateTime>>();

        public async Task<int> GetNumberOfFlights(HttpClient httpClient, string tag)
        {
            if (!flightsCache.ContainsKey(tag))
            {
                flightsCache[tag] = Tuple.Create(0, DateTime.Now.AddDays(-1));
            }

            // Check if it has been 1 hour since the value was last updated
            if (DateTime.Now - flightsCache[tag].Item2 > TimeSpan.FromHours(1))
            {
                // The value has not been updated in 1 hour
                // Do something here, such as updating the value
                using HttpResponseMessage flightData = await httpClient.GetAsync(
                    $"https://flydata.avinor.no/XmlFeed.asp?airport={tag}");

                string xmlString = await flightData.Content.ReadAsStringAsync();

                // Create an XmlReader from the string
                XmlReader xmlReader = XmlReader.Create(new StringReader(xmlString));

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                // Get the list of <flight> elements
                XmlNodeList flightNodes = xmlDoc.GetElementsByTagName("flight");

                // Get the length of the list
                int numFlights = flightNodes.Count;

                // Print the length to the console
                Console.WriteLine("Number of flights: " + numFlights);

                flightsCache[tag] = Tuple.Create(numFlights, DateTime.Now);

                return numFlights;
            }
            else
            {
                // The value has been updated within the last hour
                return flightsCache[tag].Item1;
            }
        }

        public async Task<int> GetNumberOfFlightsNext3Hours(HttpClient httpClient, string tag)
        {
            if (!next3HoursCache.ContainsKey(tag))
            {
                next3HoursCache[tag] = Tuple.Create(0, DateTime.Now.AddDays(-1));
            }

            // Check if it has been 1 hour since the value was last updated
            if (DateTime.Now - next3HoursCache[tag].Item2 > TimeSpan.FromHours(1))
            {
                // The value has not been updated in 1 hour
                // Do something here, such as updating the value
                using HttpResponseMessage flightData = await httpClient.GetAsync(
                $"https://flydata.avinor.no/XmlFeed.asp?TimeFrom=0&TimeTo=3&airport={tag}&direction=D");

                string xmlString = await flightData.Content.ReadAsStringAsync();

                // Create an XmlReader from the string
                XmlReader xmlReader = XmlReader.Create(new StringReader(xmlString));

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                // Get the list of <flight> elements
                XmlNodeList flightNodes = xmlDoc.GetElementsByTagName("flight");

                // Get the length of the list
                int numFlights = flightNodes.Count;

                // Print the length to the console
                Console.WriteLine("Number of flights next 3 hours: " + numFlights);

                next3HoursCache[tag] = Tuple.Create(numFlights, DateTime.Now);

                return numFlights;
            }
            else
            {
                // The value has been updated within the last hour
                return next3HoursCache[tag].Item1;
            }

        }

        public async Task<List<string>> GetFlightScheduleTimes(HttpClient httpClient, string tag)
        {
            if (!scheduleCache.ContainsKey(tag))
            {
                scheduleCache[tag] = Tuple.Create(new List<string>(), DateTime.Now.AddDays(-1));
            }

            // Check if it has been 1 hour since the value was last updated
            if (DateTime.Now - scheduleCache[tag].Item2 > TimeSpan.FromHours(1))
            {
                // The value has not been updated in 1 hour
                // Do something here, such as updating the value
                // Send a GET request to the specified URL and retrieve the response
                using HttpResponseMessage flightData = await httpClient.GetAsync(
                    $"https://flydata.avinor.no/XmlFeed.asp?airport={tag}&direction=D");

                string xmlContent = await flightData.Content.ReadAsStringAsync();
                XmlDocument document = new XmlDocument();
                document.LoadXml(xmlContent);

                List<string> scheduleTimes = new List<string>();
                foreach (XmlNode flight in document.SelectNodes("//flight"))
                {
                    XmlNode scheduleTimeNode = flight.SelectSingleNode("schedule_time");
                    if (scheduleTimeNode != null && !string.IsNullOrEmpty(scheduleTimeNode.InnerText))
                    {
                        scheduleTimes.Add(scheduleTimeNode.InnerText);
                    }
                }

                scheduleCache[tag] = Tuple.Create(new List<string>(scheduleTimes), DateTime.Now);

                return scheduleTimes;
            }
            else
            {
                // The value has been updated within the last hour
                return scheduleCache[tag].Item1;
            }
        }

    }
}
