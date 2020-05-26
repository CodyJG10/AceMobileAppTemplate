using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AceMobileAppTemplate.Models
{
    public class AuthResult
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
