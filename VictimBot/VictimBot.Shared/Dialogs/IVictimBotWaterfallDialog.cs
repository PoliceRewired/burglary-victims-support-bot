using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using VictimBot.Shared.State;

namespace VictimBot.Shared.Dialogs
{
    public interface IVictimBotWaterfallDialog
    {
        WaterfallDialog Dialog { get; }

        void RegisterWith(DialogSet dialogs);
    }
}
