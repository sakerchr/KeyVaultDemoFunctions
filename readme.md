# Azure Key Vault Introduction

This set of steps will take you through the steps that you commonly go through when working with Azure Key Vault. The goal is to end up with an HTTP triggered Function App that will have its own Managed Identity set up in Azure. This Managed Identity should have the appropriate roles assigned to it to be able to extract a particular secret from within this vault.

We will be taking advantage of the `Identity` and `Security.KeyVault` nuget packages for Microsoft Azure that expose the required functionality. For those interested there is another, and even simpler way of implementing this. If you want to see how, go to our LMS and browse my "Introduction to Azure Key Vault" course.

If you do get stuck there is a `FunctionSolution` example avaiable here. Do note that this uses the in-process .NET hosting model and .NET6 (you want to use the isolated worker model and .NET8)

## 1. Getting a local Function Project to work with

These steps are for generating a Function Project template locally using C# and .NET via dotnet CLI. If you want to do it using Visual Studio, Visual Studio Code, or wanting to use another programming language, there are guides for that here:

- https://learn.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio
- https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-csharp
- https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-typescript?pivots=nodejs-model-v4
- (many other alternatives in the menu of the above links)

With the dotnet CLI this is easy, navigate to where you want your Function Project with a terminal having the dotnet CLI available.
If you haven't installed the dotnet SDK then do that first (https://dotnet.microsoft.com/en-us/download).

1. Install the Function CLI https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=windows%2Cazure-cli#install-the-azure-functions-core-tools
2. Use the Function CLI to generate a Function Project template. The following `func init SecretFunctionApp --worker-runtime dotnet-isolated --target-framework net8.0` will provide you with a Function Project that we can start using directly.
3. The Function CLI has a `func new` command for creating new functions within a project taking a template-parameter. Use it from within your `SecretFunctionApp` project folder to generate a new HTTP triggered function: `func new --name Secret --template "HTTP trigger" --authlevel "anonymous"`

You now have a Function Project ready with an HTTP triggered function.

## 2. Setting up a Function App in Azure

If you simply want to run your function locally, you can skip this step. Otherwise follow either of the guides listed above, follow the same steps as have been shown in the presentation today.

Below are guidelines for deploying a new Function App using Azure Portal:

- You want to deploy a new Function App within a new or existing resource group.
- The Function App should deploy code, have a runtime stack and version matching your code and be deployed to a sensible region.
- The OS must support your stack and version.
- For hosting you should go with Consumpion (Servesless) to utilize the free offering.
- The next tabs, Storage, Networking, Monitoring, Deployment and Tags, can be left with their defaults.

## 3. Giving your Function App a Managed Identity

If you are running your function locally, make sure you use your own Identity in this step, and not that of your Function App.

1. Navigate to your Function App.
2. Look for the "Identiy" option under "Settings" in the menu on the left side.
3. In the "System assigned" tag, toggle the Status to be On, and click Save. Take note of the name of the Managed Identity you just created as it will be used in the next step.

## 4. Assigning the Managed Identity the necessary role within an existing Key Vault

1. Navigate to the overview page of the Key Vault within Azure. The URL has been provided to you.
2. Find "Access Control (IAM)" in the menu to the left.
3. Click "Add" and select "Add role assignment".
4. Select the "Key Vault Secrets User", which gives the correct DATA PLANE access, and click next. Check the Managed Identity option and find and select your Function App's identity. Click next and assign the role to the Managed Identity. (If you are running locally, this is the step where you select your own identity, not that of the Function App.)

## 5. Writing code to authenticate as the Managed Identity and extracting a Secret from Key Vault

1. Install the Azure.Identity nuget package.
2. Instantiate a TokenCredential via the DefaultAzureCredential via something like `var credential = new DefaultAzureCredential();`. This looks for a valid identity in several locations, and supports both local users, managed identities in Azure and Visual Studio and Visual Studio Code users (see https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet).
3. Install the Azure.KeyVault.Secret nuget package.
4. Instantiate a SecretClient to be able to interact with the Key Vault via something like `var secretClient = new SecretClient("https://myvault.vault.azure.net/", credential);`
5. Retrieve a secret by using the SecretClient via something like `var secret = await secretClient.GetSecretAsync("MySecret");`
6. Code your function to return the secret's value.
7. Deploy your function to Azure and run it. See the below links for how to do this with the various tooling available:
   - https://learn.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio#publish-the-project-to-azure
   - https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-csharp
   - https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=windows%2Cazure-cli
