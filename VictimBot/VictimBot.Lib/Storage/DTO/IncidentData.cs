using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VictimBot.Lib.Constants;

namespace VictimBot.Lib.Storage.DTO
{
    /// <summary>
    /// All details of a specific incident.
    /// </summary>
    public class IncidentData : IStoreItem
    {
        public Guid IncidentGuid { get; set; } = Guid.NewGuid();

        public Guid OwnerGuid { get; set; }

        public IncidentType Type { get; set; }

        public DateTime? ReportStarted { get; set; }
        public DateTime? ReportCompleted { get; set; }
        public DateTime? PoliceNotified { get; set; }

        public Address Location { get; set; }
        public TimeWindow TimeWindow { get; set; }
        public List<Victim> Victims { get; set; } = new List<Victim>();
        public List<Witness> Witnesses { get; set; } = new List<Witness>();
        public List<Involved> Involved { get; set; } = new List<Involved>();
        public List<Property> Stolen { get; set; } = new List<Property>();
        public List<Property> Damaged { get; set; } = new List<Property>();
        public List<Suspect> Suspects { get; set; } = new List<Suspect>();
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public string ETag { get; set; } = "*";

        public static IncidentData NewBurglary(Guid owner)
        {
            return new IncidentData()
            {
                OwnerGuid = owner,
                Type = IncidentType.Burglary,
                ReportStarted = DateTime.Now
            };
        }

        public static IncidentData NewUnspecified(Guid owner)
        {
            return new IncidentData()
            {
                OwnerGuid = owner,
                Type = IncidentType.Unspecified,
                ReportStarted = DateTime.Now
            };
        }
    }
}
