using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Lib.State;

namespace VictimBot.Lib.WaterfallDialogs
{
    public abstract class VictimBotParseChoicesStep : VictimBotWaterfallStep<TextPrompt>
    {
        protected VictimBotParseChoicesStep(VictimBotAccessors accessors) : base(accessors) { }

        protected override TextPrompt CreatePrompt(string stepId)
        {
            return null;
        }

        protected override async Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var selection = stepContext.Context.Activity.Text; // what the user typed
            var choice = stepContext.Result as FoundChoice;
            return await ParseChoiceAsync(choice, stepContext, cancellationToken);
        }

        protected abstract Task<DialogTurnResult> ParseChoiceAsync(FoundChoice choice, WaterfallStepContext stepContext, CancellationToken cancellationToken);

    }
}
