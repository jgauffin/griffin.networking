namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// Play a file
    /// </summary>
    /// TODO: Add support for playback terminators <see cref="Variable.Playback.Terminators"/>
    public class PlaybackCmd : SendMsg
    {
        private readonly string _dtmfAbort = string.Empty;
        private readonly string _fileName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">channel identifier</param>
        /// <param name="fileName">Path and filename to sound file</param>
        /// <remarks>Path should be relative to FS path.</remarks>
        /// <example>
        /// mgr.SendApi(new PlaybackCmd(evt.UniqueId, "sounds\\sv\\welcome.wav", PlaybackCmd.All));
        /// </example>
        public PlaybackCmd(UniqueId id, string fileName)
            : base(id)
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackCmd"/> class.
        /// </summary>
        /// <param name="id">The UUID.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="dtmfAbort">DTMF digits used to terminate the playback</param>
        public PlaybackCmd(UniqueId id, string fileName, string dtmfAbort) : base(id)
        {
            _fileName = fileName;
            _dtmfAbort = dtmfAbort;
        }

        public override string Command
        {
            get { return "playback"; }
        }

        public override string[] Arguments
        {
            get { return new[] {_fileName}; }
        }

        public string DtmfAbort
        {
            get { return _dtmfAbort; }
        }
    }
}