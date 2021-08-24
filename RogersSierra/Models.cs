using FusionLibrary;

namespace RogersSierra
{
    public class Models : CustomModelHandler
    {
        public static CustomModel InvisibleSierra = new CustomModel("sierra_debug");
        public static CustomModel VisibleSierra = new CustomModel("sierra");

        public static CustomModel FrontWheel = new CustomModel("sierra_fwheel");
        public static CustomModel DrivingWheel = new CustomModel("sierra_mwheel");

        public static CustomModel CouplingRod = new CustomModel("sierra_rods_main");
        public static CustomModel ConnectingRod = new CustomModel("sierra_rods_large_pistons");
        public static CustomModel Piston = new CustomModel("sierra_large_piston");

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
