
using System.Net;
using System.Net.Http.Headers;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace BillingFunction
{
	public class GetBillingInfoByResourceId
	{

		private static readonly HttpClient _httpClient = new HttpClient();

		public GetBillingInfoByResourceId()
		{
		}

		[Function("GetBillingPeriods")]
		public async Task<HttpResponseData> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "billing/periods")] HttpRequestData req)
		{


			var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
			string subscriptionId = queryParams["subscriptionId"]; // Get the subscriptionId from query params

			if (string.IsNullOrEmpty(subscriptionId))
			{
				var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
				await errorResponse.WriteStringAsync("Error: Subscription ID is missing.");
				return errorResponse;
			}


			//string subscriptionId = "529c15e0-232e-4ca7-81de-40ef39cfedc2";
			string apiVersion = "2022-10-01-";
			// give your billingAcccount nubmer or you can get programiticaly from api or sdk
			string billingAccountName = "00000000-00000000-4abe-0000000000:70e961e7-4c17-4131-8301-00000000_2019-05-31";


			string endpoint = $"https://management.azure.com/providers/Microsoft.Billing" +
				$"/billingAccounts/{billingAccountName}/billingSubscriptions/{subscriptionId}?api-version={apiVersion}" +
				$"privatepreview&excludeCharges=true&includeDeleted=true";


			//	string endpoint = "https://management.azure.com/providers/Microsoft.Billing/billingAccounts?api-version=2020-05-01";


			try
			{
				// Use DefaultAzureCredential to obtain an access token

				var credentialOptions = new DefaultAzureCredentialOptions
				{
					TenantId = "3f4970f5-8b3c-4657-a628-d09cc3ece38d"
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
