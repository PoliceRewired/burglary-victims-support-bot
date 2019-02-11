using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Dialogs.Summaries;
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
                new DisplayCurrentStateStep(Accessors),     // show the current state of the burglary
                //new PresentOptionsStep(Accessors),          // allow the user to modify something or finish
                //new ParseSelectionStep(Accessors)           // either enter a modification flow or finish up
            };
        }
    }

    public class DisplayCurrentStateStep : VictimBotWaterfallStep<ChoicePrompt>
    {
        public DisplayCurrentStateStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected override ChoicePrompt CreatePrompt(string stepId)
        {
            return null;
        }

        protected async override Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var incident = await Accessors.CurrentIncident_Accessor.GetAsync(stepContext.Context, () => throw new NullReferenceException("Incident cannot be null."), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(incident.Summarise()), cancellationToken);
            return await stepContext.NextAsync(cancellationToken);
        }
    }


    // TODO: the main steps for review

}
