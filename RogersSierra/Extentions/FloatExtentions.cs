namespace RogersSierra.Extentions
{
    public static class FloatExtentions
    {
        /// <summary>
        /// Remaps value from one range to another.
        /// </summary>
        /// <param name="value">Value to remap.</param>
        /// <param name="from1">Original range min.</param>
        /// <param name="to1">To range min.</param>
        /// <param name="from2">Original range max.</param>
        /// <param name="to2">To range max.</param>
        /// <returns>Remaped value.</returns>
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}
