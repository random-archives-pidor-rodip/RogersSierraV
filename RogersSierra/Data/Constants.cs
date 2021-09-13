using FusionLibrary;

namespace RogersSierra.Data
{
    public class Constants
    {
        public const string TrainDecorator = "IsTrain";
        public const string TrainDirection = "Direction";
        
        public const string TrainCarriagesNumber = "TrainCarriagesNumber";
        public const string TrainVisibleCarriageHandle = "TrainCarriageEntityHandle";
        public const string TrainHeadHandle = "TrainHeadEntityHandle";
        public const string TrainIsCustom = "IsCustomTrain";
        public const string TrainGuid = "TrainGuid";

        public const string InteractableEntity = "InteractableEntity";
        public const string InteractableId = "InteractableId";
        public const string InteractableCurrentAngle = "InteractableCurrentAngle";

        public static void RegisterDecorators()
        {
            Decorator.Register(TrainDecorator, FusionEnums.DecorType.Bool);
            Decorator.Register(TrainDirection, FusionEnums.DecorType.Bool);
            Decorator.Register(TrainCarriagesNumber, FusionEnums.DecorType.Int);
            Decorator.Register(TrainVisibleCarriageHandle, FusionEnums.DecorType.Int);
            Decorator.Register(TrainHeadHandle, FusionEnums.DecorType.Int);
            Decorator.Register(TrainIsCustom, FusionEnums.DecorType.Bool);
            Decorator.Register(TrainGuid, FusionEnums.DecorType.Int);

            Decorator.Register(InteractableEntity, FusionEnums.DecorType.Bool);
            Decorator.Register(InteractableId, FusionEnums.DecorType.Int);
            Decorator.Register(InteractableCurrentAngle, FusionEnums.DecorType.Float);

            Decorator.Lock();
        }
    }
}
