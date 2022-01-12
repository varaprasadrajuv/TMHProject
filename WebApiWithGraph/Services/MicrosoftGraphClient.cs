﻿using WebApiWithGraph.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiWithGraph.Services
{
    public static class MicrosoftGraphClient
    {
        private static GraphServiceClient graphClient;
        private static IConfiguration configuration;

        private static string clientId;
        private static string clientSecret;
        private static string tenantId;
        private static string aadInstance;
        private static string graphResource;
        private static string graphAPIEndpoint;
        private static string authority;
        
        static MicrosoftGraphClient()
        {
            configuration = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
            SetAzureADOptions();
        }

        private static void SetAzureADOptions()
        {
            var azureOptions = new AzureAD();
            configuration.Bind("AzureAD", azureOptions);

            clientId = azureOptions.ClientId;
            clientSecret = azureOptions.ClientSecret;
            tenantId = azureOptions.TenantId;
            aadInstance = azureOptions.Instance;
            graphResource = azureOptions.GraphResource;
            graphAPIEndpoint = $"{azureOptions.GraphResource}{azureOptions.GraphResourceEndPoint}";
            authority = $"{aadInstance}{tenantId}";
        }

        public static async Task<GraphServiceClient> GetGraphServiceClient()
        {
            // Get Access Token and Microsoft Graph Client using access token and microsoft graph v1.0 endpoint
            var delegateAuthProvider = await GetAuthProvider();
            // Initializing the GraphServiceClient
            graphClient = new GraphServiceClient(graphAPIEndpoint, delegateAuthProvider);

            return graphClient;
        }


        private static async Task<IAuthenticationProvider> GetAuthProvider()
        {
            AuthenticationContext authenticationContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(clientId, clientSecret);

            // ADAL includes an in memory cache, so this call will only send a message to the server if the cached token is expired.
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(graphResource, clientCred);
            var token = authenticationResult.AccessToken;

            var delegateAuthProvider = new DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.ToString());
                return Task.FromResult(0);
            });

            return delegateAuthProvider;
        }
    }
}
