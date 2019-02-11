using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;
using VictimBot.Lib.WaterfallDialogs;

namespace VictimBot.Dialogs.Dialogs.PersonalDetails
{
    /// <summary>
    /// Responsible for learning the user's name and a good contact email address for them.
    /// </summary>
    public class LearnPersonalDetailsDialog : VictimBotWaterfallDialog
    {
        public LearnPersonalDetailsDialog(VictimBotAccessors accessors) : base(accessors) { }

        protected override IEnumerable<IVictimBotWaterfallStep> CreateDialogSteps()
        {
            return new IVictimBotWaterfallStep[]
            {
                new SayPreambleStep(Accessors),
                new AskNameStep(Accessors),
                new ConfirmNameStep(Accessors),
                new AskEmailStep(Accessors),
                new ConfirmEmailStep(Accessors)

            };
        }
    }

    public class SayPreambleStep : VictimBotSimpleTextStep
    {
        public SayPreambleStep(VictimBotAccessors accessors) : base(accessors) { }

        protected async override Task<string> GenerateTextAsync(WaterfallStepContext context, string userInput)
        {
            return LearnPersonalDetailsResources.Preamble_Explanation;
        }
    }

    public class AskNameStep : VictimBotRequestNameStep
    {
        public AskNameStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected async override Task<string> GeneratePromptAsync()
        {
            return LearnPersonalDetailsResources.AskForName;
        }

        protected async override Task<string> GenerateRepromptAsync()
        {
            return LearnPersonalDetailsResources.AskForName_Retry;
        }
    }

    public class ConfirmNameStep : VictimBotSimpleTextStep
    {
        public ConfirmNameStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected async override Task<string> GenerateTextAsync(WaterfallStepContext context, string userInput)
        {
            var userProfile = await GetCurrentUserAsync(context.Context);
            return string.Format(LearnPersonalDetailsResources.ConfirmName, userProfile.FullName.FirstName);
        }

        protected async override Task OperateOnUserInputAsync(WaterfallStepContext stepContext, string userInput, CancellationToken cancellationToken)
        {
            var channelId = stepContext.Context.Activity.ChannelId;
            var userProfile = await GetCurrentUserAsync(stepContext.Context);
            userProfile.RawName = userInput;
        }
    }

    public class AskEmailStep : VictimBotRequestEmailStep
    {
        public AskEmailStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected async override Task<string> GeneratePromptAsync()
        {
            return LearnPersonalDetailsResources.AskForEmailAddress;
        }

        protected async override Task<string> GenerateRepromptAsync()
        {
            return LearnPersonalDetailsResources.AskForEmailAddress_Retry;
        }
    }

    public class ConfirmEmailStep : VictimBotSimpleTextStep
    {
        public ConfirmEmailStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        protected async override Task<string> GenerateTextAsync(WaterfallStepContext context, string userInput)
        {
            return string.Format(LearnPersonalDetailsResources.ConfirmEmail, userInput);
        }

        protected async override Task OperateOnUserInputAsync(WaterfallStepContext stepContext, string userInput, CancellationToken cancellationToken)
        {
            var channelId = stepContext.Context.Activity.ChannelId;
            var userProfile = await GetCurrentUserAsync(stepContext.Context);

            userProfile.Email = userInput;
            await Accessors.Storage.UserProfiles.WriteAsync(userProfile);
        }
    }
}
