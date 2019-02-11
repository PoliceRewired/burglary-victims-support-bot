using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Lib.State;

namespace VictimBot.Lib.WaterfallDialogs
{
    public abstract class VictimBotSimpleTextStep : VictimBotWaterfallStep<TextPrompt>
    {
        public VictimBotSimpleTextStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return null;
        }

        protected override async Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await OperateOnUserInputAsync(stepContext, (string)stepContext.Result, cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(await GenerateTextAsync(stepContext, (string)stepContext.Result)), cancellationToken);
            return await stepContext.NextAsync(cancellationToken);
        }

        /// <summary>
        /// Performs actions on user input - such as storing it. Override to implement.
        /// </summary>
        protected virtual async Task OperateOnUserInputAsync(WaterfallStepContext stepContext, string userInput, CancellationToken cancellationToken)
        {
        }

        protected abstract Task<string> GenerateTextAsync(WaterfallStepContext context, string userInput);
    }
}
