namespace RogersSierra.Data
{
    /// <summary>
    /// Contains path's for script files.
    /// </summary>
    public class Files
    {
        /// <summary>
        /// Relative path to folder with sound files.
        /// </summary>
        public const string SoundsFolder = "RogersSierraV/Sounds/";

        // Actions
        public const string SteamBrakeStart = "Actions/steam_brake_start.wav";
        public const string SteamBrakeEnd = "Actions/steam_brake_end.wav";
        public const string SteamBrakeLoop = "Actions/steam_brake_loop.wav";

        // Movement
        public const string AmbientMoving = "Moving/ambient_moving";
        public const string PistonMove = "Moving/Chug/Chug";
        public const string WheelSlip = "Moving/wheel_slip.wav";
        public const string OnTrainStart = "Moving/start.wav";

        // Base
        public const string SteamIdle = "steam_idle.wav";
    }
}
