using FusionLibrary;
using GTA.Math;

namespace RogersSierra
{
    public class Utils
    {
        public static float GetRadiusOfModel(CustomModel model)
        {
            // Get the dimensions
            (Vector3 min, Vector3 max) = model.Model.Dimensions;

            // Return the output
            return (max - min).Length() / 2;
        }
    }
}
