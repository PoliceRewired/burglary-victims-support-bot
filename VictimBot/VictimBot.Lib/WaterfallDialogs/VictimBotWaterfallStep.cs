using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Lib.Helpers;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;
using VictimBot.Lib.Storage.DTO;

namespace VictimBot.Lib.WaterfallDialogs
{
    public abstract class VictimBotWaterfallStep<PromptType> : IVictimBotWaterfallStep
        where PromptType : Dialog
    {
        public Dialog Prompt { get; private set; }
        public WaterfallStep Step { get; private set; }

        protected VictimBotWaterfallStep(VictimBotAccessors accessors)
        {
            Accessors = accessors;
            Prompt = CreatePrompt(this.GetStepId());
            Step = WrapStepAsync;
        }

        protected VictimBotAccessors Accessors { get; private set; }

        protected abstract PromptType CreatePrompt(string stepId);

        protected abstract Task<DialogTurnResult> StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken);

        protected async Task<DialogTurnResult> WrapStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // TODO: interject in StepAsync, identify universal 'stop' interruptions from the user and end the dialog immediately
            return await StepAsync(stepContext, cancellationToken);
        }

        protected async Task<IncidentData> GetCurrentIncidentAsync(ITurnContext context, bool throwIfNull = true)
        {
            return await Accessors.CurrentIncident_Accessor.GetAsync(
                context, 
                () => 
                {
                    if (throwIfNull)
                    {
                        throw new NullReferenceException("UserProfile cannot be null.");
                    }
                    else
                    {
                        var user = GetCurrentUserAsync(context, throwIfNull).Result;
                        return IncidentData.NewUnspecified(user.Guid);
                    }
                });
        }

        protected async Task<UserProfileData> GetCurrentUserAsync(ITurnContext context, bool throwIfNull = false)
        {
            return await Accessors.UserProfile_Accessor.GetAsync(
                context,
                () =>
                {
                    if (throwIfNull)
                    {
                        throw new NullReferenceException("Incident cannot be null.");
                    }
                    else
                    {
                        return new UserProfileData(context.Activity.ChannelId);
                    }
                });
        }

    }
}
