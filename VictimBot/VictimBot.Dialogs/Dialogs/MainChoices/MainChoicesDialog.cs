using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using VictimBot.Lib.Dialogs;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;

namespace VictimBot.Dialogs.Dialogs.MainChoices
{
    public class MainChoicesDialog : VictimBotWaterfallDialog
    {
        public MainChoicesDialog(VictimBotAccessors accessors) : base(accessors) { }

        protected override IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps()
        {
            return new IVictimBotWaterfallStep[]
            {
                new OfferMainChoicesStep(Accessors),
                new ParseMainChoiceStep(Accessors)
            };
        }
    }

    public class OfferMainChoicesStep : VictimBotWaterfallStep<ChoicePrompt>
    {
        public OfferMainChoicesStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override ChoicePrompt CreatePrompt(string stepId)
        {
            return new ChoicePrompt(stepId, ValidateChoice);
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var actions = new List<CardAction>()
            {
                new CardAction() { Title = MainChoicesResources.Choice_ReviewIncidents, Type = ActionTypes.PostBack, Value = "ReviewIncidents" },
                new CardAction() { Title = MainChoicesResources.Choice_ReportBurglary, Type = ActionTypes.PostBack, Value = "ReportBurglary" },
                new CardAction() { Title = MainChoicesResources.Choice_UpdatePersonals, Type = ActionTypes.PostBack, Value = "UpdatePersonals" },
            };

            var message = MessageFactory.SuggestedActions(actions, MainChoicesResources.OfferOptionsPreamble);
            var message_retry = MessageFactory.SuggestedActions(actions, MainChoicesResources.OfferOptions_Retry);

            return await stepContext.PromptAsync(
                this.GetStepId(),
                new PromptOptions
                {
                    Prompt = (Activity)message,
                    RetryPrompt = (Activity)message_retry
                },
                cancellationToken);
        }

        protected async Task<bool> ValidateChoice(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken) => true;
    }

    public class ParseMainChoiceStep : VictimBotWaterfallStep<TextPrompt>
    {
        public ParseMainChoiceStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return null;
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var statedChoice = stepContext.Context.Activity.Text;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format(MainChoicesResources.ConfirmChoice, statedChoice)), cancellationToken);

            // end of sequence
            return await stepContext.EndDialogAsync(cancellationToken);
        }
    }
}
