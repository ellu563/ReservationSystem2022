using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ReservationSystem2022.Models;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace ReservationSystem2022.Middleware
{
    // Yksinkertainen käyttäjän tunnistustapa, käyttäjätunnus ja salasana siirretään HTTP headerissa =
    // (Authorization: Basic *jotain* esim. kayttis:salasana, ja base64 koodattuna)
    // esitellään myös palveluna program.cs
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> // pitää periytyä AuthenticationHandler luokasta
    {
        private readonly IUserAuthenticationService _userAuthenticationService;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserAuthenticationService userAuthenticationService) : base(options, logger, encoder, clock)
        {
            _userAuthenticationService = userAuthenticationService;
        }

        // Toteuttaa HandleAuthenticateAsync funktion
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint(); // tarkistetaan ensin mihin se ottaa yhteyttä

            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null) // endpointin metadatasta löydetään tieto
            {
                return AuthenticateResult.NoResult(); // jos se on sellainen funktio mikä ei vaadi autentikointia, ei tarkisteta käyttäjätietoja
            }

            if (!Request.Headers.ContainsKey("Authorization")) // eli Authorization on se kentta mika siellä headerissa esim.postmanissa
            {
                return AuthenticateResult.Fail("Authorization header missing"); // tarvitaan autentikointi
                // ei ole tullut tietoja/on tullut vaarat tiedot
            }

            User user = null;

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]); 
                // etitaan http kutsun headerista Authorization ja otetaan sen arvo talteen
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter); // sisaltaa base koodin, muutetaan tavujonoksi
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2); // tehaan siita string (taulukkoon)
                var userName = credentials[0]; // eli taulukon eka alkio on username
                var password = credentials[1]; // ja toka password

                // service esitelty program.cs:ssä ni nyt voidaan kayttaa taalla
                // eli kutsutaan Authenticate funktiota, tekee tietokantahaun
                user = await _userAuthenticationService.Authenticate(userName, password);
                if (user ==null) 
                {
                    return AuthenticateResult.Fail("Unauthorised");
                }
            }
            catch
            {
                return AuthenticateResult.Fail("Unauthorised");
            }
            var claims = new[] // käyttäjän tunnistusta
            {
                new Claim(ClaimTypes.Name, user.UserName), // roolin voisi laittaa esim. ClaimTypes.Role
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket); // on tunnistettu
        }
    }
}

