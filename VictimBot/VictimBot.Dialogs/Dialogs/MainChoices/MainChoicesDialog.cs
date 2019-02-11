using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using VictimBot.Dialogs.Dialogs.RecordBurglary;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;
using VictimBot.Lib.WaterfallDialogs;

namespace VictimBot.Dialogs.Dialogs.MainChoices
{
    public class MainChoicesDialog : VictimBotWaterfallDialog
    {
        public MainChoicesDialog(VictimBotAccessors accessors) : base(accessors) { }

        protected override IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps()
        {
            return new IVictimBotWaterfallStep[]
            {
                new OfferMainChoicesStep(Accessors),
                new ParseMainChoiceStep(Accessors)
            };
        }
    }

    public class OfferMainChoicesStep : VictimBotSimpleChoicesStep
    {
        public OfferMainChoicesStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected async override Task<IList<Choice>> GenerateChoicesAsync(WaterfallStepContext context)
        {
            return new List<Choice>()
            {
                new Choice(MainChoicesResources.Choice_ReviewIncidents),
                new Choice(MainChoicesResources.Choice_ReportBurglary),
                new Choice(MainChoicesResources.Choice_UpdatePersonals)
            };
        }

        protected async override Task<string> GeneratePromptAsync(WaterfallStepContext context)
        {
            return MainChoicesResources.OfferOptionsPreamble;
        }

        protected async override Task<string> GenerateRepromptAsync(WaterfallStepContext context)
        {
            return MainChoicesResources.OfferOptions_Retry;
        }
    }

    public class ParseMainChoiceStep : VictimBotParseChoicesStep
    {
        public ParseMainChoiceStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected override async Task<DialogTurnResult> ParseChoiceAsync(FoundChoice choice, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (MainChoicesResources.Choice_ReviewIncidents == choice.Value)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(MainChoicesResources.ReviewNotImplemented), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else if (MainChoicesResources.Choice_UpdatePersonals == choice.Value)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(MainChoicesResources.UpdatePersonalsNotImplemented), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else if (MainChoicesResources.Choice_ReportBurglary == choice.Value)
            {
                return await stepContext.ReplaceDialogAsync(typeof(RecordNewBurglaryDialog).CalcDialogId(), null, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(MainChoicesResources.UnrecognisedOption), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
        }
    }
}
