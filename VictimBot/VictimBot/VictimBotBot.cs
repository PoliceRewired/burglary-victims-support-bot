// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Shared;
using VictimBot.Shared.Dialogs.PersonalDetails;
using VictimBot.Shared.State;

namespace VictimBot
{
    public class VictimBotBot : IBot
    {
        private readonly VictimBotAccessors accessors;
        private readonly VictimBotDialogRegistry registry;
        private readonly DialogSet dialogs;
        private readonly ILogger logger;

        public VictimBotBot(VictimBotAccessors accessors, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) { throw new System.ArgumentNullException(nameof(loggerFactory)); }
            logger = loggerFactory.CreateLogger<VictimBotBot>();
            logger.LogTrace("Turn start.");
            this.accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            // use a registry to automatically add all dialogs and their prompts to a DialogSet
            this.registry = new VictimBotDialogRegistry(accessors);
            this.dialogs = registry.DialogSet; // all defined dialogs and prompts registered!
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // TODO: instead of monitoring the username and starting *that* dialog (or echoing back, as below)...

                // 1. examine the text for user intent
                // 2. if the intent is to escape/switch dialog context, do that
                // 3. if not, then continue with the dialog
                // 4. if there isn't a current dialog, examine the user intent again
                // 5. launch the appropriate dialog for the user intent (where possible)
                // 6. if the intent cannot be determined, apologise and offer the user ways to re-enter dialog

                // Get the user state from the turn context.
                var user = await accessors.UserProfile_Accessor.GetAsync(turnContext, () => new UserProfileData());
                if (user.Name == null)
                {
                    // The framework figures out the current dialog, and continues it if there is one
                    var dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);
                    var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                    switch (results.Status)
                    {
                        case DialogTurnStatus.Cancelled:
                        case DialogTurnStatus.Empty:
                            // initiate the opening (greeting) dialog
                            await dialogContext.BeginDialogAsync(registry.StartDialogId, null, cancellationToken);
                            break;

                        case DialogTurnStatus.Complete:
                            // NOP
                            break;

                        case DialogTurnStatus.Waiting:
                            // NOP
                            break;
                    }
                }
                else
                {
                    // Echo back to the user whatever they typed.
                    var responseMessage = $"{user.Name} you said '{turnContext.Activity.Text}'\n";
                    await turnContext.SendActivityAsync(responseMessage);
                }

                await accessors.ConversationState.SaveChangesAsync(turnContext);
                await accessors.UserState.SaveChangesAsync(turnContext);
            }
            else
            {
                // This was not a message activity.

                // TODO: remove this
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected");
            }
        }
    }
}
