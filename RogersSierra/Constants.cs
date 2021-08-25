using FusionLibrary;

namespace RogersSierra
{
    public class Constants
    {
        public const string TrainDecorator = "IsSierra";
        public const string TrainDirection = "Direction";
        public const string InteractableEntity = "InteractableEntity";
        public const string InteractableId = "InteractableId";

        public static void RegisterDecorators()
        {
            Decorator.Register(TrainDecorator, FusionEnums.DecorType.Bool);
            Decorator.Register(TrainDirection, FusionEnums.DecorType.Bool);
            Decorator.Register(InteractableEntity, FusionEnums.DecorType.Bool);
            Decorator.Register(InteractableId, FusionEnums.DecorType.Int);
            Decorator.Lock();
        }
    }
}
