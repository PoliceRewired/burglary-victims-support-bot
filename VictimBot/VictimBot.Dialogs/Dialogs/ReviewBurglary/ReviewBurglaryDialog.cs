using System;
using System.Collections.Generic;
using System.Text;
using VictimBot.Lib.Dialogs;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;

namespace VictimBot.Dialogs.Dialogs.ReviewBurglary
{
    public class ReviewBurglaryDialog : VictimBotWaterfallDialog
    {
        public ReviewBurglaryDialog(VictimBotAccessors accessors) : base(accessors) { }

        protected override IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps()
        {
            return new IVictimBotWaterfallStep[]
            {
                //new DisplayCurrentStateStep(Accessors),     // show the current state of the burglary
                //new PresentOptionsStep(Accessors),          // allow the user to modify something or finish
                //new ParseSelectionStep(Accessors)           // either enter a modification flow or finish up
            };
        }
    }

    // TODO: the main steps for review

}
