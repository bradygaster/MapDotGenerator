using System.Text.Json;
using static Bogus.DataSets.Name;

namespace MapDotGenerator
{
    internal class User
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Gender Gender { get; set; }
        public string Name { get; set; }

        public string AsJson()
        {
            return JsonSerializer.Serialize(
                this, 
                typeof(User), 
                new JsonSerializerOptions 
                {
                    WriteIndented = true
                });
        }
    }
}