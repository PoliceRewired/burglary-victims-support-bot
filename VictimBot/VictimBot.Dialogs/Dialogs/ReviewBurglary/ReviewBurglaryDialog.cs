using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Dialogs.Summaries;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;
using VictimBot.Lib.WaterfallDialogs;

namespace VictimBot.Dialogs.Dialogs.ReviewBurglary
{
    public class ReviewBurglaryDialog : VictimBotWaterfallDialog
    {
        public ReviewBurglaryDialog(VictimBotAccessors accessors) : base(accessors) { }

        protected override IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps()
        {
            return new IVictimBotWaterfallStep[]
            {
                new DisplayCurrentStateStep(Accessors),     // show the current state of the burglary
                //new PresentOptionsStep(Accessors),          // allow the user to modify something or finish
                //new ParseSelectionStep(Accessors)           // either enter a modification flow or finish up
            };
        }
    }

    public class DisplayCurrentStateStep : VictimBotSimpleTextStep
    {
        public DisplayCurrentStateStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected override async Task<string> GenerateTextAsync(WaterfallStepContext context, string userInput)
        {
            var incident = await GetCurrentIncidentAsync(context.Context, true);
            return incident.Summarise();
        }
    }


    // TODO: the main steps for review

}
