using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;

namespace BillingFunction
{
    public class GetBillingInformationByResouceId
    {
        private readonly ILogger<GetBillingInformationByResouceId> _logger;

		private static readonly HttpClient _httpClient = new HttpClient();

		public GetBillingInformationByResouceId(ILogger<GetBillingInformationByResouceId> logger)
        {
            _logger = logger;
        }

        [Function("GetBillingInformationByResouceId")]
		public async Task<HttpResponseData> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "billing/periods")] HttpRequestData req)
		{


			var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
			string resourceId = queryParams["reourceId"]; // Get the subscriptionId from query params

			if (string.IsNullOrEmpty(resourceId))
			{
				var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
				await errorResponse.WriteStringAsync("Error: reourceId ID is missing.");
				return errorResponse;
			}


			string apiVersion = "2022-10-01-";
			// give your billingAcccount nubmer or you can get programiticaly from api or sdk
			string billingAccountName = "00000000-00000000-4abe-0000000000:70e961e7-4c17-4131-8301-00000000_2019-05-31";



			string endpoint = "end point for billing data against resource id";

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

				// Send the request to the Azure REST API
				var response = await _httpClient.GetAsync(endpoint);

				// Handle the response
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();

					var httpResponse = req.CreateResponse(HttpStatusCode.OK);
					httpResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
					await httpResponse.WriteStringAsync(content);

					return httpResponse;
				}
				else
				{
					var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
					await errorResponse.WriteStringAsync($"Error: {response.ReasonPhrase}");
					return errorResponse;
				}
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
