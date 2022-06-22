using System.Text.Json.Serialization;

namespace UrlShortener.Core
{
    public class IpInfo
    {
      
        public string Ip { get;  set; }
        public string City { get;  set; }
        public string Region { get;  set; }
        public string Country { get;  set; }
        public string Org { get;  set; }
        public string Postal { get;  set; }
        public string Timezone { get;  set; }
    }
}
 