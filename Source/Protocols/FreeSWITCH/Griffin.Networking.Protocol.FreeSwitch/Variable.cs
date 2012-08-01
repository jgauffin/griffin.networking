namespace Griffin.Networking.Protocol.FreeSwitch
{
    public static class Variable
    {
        #region Nested type: Originator

        public static class Originator
        {
            #region Nested type: CallerId

            public static class CallerId
            {
                public const string Number = "origination_caller_id_number";
                public const string Name = "origination_caller_id_name";
            }

            #endregion
        }

        #endregion

        #region Nested type: Playback

        public static class Playback
        {
            public const string Terminators = "playback_terminator_used";
        }

        #endregion

        #region Nested type: Sip

        public static class Sip
        {
            public const string CallInfo = "sip_h_Call-Info";
            public const string AutoAnswer = "sip_auto_answer";
        }

        #endregion
    }
}