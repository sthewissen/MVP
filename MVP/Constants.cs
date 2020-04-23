namespace MVP
{
    public class Constants
    {
        public const string AuthType = "Bearer";
        public static readonly string[] AuthScopes = { "wl.signin", "wl.emails" };
        public const string AuthClientId = "c18f496a-94e7-4307-87f4-2a255314bb4c";
        public const string AuthSignatureHash = "AvlqWgVzwpWojk8fpGXIZrw3oIQ%3D";
        public const string OcpApimSubscriptionKey = "cd3b2e5d2edc43718ae46a4dfd627323";
    }

    public class Alerts
    {
        public const string OK = "OK";
        public const string Cancel = "Cancel";
        public const string HoldOn = "Hold on...";
        public const string Error = "That's not good";
    }

    public class Messaging
    {
        public const string AuthorizationComplete = "AuthorizationComplete";
    }
}
