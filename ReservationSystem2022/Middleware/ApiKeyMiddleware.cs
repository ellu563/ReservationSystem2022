namespace ReservationSystem2022.Middleware
{
    public class ApiKeyMiddleware
    {
        // tarvitaan constructori joka ottaa RequestDeleaten vastaan

        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "ApiKey"; // avaimen kenttä on nimetty näin
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // tarvitaan käsittelevä funktio
        public async Task InvokeAsync(HttpContext context)
        {
            // onko avainta olemassa, onko avain headerissa
            // out var extractedApiKey = jos löytyy arvo, tähän tallennetaan se
            if(!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // ei löytynyt sitä
                await context.Response.WriteAsync("Api key missing");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            // luetaan se avain mikä laitettiin appsettings.jsoniin
            var apiKey = appSettings.GetValue<string>(APIKEYNAME);

            // onko avaimet erilaiset
            if(!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Unauthorised client"); // vaara api key
                return; // ei mennä eteenpain
            }

            await _next(context);
        }

        // postmaniin lisatty ApiKey (headers kohtaan ja sinne se key) sit voidaan get ni toimii
    }
}
