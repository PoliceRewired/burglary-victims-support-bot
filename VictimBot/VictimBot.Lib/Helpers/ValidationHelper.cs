using BinaryFog.NameParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using VictimBot.Lib.State;
using VictimBot.Lib.Storage.DTO;

namespace VictimBot.Lib.Helpers
{
    public static class ValidationHelper
    {
        public static bool HasName(this UserProfileData profile) => profile.FullName != null && profile.FullName.Results.Count > 0;

        public static bool IsFullName(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return false; }
            try
            {
                var target = new FullNameParser(text);
                target.Parse();

                // there must be at least one parsed name
                if (target.Results.Count == 0) { return false; }

                // a full name has both first and last name
                return
                    !string.IsNullOrWhiteSpace(target.Results.First().FirstName) &&
                    !string.IsNullOrWhiteSpace(target.Results.First().LastName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsEmailAddress(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return false; }
            try
            {
                MailAddress m = new MailAddress(text);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }
}
