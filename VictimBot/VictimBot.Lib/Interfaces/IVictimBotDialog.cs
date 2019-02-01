using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace VictimBot.Lib.Interfaces
{
    public interface IVictimBotDialog
    {
        Dialog Dialog { get; }

        void RegisterWith(DialogSet dialogs);
    }
}
