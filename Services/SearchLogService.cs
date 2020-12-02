using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace hellodotnetcore.Services
{
    class SearchLogService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public SearchLogService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		// Update customerId to your Log Analytics workspace ID
		static string customerId = "b34db949-e22d-48ff-960d-4a165c625949";

		// For sharedKey, use either the primary or the secondary Connected Sources client authentication key   
		static string sharedKey = "WvId8iON5r9ZvN7l0mxEA26T9cwQkWWJ/9QZGn5VJmzkcpR5jAx7PK4lHTttFz30kKpN3GQYuOOJ/WCVktobMg==";

		// LogName is name of the event type that is being submitted to Azure Monitor
		static string LogName = "TestSearchLog";

		// You can use an optional field to specify the timestamp from the data. If the time field is not specified, Azure Monitor assumes the time is the message ingestion time
		static string TimeStampField = "";

        class SearchLog {
            public string FreeText;
            public string Username;
        }

		public void createSearchLog(string searchText)
		{
            Console.WriteLine("in search service create search log");

            SearchLog searchLog = new SearchLog();

            searchLog.Username = _httpContextAccessor.HttpContext.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].ToString();
            searchLog.FreeText = searchText;

            string json = JsonConvert.SerializeObject(searchLog);
            Console.WriteLine(json);
            
			// Create a hash for the API signature
			var datestring = DateTime.UtcNow.ToString("r");
			var jsonBytes = Encoding.UTF8.GetBytes(json);
			string stringToHash = "POST\n" + jsonBytes.Length + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
			string hashedString = BuildSignature(stringToHash, sharedKey);
			string signature = "SharedKey " + customerId + ":" + hashedString;

			PostData(signature, datestring, json);
		}

		// Build the API signature
		public static string BuildSignature(string message, string secret)
		{
			var encoding = new System.Text.ASCIIEncoding();
			byte[] keyByte = Convert.FromBase64String(secret);
			byte[] messageBytes = encoding.GetBytes(message);
			using (var hmacsha256 = new HMACSHA256(keyByte))
			{
				byte[] hash = hmacsha256.ComputeHash(messageBytes);
				return Convert.ToBase64String(hash);
			}
		}

		// Send a request to the POST API endpoint
		public static void PostData(string signature, string date, string json)
		{
			try
			{
				string url = "https://" + customerId + ".ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

				System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
				client.DefaultRequestHeaders.Add("Accept", "application/json");
				client.DefaultRequestHeaders.Add("Log-Type", LogName);
				client.DefaultRequestHeaders.Add("Authorization", signature);
				client.DefaultRequestHeaders.Add("x-ms-date", date);
				client.DefaultRequestHeaders.Add("time-generated-field", TimeStampField);

				System.Net.Http.HttpContent httpContent = new StringContent(json, Encoding.UTF8);
				httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				Task<System.Net.Http.HttpResponseMessage> response = client.PostAsync(new Uri(url), httpContent);

				System.Net.Http.HttpContent responseContent = response.Result.Content;
				string result = responseContent.ReadAsStringAsync().Result;
			}
			catch (Exception excep)
			{
				Console.WriteLine("API Post Exception: " + excep.Message);
			}
		}
	}
}