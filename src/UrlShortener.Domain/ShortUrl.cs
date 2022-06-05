namespace UrlShortener.Domain
{
    public class ShortUrl
    {
        public string ShortName { get; set; } = String.Empty;
        public string DestinationUrl { get; set; } = String.Empty;
        public long CreationDateTime { get; set; }
        public long LastUpdateDateTime { get; set; }
        public List<ShortUrlClick>  ShortUrlClicks { get; set; }
    }
} 
 
