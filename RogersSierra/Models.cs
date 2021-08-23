using FusionLibrary;

namespace RogersSierra
{
    public class Models : CustomModelHandler
    {
        public static CustomModel InvisibleSierra = new CustomModel("sierra_debug");
        public static CustomModel VisibleSierra = new CustomModel("sierra");

        public static CustomModel FrontWheel = new CustomModel("fwheel");
        public static CustomModel MainWheel = new CustomModel("mwheel");

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
