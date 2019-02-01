using BinaryFog.NameParser;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using VictimBot.Lib.State;

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
                return target.Results.Count > 0;
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
