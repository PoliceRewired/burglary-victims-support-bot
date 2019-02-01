// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using VictimBot.Lib.State;

namespace VictimBot
{
    /// <summary>
    /// The Startup class configures services and the request pipeline.
    /// </summary>
    public class Startup
    {
        private ILoggerFactory loggerFactory;
        private readonly bool isProduction;

        public Startup(IHostingEnvironment env)
        {
            isProduction = env.IsProduction();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> specifies the contract for a collection of service descriptors.</param>
        /// <seealso cref="IStatePropertyAccessor{T}"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0"/>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<VictimBotBot>(options =>
            {
               var secretKey = Configuration.GetSection("botFileSecret")?.Value;
               var botFilePath = Configuration.GetSection("botFilePath")?.Value;

               // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection
               var botConfig = BotConfiguration.Load(botFilePath ?? @".\VictimBot.bot", secretKey);
               services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot config file could not be loaded. ({botConfig})"));

               // Retrieve current endpoint
               var environment = isProduction ? "production" : "development";
               var service = botConfig.Services.FirstOrDefault(s => s.Type == "endpoint" && s.Name == environment);
               if (!(service is EndpointService endpointService))
               {
                   throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
               }

               options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

               ILogger logger = loggerFactory.CreateLogger<VictimBotBot>();
               options.OnTurnError = async (context, exception) =>
               {
                  logger.LogError($"Exception caught : {exception}");
                  await context.SendActivityAsync("Sorry, it looks like something went wrong.");
               };

               // The Memory Storage used here is for local bot debugging only.
               // When the bot is restarted, everything stored in memory will be gone.
               IStorage dataStore = new MemoryStorage();

               // For production bots use the Azure Blob or
               // Azure CosmosDB storage providers. For the Azure
               // based storage providers, add the Microsoft.Bot.Builder.Azure
               // Nuget package to your solution. That package is found at:
               // https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/

               // Uncomment the following lines to use Azure Blob Storage
               // //Storage configuration name or ID from the .bot file.
               // const string StorageConfigurationId = "<STORAGE-NAME-OR-ID-FROM-BOT-FILE>";
               // var blobConfig = botConfig.FindServiceByNameOrId(StorageConfigurationId);
               // if (!(blobConfig is BlobStorageService blobStorageConfig))
               // {
               //    throw new InvalidOperationException($"The .bot file does not contain an blob storage with name '{StorageConfigurationId}'.");
               // }
               // // Default container name.
               // const string DefaultBotContainer = "<DEFAULT-CONTAINER>";
               // var storageContainer = string.IsNullOrWhiteSpace(blobStorageConfig.Container) ? DefaultBotContainer : blobStorageConfig.Container;
               // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage(blobStorageConfig.ConnectionString, storageContainer);

               // Create conversation state and user state objects
               var conversationState = new ConversationState(dataStore);
                var userState = new UserState(dataStore);
                options.State.Add(conversationState);
                options.State.Add(userState);
            });

            // Create and register state accessors
            services.AddSingleton<VictimBotAccessors>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the state accessors");
                }

                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null) { throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors."); }

                var userState = options.State.OfType<UserState>().FirstOrDefault();
                if (userState == null) { throw new InvalidOperationException("UserState must be defined and added before adding any user-scoped state accessors."); }

                var accessors = new VictimBotAccessors(conversationState, userState)
                {
                    CurrentIncident_Accessor = conversationState.CreateProperty<IncidentData>(VictimBotAccessors.CurrentIncidentState_Key),
                    UserProfile_Accessor = conversationState.CreateProperty<UserProfileData>(VictimBotAccessors.UserProfileState_Key),
                    DialogState_Accessor = conversationState.CreateProperty<DialogState>(VictimBotAccessors.DialogState_Key)
                };

                return accessors;
            });
        }

        public void Configure(IApplicationBuilder application, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            application
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
