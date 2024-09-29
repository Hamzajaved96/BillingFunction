using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net;

namespace BillingFunction
{
	public class GetBillingInformationByResourceIdList
	{
		private readonly ILogger<GetBillingInformationByResourceIdList> _logger;
		private static readonly HttpClient _httpClient = new HttpClient();

		public GetBillingInformationByResourceIdList(ILogger<GetBillingInformationByResourceIdList> logger)
		{
			_logger = logger;
		}

		[Function("GetBillingInformationByResourceIdList")]
		public async Task<HttpResponseData> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "billing/periods")] HttpRequestData req)
		{
			var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
			string resourceIdsQuery = queryParams["resourceIds"]; // Get the comma-separated resource IDs from query params

			if (string.IsNullOrEmpty(resourceIdsQuery))
			{
				var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
				await errorResponse.WriteStringAsync("Error: resourceIds are missing.");
				return errorResponse;
			}

			// Split the resource IDs into an array
			var resourceIds = resourceIdsQuery.Split(',');

			string apiVersion = "2022-10-01-";
			string billingAccountName = "00000000-00000000-4abe-0000000000:70e961e7-4c17-4131-8301-00000000_2019-05-31";
			var results = new List<string>();

			try
			{
				// Use DefaultAzureCredential to obtain an access token
				var credentialOptions = new DefaultAzureCredentialOptions
				{
					TenantId = "your tenant id"
				};
				TokenCredential credential = new DefaultAzureCredential(credentialOptions);
				ArmClient client = new ArmClient(credential);

				string[] scopes = new string[] { "https://management.azure.com/.default" };
				AccessToken token = await credential.GetTokenAsync(new TokenRequestContext(scopes), new CancellationToken());

				// Set up the HTTP request with the access token
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

				foreach (var resourceId in resourceIds)
				{
					// Construct the endpoint for each resource ID
					string endpoint = $"url"
;

					var response = await _httpClient.GetAsync(endpoint);

					
					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();
						results.Add(content);
					}
					else
					{
						results.Add($"Error for resource ID {resourceId}: {response.ReasonPhrase}");
					}
				}

				var httpResponse = req.CreateResponse(HttpStatusCode.OK);
				httpResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
				await httpResponse.WriteStringAsync(string.Join(Environment.NewLine, results));

				return httpResponse;
			}
			catch (Exception ex)
			{
				var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
				await errorResponse.WriteStringAsync("Error occurred while calling the API.");
				return errorResponse;
			}
		}
	}
}
