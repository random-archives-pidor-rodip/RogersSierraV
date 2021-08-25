using FusionLibrary;

namespace RogersSierra.Sierra
{
    public class Constants
    {
        public const string TrainDecorator = "IsSierra";
        public const string TrainDirection = "Direction";

        public const string InteractableEntity = "InteractableEntity";
        public const string InteractableId = "InteractableId";
        //public const string InteractableAxis = "InteractableAxis";
        //public const string InteractableControl = "InteractableControl";
        //public const string InteractableInvert = "InteractableInvert";
        //public const string InteractableMinAngle = "InteractableMinAngle";
        //public const string InteractableMaxAngle = "InteractableMaxAngle";
        public const string InteractableCurrentAngle = "InteractableCurrentAngle";
        //public const string InteractableDefaultAngle = "InteractableCurrentAngle";

        public static void RegisterDecorators()
        {
            Decorator.Register(TrainDecorator, FusionEnums.DecorType.Bool);
            Decorator.Register(TrainDirection, FusionEnums.DecorType.Bool);

            Decorator.Register(InteractableEntity, FusionEnums.DecorType.Bool);
            Decorator.Register(InteractableId, FusionEnums.DecorType.Int);
            //Decorator.Register(InteractableAxis, FusionEnums.DecorType.Int);
            //Decorator.Register(InteractableControl, FusionEnums.DecorType.Int);
            //Decorator.Register(InteractableInvert, FusionEnums.DecorType.Bool);
            //Decorator.Register(InteractableMinAngle, FusionEnums.DecorType.Int);
            //Decorator.Register(InteractableMaxAngle, FusionEnums.DecorType.Int);
            Decorator.Register(InteractableCurrentAngle, FusionEnums.DecorType.Float);
            //Decorator.Register(InteractableDefaultAngle, FusionEnums.DecorType.Float);

            Decorator.Lock();
        }
    }
}
