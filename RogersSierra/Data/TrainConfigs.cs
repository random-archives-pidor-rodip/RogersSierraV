using AdvancedTrainSystem.Train;
using System.Collections.Generic;

namespace RogersSierra.Data
{
    /// <summary>
    /// Various ATS train configs.
    /// </summary>
    public class TrainConfigs
    {
        private static readonly List<TrainModel> SierraTrainModels = new List<TrainModel>()
            {
                new TrainModel(Models.InvisibleSierra, Models.VisibleSierra),
                new TrainModel(Models.InvisibleTender, Models.VisibleTender)
            };

        /// <summary>
        /// Sierra base train config.
        /// </summary>
        public static readonly TrainConfig SierraTrainConfig = new TrainConfig(26, SierraTrainModels, "Rogers Sierra No.3");
    }
}
