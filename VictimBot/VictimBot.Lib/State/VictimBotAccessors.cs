using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace VictimBot.Lib.State
{
    public class VictimBotAccessors
    {
        public static string CurrentIncidentState_Key { get; } = $"{nameof(VictimBotAccessors)}.CurrentIncidentState";
        public static string UserProfileState_Key { get; } = $"{nameof(VictimBotAccessors)}.UserProfileState";
        public static string DialogState_Key { get; } = $"{nameof(VictimBotAccessors)}.DialogState";

        public VictimBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        public ConversationState ConversationState { get; }

        public UserState UserState { get; }

        public IStatePropertyAccessor<DialogState> DialogState_Accessor { get; set; }
        public IStatePropertyAccessor<UserProfileData> UserProfile_Accessor { get; set; }
        public IStatePropertyAccessor<IncidentData> CurrentIncident_Accessor { get; set; }
    }
}
