using FusionLibrary;

namespace RogersSierra
{
    public class Models : CustomModelHandler
    {
        public static CustomModel InvisibleSierra = new CustomModel("sierra_debug");
        public static CustomModel VisibleSierra = new CustomModel("sierra");

        public static CustomModel FrontWheel = new CustomModel("sierra_fwheel");
        public static CustomModel DrivingWheel = new CustomModel("sierra_dwheel");

        public static CustomModel CouplingRod = new CustomModel("sierra_coupling_rod");
        public static CustomModel ConnectingRod = new CustomModel("sierra_connecting_rod");
        public static CustomModel Piston = new CustomModel("sierra_piston");

        public static CustomModel CombinationLever = new CustomModel("sierra_combination_lever");
        public static CustomModel RadiusRod = new CustomModel("sierra_radius_rod");
        public static CustomModel ValveRod = new CustomModel("sierra_valve_rod");

        public static void RequestAll()
        {
            var allModels = GetAllModels(typeof(Models));
            for (int i = 0; i < allModels.Count; i++)
            {
                PreloadModel(allModels[i]);
            }
        }
    }
}
