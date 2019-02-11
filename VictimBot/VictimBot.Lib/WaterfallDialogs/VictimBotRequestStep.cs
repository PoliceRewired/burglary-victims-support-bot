using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.State;

namespace VictimBot.Lib.WaterfallDialogs
{
    public abstract class VictimBotRequestStep : VictimBotWaterfallStep<TextPrompt>
    {
        public VictimBotRequestStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return new TextPrompt(stepId, ValidateInputAsync);
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                this.GetStepId(),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(await GeneratePromptAsync()),
                    RetryPrompt = MessageFactory.Text(await GenerateRepromptAsync())
                },
                cancellationToken);
        }

        protected abstract Task<string> GeneratePromptAsync();
        protected abstract Task<string> GenerateRepromptAsync();

        protected abstract Task<bool> ValidateInputAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellation);
    }
}
