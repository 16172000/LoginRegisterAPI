
using System.Net.Http.Formatting;
using System.Text;
using System.Text.Json;

namespace TicketProjectWEB.Models
{
    public class MyService
    {
        //private readonly IConfiguration _configuration;
        //private readonly HttpClient _httpClient;
        //private readonly IHttpClientFactory _httpClientFactory;


        //public MyService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        //{
        //    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        //    _httpClient = httpClientFactory.CreateClient();
        //}


        //public async Task<Register> GetDataFromApi()
        //{
        //    var apiUrl = _configuration["ApiSettings:BaseUrl"];
        //    var apiEndpoint = "api/Project/register"; // Replace with your API endpoint

        //    if (string.IsNullOrEmpty(apiUrl))
        //    {
        //        // Handle the error (e.g., log, throw exception, etc.)
        //        return null;
        //    }

        //    _httpClient.BaseAddress = new Uri(apiUrl);

            
        //    var registerData = new Register
        //    {
        //        UserName = "exampleUser",
        //        Email = "example@example.com",
        //        Password = "password",
        //        ConfirmPassword = "cnfpass",
        //        Age = 0,
        //        State = "Jharkhand",
        //        PhoneNumber = 98989898987

        //    };

        //    var jsonContent = JsonSerializer.Serialize(registerData);

        //    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //    var response = await _httpClient.PostAsync(apiEndpoint, content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseJson = await response.Content.ReadAsStringAsync();
        //        var data = JsonSerializer.Deserialize<Register>(responseJson);

        //        return data;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public MyService(IHttpClientFactory httpClientFactory)
        //{
        //    _httpClientFactory = httpClientFactory;
        //}

        //public void SomeMethod()
        //{
        //    var httpClient = _httpClientFactory.CreateClient();
        //    // Use the httpClient to make requests
        //}
    }
}
