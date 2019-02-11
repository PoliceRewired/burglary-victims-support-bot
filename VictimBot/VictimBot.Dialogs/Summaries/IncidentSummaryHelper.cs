using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VictimBot.Lib.Constants;
using VictimBot.Lib.Storage.DTO;

namespace VictimBot.Dialogs.Summaries
{
    public static class IncidentSummaryHelper
    {
        public static string Summarise(this IncidentData incident)
        {
            if (incident.LooksEmpty())
            {
                return string.Format(IncidentSummaryResources.summary_empty_for_type, incident.Type.Summarise());
            }

            var entries = new List<string>();

            if (incident.Location != null) {
                if (incident.TimeWindow != null)
                {
                    entries.Add(string.Format(IncidentSummaryResources.summary_location_and_timewindow_for_type, incident.Location.Summarise(), incident.TimeWindow.Summarise(), incident.Type.Summarise()));
                }
                else
                {
                    entries.Add(string.Format(IncidentSummaryResources.summary_location_for_type, incident.Location.Summarise(), incident.Type.Summarise()));
                }
            }

            if (incident.ReportStarted != null)
            {
                if (incident.ReportCompleted != null)
                {
                    entries.Add(string.Format(IncidentSummaryResources.summary_started_and_completed, incident.ReportStarted.Summarise(), incident.ReportCompleted.Summarise()));
                }
                else
                {
                    entries.Add(string.Format(IncidentSummaryResources.summary_started_only, incident.ReportStarted.Summarise()));
                }
            }

            if (incident.Stolen.Count > 0)
            {
                entries.Add(string.Format(IncidentSummaryResources.summary_X_items_stolen_totalling, incident.Stolen.Count, incident.Stolen.TotalCost()));
            }

            if (incident.Damaged.Count > 0)
            {
                entries.Add(string.Format(IncidentSummaryResources.summary_X_items_damaged, incident.Damaged.Count));
            }

            if (incident.Victims.Count > 0)
            {
                var descriptions = incident.Victims.Select(p => p.Summarise());
                entries.Add(string.Format(IncidentSummaryResources.summary_X_victims, incident.Victims.Count, descriptions));
            }

            if (incident.Witnesses.Count > 0)
            {
                var descriptions = incident.Witnesses.Select(p => p.Summarise());
                entries.Add(string.Format(IncidentSummaryResources.summary_X_witnesses, incident.Witnesses.Count, descriptions));
            }

            if (incident.Involved.Count > 0)
            {
                var descriptions = incident.Involved.Select(p => p.Summarise());
                entries.Add(string.Format(IncidentSummaryResources.summary_X_involved, incident.Involved.Count, descriptions));
            }

            if (incident.Suspects.Count > 0)
            {
                var descriptions = incident.Suspects.Select(p => p.Summarise());
                entries.Add(string.Format(IncidentSummaryResources.summary_X_suspects, incident.Suspects.Count, descriptions));
            }

            if (incident.Vehicles.Count > 0)
            {
                var descriptions = incident.Involved.Select(v => v.Summarise());
                entries.Add(string.Format(IncidentSummaryResources.summary_X_vehicles, incident.Vehicles.Count, descriptions));
            }

            entries.RemoveAll(e => string.IsNullOrWhiteSpace(e));

            return string.Join("\n", entries.Select(e => "* " + e));
        }

        public static string TotalCost(this IEnumerable<Property> property)
        {
            var totals = new Dictionary<Currency, decimal>();

            foreach (var currency in property.Select(p => p.EstimatedCost.Currency).Distinct())
            {
                totals[currency] = property.Sum(p => p.EstimatedCost.Amount);
            }

            if (totals.Count == 0)
            {
                return IncidentSummaryResources.summary_property_cost_zero;
            }

            if (totals.Count == 1)
            {
                var currency = totals.First().Key;
                var amount = totals.First().Value;
                return string.Format(IncidentSummaryResources.summary_property_cost_X, currency.Represent(amount));
            }
            else
            {
                var symbolised = totals.Select(pair => pair.Key.Represent(pair.Value));
                return string.Format(IncidentSummaryResources.summary_property_cost_multiple, string.Join(", ", symbolised));
            }

        }

        public static string Summarise(this Suspect suspect)
        {
            return null;
        }

        public static string Summarise(this Person person)
        {
            return person.Raw;
        }

        public static string Summarise(this DateTime? datetime)
        {
            if (datetime == null) { return "unknown"; }
            return datetime.Value.ToString("dddd, dd MMMM yyyy, HH:mm");
        }

        public static string Summarise(this Address address)
        {
            return address.Raw;
        }

        public static string Summarise(this TimeWindow timewindow)
        {
            return null;
        }

        public static bool LooksEmpty(this IncidentData incident)
        {
            return
                incident.Location == null &&
                incident.TimeWindow == null &&
                incident.Victims.Count == 0 &&
                incident.Witnesses.Count == 0 &&
                incident.Involved.Count == 0 &&
                incident.Stolen.Count == 0 &&
                incident.Damaged.Count == 0 &&
                incident.Suspects.Count == 0 &&
                incident.Vehicles.Count == 0;
        }

        public static string Summarise(this IncidentType type)
        {
            switch (type)
            {
                case IncidentType.Burglary:
                    return IncidentSummaryResources.type_burglary;

                default:
                    throw new IndexOutOfRangeException("Unrecognised incident type: " + type.ToString());
            }
        }

    }
}
