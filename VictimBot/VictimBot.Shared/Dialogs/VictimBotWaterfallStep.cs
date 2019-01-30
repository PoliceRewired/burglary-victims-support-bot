using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Shared.State;

namespace VictimBot.Shared.Dialogs
{
    public abstract class VictimBotWaterfallStep<PromptType> : IVictimBotWaterfallStep
        where PromptType : Dialog
    {
        public string StepId { get { return GetType().FullName; } }
        public Dialog Prompt { get; private set; }
        public WaterfallStep Step { get; private set; }

        protected VictimBotWaterfallStep(VictimBotAccessors accessors)
        {
            Accessors = accessors;
            Prompt = CreatePrompt();
            Step = StepAsync;
        }

        protected VictimBotAccessors Accessors { get; private set; }

        protected abstract PromptType CreatePrompt();

        protected abstract Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken);

        public void RegisterWith(DialogSet set)
        {
            set.Add(Prompt);
        }
    }
}
