using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace Movies.Api.Consumer
{
    internal class AuthTokenProvider
    {
        private readonly HttpClient httpClient;
        private string cachedToken = string.Empty;
        private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        public AuthTokenProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(cachedToken))
            { 
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(cachedToken);

                var expiryTimeText = jwt.Claims.Single(claim => claim.Type == "exp").Value;

                var expiryDateTime = UnixTimeStampToDateTime(int.Parse(expiryTimeText));

                if (expiryDateTime > DateTime.UtcNow)
                {
                    return cachedToken; 
                }
            }

            await Lock.WaitAsync();

            var response = await httpClient.PostAsJsonAsync("https://localhost:5003/token", new
            {
                userid = "d8566de3-b1a6-4a9b-b842-8e3887a82e41",
                email = "hoang.d1997@gmail.com",
                customClaims = new 
                {
                    admin = true,
                    trusted_member = true
                }
            });

            var newToken = await response.Content.ReadAsStringAsync();  

            cachedToken = newToken;

            Lock.Release(); 

            return newToken;
        }

        private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();

            return dateTime; 
        }
    }
}
