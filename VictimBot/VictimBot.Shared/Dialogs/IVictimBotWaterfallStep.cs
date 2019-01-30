using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace VictimBot.Shared.Dialogs
{
    public interface IVictimBotWaterfallStep
    {
        string StepId { get; }
        Dialog Prompt { get; }
        WaterfallStep Step { get; }

        void RegisterWith(DialogSet set);

    }
}
