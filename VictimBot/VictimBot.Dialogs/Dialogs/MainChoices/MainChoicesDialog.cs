using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
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

    public class OfferMainChoicesStep : VictimBotWaterfallStep<TextPrompt>
    {
        public OfferMainChoicesStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return new TextPrompt(stepId, ValidateChoice);
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                this.GetStepId(),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(MainChoicesResources.OfferOptionsPreamble),
                    RetryPrompt = MessageFactory.Text($"{MainChoicesResources.OfferOptions_Retry}")
                },
                cancellationToken);
        }

        protected async Task<bool> ValidateChoice(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken) => true;
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
            var statedChoice = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format(MainChoicesResources.ConfirmChoice, statedChoice)), cancellationToken);

            // end of sequence
            return await stepContext.EndDialogAsync(cancellationToken);
        }
    }
}
