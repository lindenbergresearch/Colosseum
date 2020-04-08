namespace Renoir {
    /// <summary>
    ///     Extension methods for numeric stuff
    /// </summary>
    public static class RMath {
        /// <summary>
        ///     Test for number exceeding a special value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool exceeds(this float x, float y) {
            return x > y;
        }
    }
}