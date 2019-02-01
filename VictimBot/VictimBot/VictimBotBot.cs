using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Dialogs.Dialogs;
using VictimBot.Lib;
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

                // The framework figures out the current dialog, and continues it if there is one
                var dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                switch (results.Status)
                {
                    case DialogTurnStatus.Cancelled:
                    case DialogTurnStatus.Empty:
                    case DialogTurnStatus.Complete:
                        // initiate the opening (greeting) dialog
                        if (!user.Complete)
                        {
                            await dialogContext.BeginDialogAsync(registry.RegistrationDialogId, null, cancellationToken);
                        }
                        else
                        {
                            await dialogContext.BeginDialogAsync(registry.MainChoicesDialogId, null, cancellationToken);
                        }
                        break;

                    case DialogTurnStatus.Waiting:
                        // NOP
                        break;
                }

                // Echo back to the user whatever they typed.
                // var responseMessage = $"{user.Name} you said '{turnContext.Activity.Text}'\n";
                // await turnContext.SendActivityAsync(responseMessage);

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
                    await turnContext.SendActivityAsync(MessageFactory.Text(SharedResources.FirstTimeGreeting), cancellationToken);
                    await turnContext.SendActivityAsync(MessageFactory.Text(SharedResources.EmergenciesAdviceReminder), cancellationToken);
                    await turnContext.SendActivityAsync(MessageFactory.Text(SharedResources.GetStartedAdvice), cancellationToken);
                }

            }
        }
    }
}
