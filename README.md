
******************************************************
  Azure Functions Billing Information Retrieval
******************************************************

Overview

This project showcases how to retrieve billing information from Azure using Azure Functions. 
As I am new to Azure, this experience has been invaluable in learning the basics of Azure and Azure Functions, 
especially since I have previous experience with AWS, which facilitated my understanding.

Steps Followed to Complete the Task

1.	Create an Azure Account  
 
        o  I set up a free Azure subscription.

2.	Create the Azure Function

        o	Opened Visual Studio and created a new Azure Function project.
        o	Deployed the Azure Function to my Azure Function App.

3.	Role Assignment Proper role assignments were configured for the Azure Function to ensure it has the necessary permissions to access Azure resources.

4.	Install Required Azure SDKs

     I added the following Azure SDK packages to my project:
       o	Azure.Core
       o	Azure.Identity
       o	Azure.ResourceManager.Billing
       o	Azure.ResourceManager.CostManagement

5.	Implement Authentication Logic
        Developed logic to authenticate with the Azure portal, utilizing the default credentials from Visual Studio.

6.	Call Azure REST API

        Using the obtained authentication token, I successfully called the Azure REST API to retrieve billing information.


   API Endpoints
 ______________________________________________

 * Get Billing Information for Subscription ID
 ______________________________________________
Running Locally

1.	Add your Azure account.

2.	Specify the account name in GetBillingInfoBySubscriptionId.cs on line 43.

3.	Run the application.

4.	Access the endpoint:

http://localhost:7071/api/billing/periods?subscriptionId=<your_subscription_id>

Testing on Azure Portal

1.	This function is deployed on my Azure account and can retrieve billing details for my subscription.

2.	You can test it using the following URL:

https://billingfunction2024.azurewebsites.net/api/billing/periods?code=k_yRblRy8ZUVi1hVQlqnJfmZXpqGJqd22oN8uzr8UDw5AzFuW2xgJw%3D%3D&subscriptionId=529c15e0-232e-4ca7-81de-40ef39cfedc2
________________________________________________________________________________

 *  Get Billing Information for Resource ID
________________________________________________________________________________

1.	Similar to the subscription ID endpoint, I utilized REST APIs to retrieve the data.

2.	I am still clarifying the specific data types required for this endpoint.
________________________________________________________________________________

 *  Get Billing Information for Multiple Resource IDs
________________________________________________________________________________

1.	Implemented logic to handle multiple resource IDs in a single request.

2.	Further refinement and validation of the data retrieved are in progress.

