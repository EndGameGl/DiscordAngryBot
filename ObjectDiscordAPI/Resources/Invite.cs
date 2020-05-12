using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.GuildResources;

namespace ObjectDiscordAPI.Resources
{
    public class Invite
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("guild")]
        public Guild Guild { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("inviter")]
        public User Inviter { get; set; }

        [JsonProperty("target_user")]
        public User TargetUser { get; set; }

        [JsonProperty("target_user_type")]
        public int? TargetUserType { get; set; }

        [JsonProperty("approximate_presence_count")]
        public int? ApproximatePresenceCount { get; set; }

        [JsonProperty("approximate_member_count")]
        public int? ApproximateMemberCount { get; set; }

        [JsonConstructor]
        public Invite() { }
    }
}
