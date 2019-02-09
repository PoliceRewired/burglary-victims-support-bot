using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VictimBot.Lib.Constants;

namespace VictimBot.Lib.State
{
    public class IncidentData
    {
        public Guid IncidentId { get; set; } = Guid.NewGuid();

        public IncidentType Type { get; set; }


        public static IncidentData NewBurglary()
        {
            return new IncidentData()
            {
                Type = IncidentType.Burglary
            };

        }
    }
}
