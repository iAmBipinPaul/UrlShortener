using Newtonsoft.Json;

namespace UrlShortener.Domain
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PartitionValue { get; set; }
        public EntityKind EntityKind { get; set; }=EntityKind.User;
    }
}
 