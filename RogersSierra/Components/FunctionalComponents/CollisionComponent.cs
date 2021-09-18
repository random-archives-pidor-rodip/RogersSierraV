using RageComponent;
using FusionLibrary.Extensions;
using FusionLibrary;
using RogersSierra.Other;

namespace RogersSierra.Components.FunctionalComponents
{
    /// <summary>
    /// Handles what to do on collision.
    /// </summary>
    public class CollisionComponent : Component<RogersSierra>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Start()
        {
            // Detach every prop on collision
            Base.CustomTrain.CollisionComponent.OnDerail += () =>
            {
                var totalProps = Base.TrainComponents.AllHandlerComponentProps.Count;
                for (int i = 0; i < FusionUtils.Random.Next(5, totalProps); i++)
                {
                    var randomProp =
                        Base.TrainComponents.AllHandlerComponentProps.SelectRandomElement();
                    randomProp.ScatterProp(FusionUtils.Random.Next(1, 4));
                }

                Utils.ProcessAllValuesFieldsByType<AnimatePropsHandler>(
                    Base.WheelComponent, x => x.ScatterProp(3));
            };
        }
    }
}
