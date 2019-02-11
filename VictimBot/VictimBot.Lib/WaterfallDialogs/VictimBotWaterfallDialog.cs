using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.State;

namespace VictimBot.Lib.WaterfallDialogs
{
    public abstract class VictimBotWaterfallDialog : IVictimBotDialog
    {
        public Dialog Dialog { get; private set; }

        protected VictimBotAccessors Accessors { get; private set; }

        protected IEnumerable<IVictimBotWaterfallStep> DialogSteps { get; private set; }

        protected VictimBotWaterfallDialog(VictimBotAccessors accessors)
        {
            this.Accessors = accessors;
            this.DialogSteps = CreateDialogSteps();
            this.Dialog = new WaterfallDialog(this.GetDialogId(), DialogSteps.Select(s => s.Step));
        }

        protected abstract IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps();

        public void RegisterWith(DialogSet dialogs)
        {
            dialogs.Add(Dialog);
            foreach (var step in DialogSteps.Where(d => d.Prompt != null))
            {
                dialogs.Add(step.Prompt);
            }
        }

    }
}
