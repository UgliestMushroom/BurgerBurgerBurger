using System;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// Direction an object can face, travel, or direct on the Board.
    /// </summary>
    public enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        Random = 4
    }

    /// <summary>
    /// Utility class for working with Directions
    /// </summary>
    public static class DirectionUtils
    {
        /// <summary>
        /// Get a random Direction value that isn't Direction.Random.
        /// </summary>
        /// <returns>Direction value</returns>
        public static Direction GetRandomDirection()
        {
            // Will give values 0-3 inclusive; we don't want 4 (random)
            return DirectionUtils.GetRandomDirection(0, 4);
        }

        /// <summary>
        /// Get a random Horizontal Direction value (Left or Right).
        /// </summary>
        /// <returns>Direction value</returns>
        public static Direction GetRandomHorizontalDirection()
        {
            // Will give values 2-3 inclusive; we don't want 0, 1, 4.
            return DirectionUtils.GetRandomDirection(2, 4);
        }

        /// <summary>
        /// Get a random Vertical Direction value (Up or Down).
        /// </summary>
        /// <returns>Direction value</returns>
        public static Direction GetRandomVerticalDirection()
        {
            // Will give values 0-1 inclusive; we don't want 2, 3, 4.
            return DirectionUtils.GetRandomDirection(0, 2);
        }

        /// <summary>
        /// Get a random Direction value.  Throws if the parameters can provide an invalid Direction.
        /// </summary>
        /// <param name="startInclusive">Direction value to start (inclusive)</param>
        /// <param name="endExclusive">Direction value to end (exclusive)</param>
        /// <returns>Direction value</returns>
        private static Direction GetRandomDirection(int startInclusive, int endExclusive)
        {
            if (startInclusive < 0 || endExclusive > 5)
            {
                throw new ArgumentException("GetRandomDirection must be in the range (0,5]");
            }

            int randomDirectionInt = GameSettings.RANDOM.Next(startInclusive, endExclusive);
            return (Direction)randomDirectionInt;
        }
    }
}
