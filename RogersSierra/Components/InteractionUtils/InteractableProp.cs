using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using RogersSierra.Sierra;

namespace RogersSierra.Components.InteractionUtils
{
    public class InteractableProp
    {
        /// <summary>
        /// Interactable prop.
        /// </summary>
        public AnimateProp Prop { get; private set; }

        /// <summary>
        /// Returns True if player is currently interacting with prop, otherwise False.
        /// </summary>
        public bool IsInteracting { get; private set; }

        /// <summary>
        /// Axis of interactable prop rotation.
        /// </summary>
        private Vector3 _axis;

        /// <summary>
        /// Game control of the interaction.
        /// </summary>
        private Control _control;

        /// <summary>
        /// Invert control or not.
        /// </summary>
        private bool _invert;

        /// <summary>
        /// Minimum angle of interactable prop rotation.
        /// </summary>
        private int _minAngle;

        /// <summary>
        /// Maximum angle of interactable prop rotation.
        /// </summary>
        private int _maxAngle;

        private float _angleTo;
        private float _currentAngle;

        private float _sensetivity = 14;

        public InteractableProp(
            AnimateProp prop,
            Vector3 axis, 
            Control control,
            bool invert,
            int minAngle, 
            int maxAngle, 
            float defaultAngle,
            float sensetivityMultiplier)
        {
            Prop = prop;

            _axis = axis;
            _control = control;
            _invert = invert;
            _minAngle = minAngle;
            _maxAngle = maxAngle;
            _angleTo = defaultAngle;

            _sensetivity *= sensetivityMultiplier;
        }

        public void OnTick()
        {
            // We still will process angle even after stopping the interaction to get better animation
            if (IsInteracting)
            {
                var controlInput = Game.GetControlValueNormalized(_control);

                if (_invert)
                    controlInput *= -1;

                _angleTo += controlInput * _sensetivity;
            }
            _angleTo = _angleTo.Clamp(_minAngle, _maxAngle);
            _currentAngle = FusionUtils.Lerp(_currentAngle, (int) _angleTo, 0.1f);

            // Convert angle to usable range
            var convertedAngle = _currentAngle.Remap(_minAngle, _maxAngle, 0, 1);
            Prop.Prop.Decorator().SetFloat(Constants.InteractableCurrentAngle, convertedAngle);

            //GTA.UI.Screen.ShowSubtitle($"MouseInput: {controlInput} AngleTo :{_angleTo} CurrentAngle: {_currentAngle}");

            Prop.SecondRotation = _axis * _currentAngle;
        }

        /// <summary>
        /// Artificially sets value of interaction prop.
        /// </summary>
        /// <param name="value">Value in range of 0.0 - 1.0</param>
        public void SetValue(float value)
        {
            value = value.Clamp(0, 1).Remap(0, 1, _minAngle, _maxAngle);
            _angleTo = value;
        }

        public void StartInteraction()
        {
            IsInteracting = true;
        }

        public void StopInteraction()
        {
            IsInteracting = false;
        }
    }
}
