using System.Net.Http.Headers;
using System.Text;
namespace api.Controllers.Helpers;

public class RequestHeaders
{
    public static string GetCurrentUser(HttpRequest request)
    {
        try {
            var headerValue = AuthenticationHeaderValue.Parse(request.Headers["Authorization"]);
            var bytes = Convert.FromBase64String(headerValue.Parameter);
            string credentials = Encoding.UTF8.GetString(bytes);
            string username = credentials.Split(":")[0];
            return username;
        } catch(Exception e){
            Console.WriteLine("Error, unauthorized request is trying to access username field!");
            return "";
        }
    }
}