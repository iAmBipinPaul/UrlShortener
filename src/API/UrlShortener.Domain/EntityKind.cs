using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Domain
{
    public enum EntityKind
    {

        None = 0,
        User = 1,
        ShortUrl = 2,
        ShortUrlClick = 3
    }
}
