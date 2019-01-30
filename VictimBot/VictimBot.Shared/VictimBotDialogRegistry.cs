using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VictimBot.Shared.Dialogs;
using VictimBot.Shared.Dialogs.PersonalDetails;
using VictimBot.Shared.State;

namespace VictimBot.Shared
{
    /// <summary>
    /// Holds all dialogs used by the bot - and registers them all with a DialogSet
    /// </summary>
    public class VictimBotDialogRegistry
    {
        /// <summary>
        /// A list of Dialogs to instantiate and use
        /// </summary>
        public static readonly IEnumerable<Type> RegisteredClasses = new Type[]
        {
            typeof(LearnPersonalDetailsDialog)
        };

        /// <summary>
        /// Id of the dialog to use to initiate conversation
        /// </summary>
        public string StartDialogId => VictimBotWaterfallDialog.CalcDialogId(typeof(LearnPersonalDetailsDialog));

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
        public IEnumerable<IVictimBotWaterfallDialog> Dialogs { get; private set; }

        /// <summary>
        /// A DialogSet with all the Dialogs fully registered
        /// </summary>
        public DialogSet DialogSet { get; private set; }

        /// <summary>
        /// Creates all the Dialogs from their specified classes
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IVictimBotWaterfallDialog> GenerateDialogs(VictimBotAccessors accessors)
        {
            return RegisteredClasses.Select(type => 
                (IVictimBotWaterfallDialog) type
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
