using GTA;
using System.IO;

namespace RogersSierra
{
    /// <summary>
    /// Custom model extends functionality of <see cref="Model"/>.
    /// </summary>
    public class CustomModel
    {
        public Model Model {  get; }

        public CustomModel(string modelName)
        {
            Model = new Model(modelName);

            // Requrest model and wait until it loads
            Model.Request();
            while (Model.IsLoaded is false)
            {
                Script.Yield();
            }
        }
    }
}
