using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace VictimBot.Lib.Interfaces
{
    public interface IVictimBotWaterfallStep
    {
        Dialog Prompt { get; }
        WaterfallStep Step { get; }
    }
}
