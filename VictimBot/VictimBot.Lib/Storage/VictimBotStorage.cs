using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using VictimBot.Lib.Storage.DAO;

namespace VictimBot.Lib.Storage
{
    public class VictimBotStorage
    {
        // TODO: replace with Cosmos DB storage (or other) when ready for staging
        // See: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-storage?view=azure-bot-service-4.0&tabs=csharp
        public IStorage Storage { get; private set; }

        public UserProfileDAO UserProfiles { get; private set; }
        public IncidentDAO Incidents { get; private set; }

        public VictimBotStorage()
        {
            Storage = new MemoryStorage();
            UserProfiles = new UserProfileDAO(Storage);
            Incidents = new IncidentDAO(Storage);
        }


    }
}
