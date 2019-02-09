using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Lib.Dialogs;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;

namespace VictimBot.Dialogs.Dialogs.RecordBurglary
{
    public class RecordNewBurglaryDialog : VictimBotWaterfallDialog
    {
        public RecordNewBurglaryDialog(VictimBotAccessors accessors) : base(accessors) { }

        protected override IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps()
        {
            return new IVictimBotWaterfallStep[]
            {
                new ConfirmIntentionStep(Accessors),
                new ParseIntentConfirmationStep(Accessors)
            };
        }
    }

    public class ConfirmIntentionStep : VictimBotWaterfallStep<ChoicePrompt>
    {
        public static readonly string CHOICE_NewReport = RecordNewBurglaryResources.Choice_NewReport;
        public static readonly string CHOICE_Back = RecordNewBurglaryResources.Choice_Back;

        public ConfirmIntentionStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override ChoicePrompt CreatePrompt(string stepId)
        {
            return new ChoicePrompt(stepId, ValidateChoice);
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var actions = new List<CardAction>()
            {
                new CardAction() { Title = CHOICE_NewReport, Type = ActionTypes.ImBack, Value = CHOICE_NewReport },
                new CardAction() { Title = CHOICE_Back, Type = ActionTypes.ImBack, Value = CHOICE_Back },
            };

            var message = MessageFactory.SuggestedActions(actions, RecordNewBurglaryResources.ConfirmIntention_Preamble);
            var message_retry = MessageFactory.SuggestedActions(actions, RecordNewBurglaryResources.ConfirmIntention_Retry);

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
            // confirm it was one of the choices available
            var selection = promptContext.Context.Activity.Text;
            return new[] { CHOICE_Back, CHOICE_NewReport }.Contains(selection);
        }
    }

    public class ParseIntentConfirmationStep : VictimBotWaterfallStep<TextPrompt>
    {
        public ParseIntentConfirmationStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return null;
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var statedChoice = stepContext.Context.Activity.Text;

            if (ConfirmIntentionStep.CHOICE_NewReport == statedChoice)
            {
                return await stepContext.NextAsync(cancellationToken);
            }
            else if (ConfirmIntentionStep.CHOICE_Back == statedChoice)
            {
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(RecordNewBurglaryResources.CannotConfirmIntent), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
        }
    }


}
