using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using VictimBot.Lib.Storage.DTO;

namespace VictimBot.Lib.Storage.DAO
{
    public class IncidentDAO : AbstractDAO<IncidentData>
    {
        public static readonly string KEY_Incidents = "Incidents";

        public IncidentDAO(IStorage storage) : base(storage)
        {
        }

        public override IncidentData CreateNew(string key = null)
        {
            return new IncidentData(); // ignore the key
        }

        public override string GenerateKey(IncidentData item)
        {
            return item.IncidentGuid.ToString();
        }
    }
}
