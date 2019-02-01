using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;

namespace VictimBot.Lib.Dialogs
{
    public abstract class VictimBotWaterfallStep<PromptType> : IVictimBotWaterfallStep
        where PromptType : Dialog
    {
        public Dialog Prompt { get; private set; }
        public WaterfallStep Step { get; private set; }

        protected VictimBotWaterfallStep(VictimBotAccessors accessors)
        {
            Accessors = accessors;
            Prompt = CreatePrompt(this.GetStepId());
            Step = StepAsync;
        }

        protected VictimBotAccessors Accessors { get; private set; }

        protected abstract PromptType CreatePrompt(string stepId);

        protected abstract Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken);

        public void RegisterWith(DialogSet set)
        {
            set.Add(Prompt);
        }
    }
}
