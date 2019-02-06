using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using VictimBot.Lib.Dialogs;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;

namespace VictimBot.Dialogs.Dialogs.PersonalDetails
{
    /// <summary>
    /// Responsible for learning the user's name and a good contact for them
    /// </summary>
    public class LearnPersonalDetailsDialog : VictimBotWaterfallDialog
    {
        public LearnPersonalDetailsDialog(VictimBotAccessors accessors) : base(accessors) { }

        protected override IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps()
        {
            return new IVictimBotWaterfallStep[]
            {
                new SayPreambleStep(Accessors),
                new AskNameStep(Accessors),
                new ConfirmNameStep(Accessors),
                new AskEmailStep(Accessors),
                new ConfirmEmailStep(Accessors)

            };
        }
    }

    public class SayPreambleStep : VictimBotWaterfallStep<TextPrompt>
    {
        public SayPreambleStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return null;
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // TODO - create Attachment image, using bot with clipboard image
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(LearnPersonalDetailsResources.Preamble_Explanation), cancellationToken);
            return await stepContext.NextAsync(cancellationToken);
        }
    }

    public class AskNameStep : VictimBotWaterfallStep<TextPrompt>
    {
        public AskNameStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return new TextPrompt(stepId, ValidateName);
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                this.GetStepId(),
                new PromptOptions {
                    Prompt = MessageFactory.Text(LearnPersonalDetailsResources.AskForName),
                    RetryPrompt = MessageFactory.Text($"{LearnPersonalDetailsResources.AskForName_Retry} {SharedResources.OfferToAbort}")
                },
                cancellationToken);
        }

        protected async Task<bool> ValidateName(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken) => promptContext.Recognized.Succeeded && !string.IsNullOrWhiteSpace(promptContext.Recognized.Value);
    }

    public class ConfirmNameStep : VictimBotWaterfallStep<TextPrompt>
    {
        public ConfirmNameStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return null;
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var statedName = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format(LearnPersonalDetailsResources.ConfirmName, statedName)), cancellationToken);

            var userProfile = await Accessors.UserProfile_Accessor.GetAsync(stepContext.Context, () => new UserProfileData(), cancellationToken);
            userProfile.RawName = statedName;

            return await stepContext.NextAsync(cancellationToken);
        }
    }

    public class AskEmailStep : VictimBotWaterfallStep<TextPrompt>
    {
        public AskEmailStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return new TextPrompt(stepId, ValidateEmail);
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var stepId = this.GetStepId();
            return await stepContext.PromptAsync(
                this.GetStepId(),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(LearnPersonalDetailsResources.AskForEmailAddress),
                    RetryPrompt = MessageFactory.Text($"{LearnPersonalDetailsResources.AskForEmailAddress_Retry} {SharedResources.OfferToAbort}")
                },
                cancellationToken);
        }

        protected async Task<bool> ValidateEmail(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken) => promptContext.Recognized.Succeeded && promptContext.Recognized.Value.IsEmailAddress();
    }

    public class ConfirmEmailStep : VictimBotWaterfallStep<TextPrompt>
    {
        public ConfirmEmailStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return null;
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var statedEmail = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format(LearnPersonalDetailsResources.ConfirmEmail, statedEmail)), cancellationToken);

            var userProfile = await Accessors.UserProfile_Accessor.GetAsync(stepContext.Context, () => new UserProfileData(), cancellationToken);
            userProfile.Email = statedEmail;

            // end of sequence
            return await stepContext.EndDialogAsync(cancellationToken);
        }
    }

}
