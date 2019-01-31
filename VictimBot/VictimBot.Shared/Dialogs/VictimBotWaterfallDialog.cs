using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VictimBot.Shared.Dialogs;
using VictimBot.Shared.State;

namespace VictimBot.Shared.Dialogs
{
    public abstract class VictimBotWaterfallDialog : IVictimBotDialog
    {
        public static string CalcDialogId(Type type) { return type.FullName; }

        public string DialogId { get { return GetType().FullName; } }

        public Dialog Dialog { get; private set; }

        protected VictimBotAccessors Accessors { get; private set; }

        protected IEnumerable<IVictimBotWaterfallStep> DialogSteps { get; private set; }

        protected VictimBotWaterfallDialog(VictimBotAccessors accessors)
        {
            this.Accessors = accessors;
            this.DialogSteps = CreateDialogSteps();
            this.Dialog = new WaterfallDialog(DialogId, DialogSteps.Select(s => s.Step));
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
