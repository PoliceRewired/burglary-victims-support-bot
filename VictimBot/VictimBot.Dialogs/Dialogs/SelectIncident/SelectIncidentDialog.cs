using System;
using System.Collections.Generic;
using System.Text;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;
using VictimBot.Lib.WaterfallDialogs;

namespace VictimBot.Dialogs.Dialogs.SelectIncident
{
    public class SelectIncidentDialog : VictimBotWaterfallDialog
    {
        public SelectIncidentDialog(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected override IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps()
        {
            return new IVictimBotWaterfallStep[]
            {
                //new OfferIncidentsStep(Accessors),    // show the current incidents the user has on record
                //new ParseSelectionStep(Accessors)     // allow the user to select an incident (or create a new one)
            };
        }
    }

    // TODO: implement these steps
}
