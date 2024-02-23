using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LiveOrders.Backend;

public class ExternalClient
{
    private HttpClient _client;
    private TimeSpan _timeout;
    private string _uri;
    private string _token;
    private string _apiLogin;

    public ExternalClient(TimeSpan timeout, string uri, string apiLogin)
    {
        _timeout = timeout;
        _uri = uri;
        _apiLogin = apiLogin;
        _client = new HttpClient();
        _client.Timeout = _timeout;
        _token = string.Empty;
        
        this.GetToken();
    }

    private async Task GetToken()
    {
        var content = new {apiLogin = _apiLogin};
        var result = await this.PostRequest<TokenResponse>("access_token", content);
        _token = result.Token;
    }

    public void GetTablesTest()
    {
        var content = "{\"terminalGroupIds\": [\"94fa1e52-1b5b-460d-aa66-a71a085310ee\"], \"returnSchema\": true, \"revision\": 0}";
        var path = "reserve/available_restaurant_sections";
        
        using (var request = new HttpRequestMessage(HttpMethod.Post, $"{_uri}{path}"))
        {
            Console.WriteLine(JsonConvert.SerializeObject(content));
            request.Content = JsonContent.Create(content);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            
            var response = _client.SendAsync(request).Result;
            
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            
            response.EnsureSuccessStatusCode();
            
            var result = response.Content.ReadFromJsonAsync<RestaurantSectionResponse>();
            Console.WriteLine(result);
        }
    }
    
    public async Task<RestaurantSectionResponse> GetTables(Guid terminalGroupId)
    {
        var content = new
        {
            terminalGroupIds = new List<string> { terminalGroupId.ToString() },
            returnSchema = true,
            revision = 0,
        };

        return await this.PostRequest<RestaurantSectionResponse>("reserve/available_restaurant_sections", content);
    }

    private async Task<T> PostRequest<T>(string path, object content)
    {
        using (var request = new HttpRequestMessage(HttpMethod.Post, $"{_uri}{path}/"))
        {
            Console.WriteLine(JsonConvert.SerializeObject(content));
            request.Content = JsonContent.Create(content);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            
            var response = _client.SendAsync(request).Result;
            
            if (path != "access_token")
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}

public record TokenResponse(Guid CorrelationId, string Token);

public record RestaurantSectionResponse(Guid CorrelationId,
    RestaurantSection RestaurantSection,
    int Revision);

public record RestaurantSection(Guid Id,
    Guid TerminalGroupId,
    string Name,
    IEnumerable<Table> Tables);
    
public record Table(Guid Id,
    int Number,
    string Name,
    string SeatingCapacity,
    int Revision,
    bool IsDeleted,
    Guid PosId);

