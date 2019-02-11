using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Lib.Constants;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;
using VictimBot.Dialogs.Dialogs.ReviewBurglary;
using VictimBot.Lib.Storage.DTO;
using VictimBot.Lib.WaterfallDialogs;

namespace VictimBot.Dialogs.Dialogs.RecordBurglary
{
    public class RecordNewBurglaryDialog : VictimBotWaterfallDialog
    {
        public RecordNewBurglaryDialog(VictimBotAccessors accessors) : base(accessors) { }

        protected override IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps()
        {
            return new IVictimBotWaterfallStep[]
            {
                new ConfirmIntentionStep(Accessors),
                new ParseIntentConfirmationStep(Accessors)
            };
        }
    }

    public class ConfirmIntentionStep : VictimBotSimpleChoicesStep
    {
        public ConfirmIntentionStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected async override Task<IList<Choice>> GenerateChoicesAsync()
        {
            return new List<Choice>()
            {
                new Choice(RecordNewBurglaryResources.Choice_NewReport),
                new Choice(RecordNewBurglaryResources.Choice_Back)
            };
        }

        protected async override Task<string> GeneratePromptAsync()
        {
            return RecordNewBurglaryResources.ConfirmIntention_Preamble;
        }

        protected async override Task<string> GenerateRepromptAsync()
        {
            return RecordNewBurglaryResources.ConfirmIntention_Retry;
        }
    }

    public class ParseIntentConfirmationStep : VictimBotParseChoicesStep
    {
        public ParseIntentConfirmationStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected async override Task<DialogTurnResult> ParseChoiceAsync(FoundChoice choice, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (RecordNewBurglaryResources.Choice_NewReport == choice.Value)
            {
                var channelId = stepContext.Context.Activity.ChannelId;
                var userProfile = await Accessors.UserProfile_Accessor.GetAsync(stepContext.Context,
                    () => Accessors.Storage.UserProfiles.ReadOrCreateAsync(channelId).Result,
                    cancellationToken);
                var ownerGuid = userProfile.Guid;
                await Accessors.CurrentIncident_Accessor.SetAsync(stepContext.Context, IncidentData.NewBurglary(ownerGuid), cancellationToken);
                return await stepContext.ReplaceDialogAsync(typeof(ReviewBurglaryDialog).CalcDialogId(), null, cancellationToken);
            }
            else if (RecordNewBurglaryResources.Choice_Back == choice.Value)
            {
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(RecordNewBurglaryResources.CannotConfirmIntent), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
        }
    }
}
