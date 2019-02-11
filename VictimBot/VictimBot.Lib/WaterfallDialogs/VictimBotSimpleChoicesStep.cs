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
using VictimBot.Lib.Helpers;
using VictimBot.Lib.State;

namespace VictimBot.Lib.WaterfallDialogs
{
    public abstract class VictimBotSimpleChoicesStep : VictimBotWaterfallStep<ChoicePrompt>
    {
        public VictimBotSimpleChoicesStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected override ChoicePrompt CreatePrompt(string stepId)
        {
            return new ChoicePrompt(this.GetStepId(), ValidateChoice);
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choices = await GenerateChoicesAsync(stepContext);

            var message = MessageFactory.SuggestedActions(choices.Select(c => c.Value), await GeneratePromptAsync(stepContext));
            var message_retry = MessageFactory.SuggestedActions(choices.Select(c => c.Value), await GenerateRepromptAsync(stepContext));

            return await stepContext.PromptAsync(
                this.GetStepId(),
                new PromptOptions()
                {
                    Prompt = message as Activity,
                    RetryPrompt = message_retry as Activity,
                    Choices = choices,
                },
                cancellationToken);
        }

        protected abstract Task<string> GeneratePromptAsync(WaterfallStepContext stepContext = null);

        protected abstract Task<string> GenerateRepromptAsync(WaterfallStepContext stepContext = null);

        protected abstract Task<IList<Choice>> GenerateChoicesAsync(WaterfallStepContext stepContext = null);

        protected async Task<bool> ValidateChoice(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded) { return false; }
            var found = promptContext.Recognized.Value;
            var choices = await GenerateChoicesAsync();
            return choices.Select(c => c.Value).Contains(found.Value);
        }
    }
}
