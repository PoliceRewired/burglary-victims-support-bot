using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Dialogs.Dialogs;
using VictimBot.Lib;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.State;

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
            this.logger = loggerFactory.CreateLogger<VictimBotBot>();
            this.logger.LogTrace("Turn start.");
            this.accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            // use VictimBotDialogRegistry to automatically add all dialogs and their prompts to a DialogSet
            this.registry = new VictimBotDialogRegistry(accessors);
            this.dialogs = registry.DialogSet; // all defined dialogs and prompts registered!
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Current implementation:
                // 1. If it's a user joining, then greet them
                // 2. If the user said something, and there's a dialog in session, continue that dialog
                // 3. If the user said something, there's no dialog, and no user information, start the RegistrationDialog
                // 4. If the user said something, there's no dialog, and we have user information, start the MainChoicesDialog

                // TODO: planned ideal flow
                // 1. Examine the text for user intent
                // 2. If the intent is to escape/switch dialog context, do that
                // 3. If not, then continue with the dialog
                // 4. If there isn't a current dialog, examine the user intent again
                // 5. Launch the appropriate dialog for the user intent (where possible)
                // 6. If the intent cannot be determined, apologise and offer the user ways to re-enter dialog

                // Get the user state from the turn context.
                var channelId = turnContext.Activity.ChannelId;
                var user = await accessors.UserProfile_Accessor.GetAsync(
                    turnContext,
                    () => accessors.Storage.UserProfiles.ReadOrCreateAsync(channelId).Result,
                    cancellationToken);

                // The framework figures out the current dialog, and continues it if there is one
                var dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                switch (results.Status)
                {
                    case DialogTurnStatus.Cancelled:
                    case DialogTurnStatus.Empty:
                    case DialogTurnStatus.Complete:
                        if (!user.Complete)
                        {
                            // initiate the minimum user details collection dialog
                            await dialogContext.BeginDialogAsync(registry.RegistrationDialogId, null, cancellationToken);
                        }
                        else
                        {
                            // initiate the main choices dialog (main menu)
                            await dialogContext.BeginDialogAsync(registry.MainChoicesDialogId, null, cancellationToken);
                        }
                        break;

                    case DialogTurnStatus.Waiting:
                        // NOP
                        break;
                }

                await accessors.UserState.SaveChangesAsync(turnContext);
                await accessors.ConversationState.SaveChangesAsync(turnContext);
            }
            else
            {

#if DEBUG
                // This was not a message activity. Echo events into the chat stream in DEBUG mode only.
                // await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected");
#endif

                // if any new member apparently added to the conversation isn't the bot itself, then greet them
                if (turnContext.Activity.MembersAdded.Any(m => m.Id != turnContext.Activity.Recipient.Id))
                {
                    // wave hello
                    var greeting = CardHelper.GenerateHero(
                        SharedResources.FirstTimeTitle,
                        SharedResources.FirstTimeSubtitle,
                        SharedResources.FirstTimeGreeting,
                        BotImages.bot_wave_01);

                    await turnContext.SendActivityAsync(greeting, cancellationToken);

                    await turnContext.SendActivityAsync(MessageFactory.Text(SharedResources.EmergenciesAdviceReminder), cancellationToken);
                    await turnContext.SendActivityAsync(MessageFactory.Text(SharedResources.GetStartedAdvice), cancellationToken);
                }

            }
        }
    }
}
