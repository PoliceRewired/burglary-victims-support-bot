using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VictimBot.Dialogs.Summaries;
using VictimBot.Lib.Interfaces;
using VictimBot.Lib.State;
using VictimBot.Lib.Storage.DTO;
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
                new MakeRecommendationStep(Accessors),      // offer the 'next logical thing to do'
                //new ParseRecommendationStep(Accessors)
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

    public class RecommendationAndChoices
    {
        public string Recommendation { get; set; }
        public IEnumerable<string> Choices { get; set; }
    }

    public class MakeRecommendationStep : VictimBotSimpleChoicesStep
    {
        public MakeRecommendationStep(VictimBotAccessors accessors) : base(accessors)
        {
        }

        private RecommendationAndChoices RecommendFor(IncidentData incident)
        {
            if (incident.Location == null)
            {
                return new RecommendationAndChoices()
                {
                    Recommendation = ReviewBurglaryResources.Recommendation_AddLocation,
                    Choices = new []
                    {
                        ReviewBurglaryResources.Choice_AddLocation,
                        ReviewBurglaryResources.Reject_AddLocation,
                        ReviewBurglaryResources.Negative_AddLocation
                    }
                };
            }

            if (incident.Stolen.Count == 0)
            {
                return new RecommendationAndChoices()
                {
                    Recommendation = ReviewBurglaryResources.Recommendation_AddStolenProperty,
                    Choices = new[]
                    {
                        ReviewBurglaryResources.Choice_AddStolenProperty,
                        ReviewBurglaryResources.Reject_AddStolenProperty,
                        ReviewBurglaryResources.Negative_AddStolenProperty
                    }
                };
            }

            // TODO: the other components of a report:
            // TimeWindow, DamagedProperty, Victims, Witnesses, Involved, Suspects, Vehicles (part of suspects?)

            return new RecommendationAndChoices()
            {
                Recommendation = ReviewBurglaryResources.Recommendation_ReviewAllOptions,
                Choices = new[]
                {
                    ReviewBurglaryResources.Choice_ReviewAllOptions,
                    ReviewBurglaryResources.Choice_CloseReport
                }
            };
        }

        protected async override Task<IList<Choice>> GenerateChoicesAsync(WaterfallStepContext context)
        {
            var incident = await GetCurrentIncidentAsync(context.Context, true);
            var outputs = RecommendFor(incident);
            return outputs.Choices.Select(s => new Choice(s)).ToList();
        }

        protected async override Task<string> GeneratePromptAsync(WaterfallStepContext stepContext)
        {
            var incident = await GetCurrentIncidentAsync(stepContext.Context);
            var outputs = RecommendFor(incident);
            return outputs.Recommendation;
        }

        protected async override Task<string> GenerateRepromptAsync(WaterfallStepContext stepContext)
        {
            var incident = await GetCurrentIncidentAsync(stepContext.Context);
            var outputs = RecommendFor(incident);
            return outputs.Recommendation;
        }
    }

    // TODO: the main steps for review

}
