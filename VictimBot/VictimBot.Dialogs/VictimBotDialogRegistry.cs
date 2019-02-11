using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;
using VictimBot.Dialogs.Dialogs;
using VictimBot.Dialogs.Dialogs.MainChoices;
using VictimBot.Dialogs.Dialogs.PersonalDetails;
using VictimBot.Lib.Helpers;
using VictimBot.Dialogs.Dialogs.RecordBurglary;
using VictimBot.Dialogs.Dialogs.ReviewBurglary;
using VictimBot.Dialogs.Dialogs.SelectIncident;

namespace VictimBot.Lib
{
    /// <summary>
    /// Holds all dialogs used by the bot - and registers them all with a DialogSet
    /// </summary>
    public class VictimBotDialogRegistry
    {
        /// <summary>
        /// A list of IVictimBotWaterfallDialogs to instantiate and use
        /// </summary>
        public static readonly IEnumerable<Type> RegisteredClasses = new Type[]
        {
            typeof(LearnPersonalDetailsDialog),
            typeof(MainChoicesDialog),
            typeof(RecordNewBurglaryDialog),
            typeof(ReviewBurglaryDialog),
            typeof(SelectIncidentDialog)
        };

        /// <summary>
        /// Id of the dialog to use to record vital identity information for the user.
        /// </summary>
        public string RegistrationDialogId => typeof(LearnPersonalDetailsDialog).CalcDialogId();

        /// <summary>
        /// Id of the dialog to offer the user their main choices.
        /// </summary>
        public string MainChoicesDialogId => typeof(MainChoicesDialog).CalcDialogId();

        /// <summary>
        /// Standard constructor - prepares dialogs with the accessors provided
        /// </summary>
        public VictimBotDialogRegistry(VictimBotAccessors accessors)
        {
            Dialogs = GenerateDialogs(accessors);
            DialogSet = new DialogSet(accessors.DialogState_Accessor);

            RegisterDialogs();
        }

        /// <summary>
        /// A list of all the Dialogs to register
        /// </summary>
        public IEnumerable<IVictimBotDialog> Dialogs { get; private set; }

        /// <summary>
        /// A DialogSet with all the Dialogs fully registered
        /// </summary>
        public DialogSet DialogSet { get; private set; }

        /// <summary>
        /// Creates all the Dialogs from their specified classes
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IVictimBotDialog> GenerateDialogs(VictimBotAccessors accessors)
        {
            return RegisteredClasses.Select(type => 
                (IVictimBotDialog) type
                    .GetConstructor(new Type[] { typeof(VictimBotAccessors) })
                    .Invoke(new object[] { accessors }));
        }

        /// <summary>
        /// Registers all the dialogs with the DialogSet for use by the bot
        /// </summary>
        private void RegisterDialogs()
        {
            foreach (var dialog in Dialogs)
            {
                dialog.RegisterWith(DialogSet);
            }
        }

    }
}
