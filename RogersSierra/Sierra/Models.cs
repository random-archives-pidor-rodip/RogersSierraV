using FusionLibrary;

namespace RogersSierra.Sierra
{
    public class Models : CustomModelHandler
    {
        // Train
        public static CustomModel InvisibleSierra = new CustomModel("sierra_debug");
        public static CustomModel VisibleSierra = new CustomModel("sierra");

        // Wheels
        public static CustomModel FrontWheel = new CustomModel("sierra_fwheel");
        public static CustomModel DrivingWheel = new CustomModel("sierra_dwheel");

        // Drivetrain
        public static CustomModel CouplingRod = new CustomModel("sierra_coupling_rod");
        public static CustomModel ConnectingRod = new CustomModel("sierra_connecting_rod");
        public static CustomModel Piston = new CustomModel("sierra_piston");
        public static CustomModel CombinationLever = new CustomModel("sierra_combination_lever");
        public static CustomModel RadiusRod = new CustomModel("sierra_radius_rod");
        public static CustomModel ValveRod = new CustomModel("sierra_valve_rod");

        // Interior
        public static CustomModel CabThrottleLever = new CustomModel("sierra_throttle_lever");
        public static CustomModel CabThrottleHandle = new CustomModel("sierra_throttle_lever_handle");
        public static CustomModel CabGearLever = new CustomModel("sierra_gear_lever");
        public static CustomModel CabGearHandle = new CustomModel("sierra_gear_lever_handle");
        public static CustomModel CabAirBrakeLever = new CustomModel("sierra_cab_airbrake_lever");
        public static CustomModel CabSteamBrakeLever = new CustomModel("sierra_cab_brake_lever");

        // Brakes
        public static CustomModel AirbrakeMain = new CustomModel("sierra_airbrake_main");
        public static CustomModel AirbrakeRod = new CustomModel("sierra_airbrake_rod");
        public static CustomModel AirbrakeLever = new CustomModel("sierra_airbrake_lever");
        public static CustomModel Brake1 = new CustomModel("sierra_brake_1");
        public static CustomModel Brake2 = new CustomModel("sierra_brake_2");
        public static CustomModel Brake3 = new CustomModel("sierra_brake_3");

        /// <summary>
        /// Requests all models.
        /// </summary>
        public static void RequestAll()
        {
            var allModels = GetAllModels(typeof(Models));

            for (int i = 0; i < allModels.Count; i++)
                allModels[i].Request();
        }
    }
}
