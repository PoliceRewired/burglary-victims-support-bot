using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using VictimBot.Shared;
using VictimBot.Shared.State;

namespace VictimBot.Shared.Dialogs.PersonalDetails
{
    /// <summary>
    /// Responsible for learning the user's name and a good contact for them
    /// </summary>
    public class LearnPersonalDetailsDialog : VictimBotWaterfallDialog
    {
        public LearnPersonalDetailsDialog(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected override IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps()
        {
            return new IVictimBotWaterfallStep[]
            {
                new AskNameStep(Accessors),
                new ConfirmNameStep(Accessors)
            };
        }
    }

    public class AskNameStep : VictimBotWaterfallStep<TextPrompt>
    {
        public AskNameStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected override TextPrompt CreatePrompt()
        {
            return new TextPrompt(StepId, null);
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                StepId,
                new PromptOptions { Prompt = MessageFactory.Text("What is your name?") },
                cancellationToken);
        }
    }

    public class ConfirmNameStep : VictimBotWaterfallStep<TextPrompt>
    {
        public ConfirmNameStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected override TextPrompt CreatePrompt()
        {
            return null;
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hello {stepContext.Result}!"), cancellationToken);

            var userProfile = await Accessors.UserProfile_Accessor.GetAsync(stepContext.Context, () => new UserProfileData(), cancellationToken);

            userProfile.Name = (string)stepContext.Result;

            // alternatively, here we could initiate another dialog (and we should, really)
            return await stepContext.EndDialogAsync(cancellationToken);
        }

    }
}
