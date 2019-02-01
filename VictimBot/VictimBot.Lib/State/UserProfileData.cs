using BinaryFog.NameParser;
using System;
using System.Collections.Generic;
using System.Text;
using VictimBot.Lib.Helpers;

namespace VictimBot.Lib.State
{
    public class UserProfileData
    {
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
                return FullName != null && FullName.Results.Count > 0 && Email != null && Email.IsEmailAddress();
            }
        }
    }
}
