
namespace BCILib.App
{
    public static class GameCommand
    {
        public const ushort Initialize = 0;
        public const ushort StartGame = 1;
        public const ushort StopGame = 3;
        public const ushort CloseGame = 4;

        // Player 1
        public const ushort SetBCIScore = 2;
        public const ushort SetPlayerName = 5;

        // Player 2
        public const ushort SetPlayerName2 = 6;
        public const ushort SetBCIScore2 = 7;

        public const int Pause = 8;
        public const int Resume = 9;
        public const ushort P300CMD = 10;

        // 20120730
        public const ushort StartTrial = 11;
        public const ushort MIStep = 12;

        /// <summary>
        /// maximun short number
        /// </summary>
        public const ushort SendCmdString = ushort.MaxValue;

        // AMPContainer: Recording
        public const ushort StartRecord = 20;
        public const ushort StopRecord = 21;
        public const ushort SendStimCode = 22;

        public const int Timer_Start = 30;
        public const int Timer_Pause = 31;
        public const int Timer_Resume = 32;
        public const int Timer_Stop = 33;
        public const int Timer_Elased = 34;

        /// <summary>
        /// MI-Cue
        /// Format MI_Cue RehabCommand RehabAction MIAction
        /// </summary>
        public const ushort MI_Cue = 31;

        /// <summary>
        /// Haptic Knob Action
        /// Format: HapticKnob RehabCommand [RehabAction]
        /// </summary>
        public const ushort HapticKnob = 32;

        public const int CMD_SENDMESSAGE = 61; // event
        public const int CMD_SENDGAMEDAT = 62; // statistics
        public const int CMD_LoadGamePars = 63; // client to server
        public const int CMD_SetGameLevel = 64; // Set game level, for both server and client
    }

    public enum ExternalTools
    {
        None,
        WIN_HANDCUE01,
        WIN_HAPTIC_KNOB,
        WIN_SWALLOW_CUE
    }
}
