using BinaryFog.NameParser;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Text;
using VictimBot.Lib.Helpers;

namespace VictimBot.Lib.Storage.DTO
{
    public class UserProfileData : IStoreItem
    {
        public UserProfileData(string channelId)
        {
            ChannelId = channelId;
        }

        public Guid Guid { get; set; } = Guid.NewGuid();

        public string ChannelId { get; set; }

        private string rawname;

        public string RawName {
            get
            {
                return rawname;
            }
            set
            {
                rawname = value;
                FullName = new FullNameParser(rawname);
                FullName.Parse();
            }
        }

        public FullNameParser FullName { get; private set; }

        public string Email { get; set; }

        public bool Complete
        {
            get
            {
                return 
                    !string.IsNullOrWhiteSpace(ChannelId) && 
                    FullName != null && 
                    FullName.Results.Count > 0 &&
                    Email != null && Email.IsEmailAddress();
            }
        }

        public string ETag { get; set; } = "*";
    }
}
