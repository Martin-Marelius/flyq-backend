using System.Runtime.CompilerServices;

namespace backend_flyko.Base.Data
{
    public class Data : IData
    {
        public List<Airport> Venues { get; set; }

        public struct Airport
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Tag { get; set; }
            public string Venue_name { get; set; }
            public string Venue_address { get; set; }

        }

        public Data()
        {
            Venues = new List<Airport>
                {
                new Airport
                {
                Id = "ven_774d6f7453397072766c67525955514475494a4b7978764a496843",
                Name = "Gardermoen Lufthavn",
                Tag = "OSL",
                Venue_name = "oslo airport",
                Venue_address = "norway"
                },
                new Airport
                {
                Id = "ven_416744474c34505f65496b5259455077663336396d5a7a4a496843",
                Name = "Bergen Lufthavn, Flesland",
                Tag = "BGO",
                Venue_name = "bergen airport",
                Venue_address = "norway"
                },
                new Airport
                {
                Id = "ven_4146517a786a387a50754452554578454c43654f2d7a444a496843",
                Name = "Tromsø Lufthavn",
                Tag = "TOS",
                Venue_name = "Tromsø Airport",
                Venue_address = "Flyplassvegen 31 9016 Tromsø Norway"
                },
                new Airport
                {
                Id = "ven_344150527570457659517352596b4f4a5659714a52336c4a496843",
                Name = "Stavanger Lufthavn, Sola",
                Tag = "SVG",
                Venue_name = "Stavanger lufthavn",
                Venue_address = "Flyplassvegen Norway"
                },
                new Airport
                {
                Id = "ven_594c47794639717149305f52595562564d2d566e785a5f4a496843",
                Name = "Trondheim Lufthavn, Værnes",
                Tag = "TRD",
                Venue_name = "Trondheim Airport",
                Venue_address = "Norway"
                },
                new Airport
                {
                Id = "ven_7771437065776d644536785259454f425146676661374c4a496843",
                Name = "Kristiansand Lufthavn",
                Tag = "KRS",
                Venue_name = "Kristiansand Lufthavn",
                Venue_address = "Norway"
                },
                new Airport
                {
                Id = "ven_4d3233564a79624a366d365255303351456c614b465a744a496843",
                Name = "Bodø Lufthavn",
                Tag = "BOO",
                Venue_name = "Bodø Airport",
                Venue_address = "Norway"
                },
                new Airport
                {
                Id = "ven_306370347830524a43546c5255307a57385a6c693956334a496843",
                Name = "Alta Lufthavn",
                Tag = "ALF",
                Venue_name = "Alta Airport",
                Venue_address = "Altagårdskogen 32 9515 Alta Norway"
                },
            };
        }
    }
}
