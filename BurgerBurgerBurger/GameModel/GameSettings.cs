using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// GameSettings contains all rule and scoring values used in the game.
    /// </summary>
    public static class GameSettings
    {
        /// <summary>
        /// Random number generator used for all randomness.
        /// </summary>
        public static readonly Random RANDOM = new Random();

        #region Rules

        /// <summary>
        /// Values for the max number of Arrows a Player can place.
        /// </summary>
        public const int DEFAULT_MAX_ARROWS_PER_PLAYER = 3;
        private static int maxArrowsPerPlayer = DEFAULT_MAX_ARROWS_PER_PLAYER;
        public static int MaxArrowsPerPlayer
        {
            get
            {
                return GameSettings.maxArrowsPerPlayer;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Max arrows per player must be > 0.");
                }
                GameSettings.maxArrowsPerPlayer = value;
            }
        }

        #endregion

        #region Scoring

        /// <summary>
        /// Values for the normal MovableObject score.
        /// </summary>
        public const int DEFAULT_OBJECT_SCORE = 1;
        private static int objectScore = DEFAULT_OBJECT_SCORE;
        public static int ObjectScore
        {
            get
            {
                return GameSettings.objectScore;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Object score cannot be negative.");
                }
                GameSettings.objectScore = value;
            }
        }

        /// <summary>
        /// Values for the negative MovableObject score.
        /// </summary>
        public const int DEFAULT_NEGATIVE_OBJECT_SCORE = -10;
        private static int negativeObjectScore = DEFAULT_NEGATIVE_OBJECT_SCORE;
        public static int NegativeObjectScore
        {
            get
            {
                return GameSettings.negativeObjectScore;
            }
            set
            {
                if (value > 0)
                {
                    throw new ArgumentException("Negative object score cannot be positive.");
                }
                GameSettings.negativeObjectScore = value;
            }
        }

        #endregion

        #region Movement

        /// <summary>
        /// Values for the movement distance of MovableObjects.
        /// </summary>
        public const double DEFAULT_CELLS_PER_MOVE = 0.5;
        private static double cellsPerMove = DEFAULT_CELLS_PER_MOVE;
        public static double CellsPerMove
        {
            get
            {
                return GameSettings.cellsPerMove;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Cells per move must be > 0.");
                }
                GameSettings.cellsPerMove = value;
            }
        }
        
        public const Direction DEFAULT_TURN_DIRECTION = Direction.Right;
        private static Direction turnDirection;
        public static Direction TurnDirection
        {
            get
            {
                return GameSettings.turnDirection;
            }
            set
            {
                if (value == Direction.Up || value == Direction.Down )
                {
                    throw new NotSupportedException("GameSettings.TurnDirection cannot be Up or Down.");
                }
                GameSettings.turnDirection = value;
            }
        }

        #endregion

        #region Spawner

        /// <summary>
        /// Values for the probability that a spawner creates a negative MovableObject.
        /// </summary>
        public const double DEFAULT_PROBABILITY_OF_NEGATIVE_SPAWN = 0.1;
        private static double probabilityOfNegativeSpawn = DEFAULT_PROBABILITY_OF_NEGATIVE_SPAWN;
        public static double ProbabilityOfNegativeSpawn
        {
            get
            {
                return GameSettings.probabilityOfNegativeSpawn;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException("Probability of negative spawn must be between 0 and 1.");
                }
                GameSettings.probabilityOfNegativeSpawn = value;
            }
        }

        /// <summary>
        /// Values for the direction that the Spawner spawns MovableObjects
        /// </summary>
        public const Direction DEFAULT_SPAWNER_DIRECTION = Direction.Right;
        private static Direction spawnerDirection = DEFAULT_SPAWNER_DIRECTION;
        public static Direction SpawnerDirection
        {
            get
            {
                return GameSettings.spawnerDirection;
            }
            set
            {
                GameSettings.spawnerDirection = value;
            }
        }

        #endregion

        #region Timing

        /// <summary>
        /// Values for the Spawner spawn delay
        /// </summary>
        public const int DEFAULT_SPAWN_DELAY_MS = 2000;
        private static int spawnDelayMs = DEFAULT_SPAWN_DELAY_MS;
        public static int SpawnDelayMs
        {
            get
            {
                return GameSettings.spawnDelayMs;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("SpawnDelayMs cannot be negative.");
                }
                GameSettings.spawnDelayMs = value;
            }
        }

        /// <summary>
        /// Values for the MoveableObject move delay
        /// </summary>
        public const int DEFAULT_MOVE_DELAY_MS = 200;
        private static int moveDelayMs = DEFAULT_MOVE_DELAY_MS;
        public static int MoveDelayMs
        {
            get
            {
                return GameSettings.moveDelayMs;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("MoveDelayMs cannot be negative.");
                }
                GameSettings.moveDelayMs = value;
            }
        }

        #endregion

        /*
        public static void InitializeDefaults()
        {
            GameSettings.MaxArrowsPerPlayer = GameSettings.DEFAULT_MAX_ARROWS_PER_PLAYER;
            GameSettings.MoveDelayMs = GameSettings.DEFAULT_MOVE_DELAY_MS;
            GameSettings.CellsPerMove = GameSettings.DEFAULT_CELLS_PER_MOVE;
            GameSettings.SpanwDelayMs = GameSettings.DEFAULT_SPAWN_DELAY_MS;
            GameSettings.SpawnerDirection = GameSettings.DEFAULT_SPAWNER_DIRECTION;
            GameSettings.TurnDirection = GameSettings.DEFAULT_TURN_DIRECTION;
        }

        public static void SetGameBoardSize(int colWidth, int colHeight)
        {
            if (colWidth != colHeight)
            {
                throw new NotSupportedException("Rectangle boards are not accepted.");
            }

            GameSettings.PxPerMove = (int)(((double)colWidth) * GameSettings.CellsPerMove);
        }
        */
    }
}
