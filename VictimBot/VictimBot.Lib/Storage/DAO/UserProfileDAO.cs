using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VictimBot.Lib.Storage.DTO;

namespace VictimBot.Lib.Storage.DAO
{
    public class UserProfileDAO : AbstractDAO<UserProfileData>
    {
        public UserProfileDAO(IStorage storage) : base(storage)
        {
        }

        public override UserProfileData CreateNew(string key = null)
        {
            if (key == null) { throw new NullReferenceException("The channel id must be specified as the UserProfile key."); }
            return new UserProfileData(key);
        }

        public override string GenerateKey(UserProfileData item)
        {
            return item.ChannelId;
        }
    }
}
