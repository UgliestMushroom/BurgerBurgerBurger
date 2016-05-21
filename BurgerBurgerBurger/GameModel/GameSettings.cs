using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    static class GameSettings
    {
        public static readonly Random RANDOM = new Random();

        public const int DEFAULT_MAX_ARROWS_PER_PLAYER = 3;
        public static int MaxArrowsPerPlayer { get; set; }
        /*
        public const int DEFAULT_MOVE_DELAY_MS = 250;
        public static int MoveDelayMs { get; set; }

        public const double DEFAULT_CELLS_PER_MOVE = 0.75;
        private static double cellsPerMove;
        public static double CellsPerMove
        {
            get
            {
                return GameSettings.cellsPerMove;
            }
            set
            {
                if (value < 0 || value > 1.0)
                {
                    throw new ArgumentException("CellsPerMove must be between 0.0 and 1.0");
                }
                GameSettings.cellsPerMove = value;
            }
        }
        */

        public static int PxPerMove { get; private set; }
        /*
        public const int DEFAULT_SPAWN_DELAY_MS = 5000;
        public static int SpanwDelayMs { get; set; }

        public const Direction DEFAULT_SPAWNER_DIRECTION = Direction.Right;
        public static Direction SpawnerDirection { get; set; }

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
                if (value == Direction.Up || value == Direction.Down)
                {
                    throw new NotSupportedException("GameSettings.TurnDirection cannot be Up or Down.");
                }

                GameSettings.turnDirection = value;
            }
        }

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
