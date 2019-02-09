using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using VictimBot.Dialogs.Dialogs.RecordBurglary;
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
        public static readonly string CHOICE_Review = MainChoicesResources.Choice_ReviewIncidents;
        public static readonly string CHOICE_ReportNewBurglary = MainChoicesResources.Choice_ReportBurglary;
        public static readonly string CHOICE_UpdatePersonals = MainChoicesResources.Choice_UpdatePersonals;

        public OfferMainChoicesStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override ChoicePrompt CreatePrompt(string stepId)
        {
            return new ChoicePrompt(stepId, ValidateChoice);
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var actions = new List<CardAction>()
            {
                new CardAction() { Title = CHOICE_Review, Type = ActionTypes.ImBack, Value = CHOICE_Review },
                new CardAction() { Title = CHOICE_ReportNewBurglary, Type = ActionTypes.ImBack, Value = CHOICE_ReportNewBurglary },
                new CardAction() { Title = CHOICE_UpdatePersonals, Type = ActionTypes.ImBack, Value = CHOICE_UpdatePersonals },
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

        protected async Task<bool> ValidateChoice(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken)
        {
            var selection = promptContext.Context.Activity.Text;
            return new[]{ CHOICE_Review, CHOICE_ReportNewBurglary, CHOICE_UpdatePersonals }.Contains(selection);
        }
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
            var selection = stepContext.Context.Activity.Text;

            if (OfferMainChoicesStep.CHOICE_Review == selection)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(MainChoicesResources.ReviewNotImplemented), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else if (OfferMainChoicesStep.CHOICE_UpdatePersonals == selection)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(MainChoicesResources.UpdatePersonalsNotImplemented), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else if (OfferMainChoicesStep.CHOICE_ReportNewBurglary == selection)
            {
                return await stepContext.ReplaceDialogAsync(typeof(RecordNewBurglaryDialog).CalcDialogId(), null, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(MainChoicesResources.UnrecognisedOption), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
        }
    }
}
