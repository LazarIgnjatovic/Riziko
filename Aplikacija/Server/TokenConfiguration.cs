using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class TokenConfiguration
    {
        public string Path { get; set; } = "/token";
        public string Key { get; set; } = "nekiOgromanKljucZaSifrovanjeDeGaaitakodalje";
        public string Issuer { get; set; } = "Server";
        public string Audience { get; set; } = "Server";
        public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(24);
        public TimeSpan RefreshExpiration { get; set; } = TimeSpan.FromHours(27);
    }
}
