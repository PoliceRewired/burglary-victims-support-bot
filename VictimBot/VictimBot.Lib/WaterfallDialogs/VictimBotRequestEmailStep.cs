using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.State;

namespace VictimBot.Lib.WaterfallDialogs
{
    public abstract class VictimBotRequestEmailStep : VictimBotRequestStep
    {
        public VictimBotRequestEmailStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected async override Task<bool> ValidateInputAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellation)
        {
            return
                promptContext.Recognized.Succeeded &&
                promptContext.Recognized.Value.IsEmailAddress();
        }
    }
}
