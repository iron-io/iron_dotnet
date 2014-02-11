namespace IronSharp.Core
{
    public static class ExtensionsForIMsg
    {
        public static bool HasExpectedMessage(this IMsg msg, string message)
        {
            return msg != null && msg.Message == message;
        }
    }
}