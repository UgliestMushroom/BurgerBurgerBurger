using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// A Spawner on the Board that creates and manages MovableObjects.
    /// </summary>
    public class Spawner : BoardObject
    {
        /// <summary>
        /// Event for when the Spawner creates a MovableObject on the Board.
        /// </summary>
        internal event MovableObject.MovableObjectHandler ObjectSpawnedHandler;

        /// <summary>
        /// Direction the MovableObjects that this Spawner creates will first move.
        /// </summary>
        private Direction SpawnDirection { get; set; }

        /// <summary>
        /// List of all objects this Spawner has spawned and that are still alive.
        /// </summary>
        private ConcurrentBag<MovableObject> SpawnedObjects { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cellCol">Cell column on the Board this Spawner is placed</param>
        /// <param name="cellRow">Cell row on the Board this Spawner is placed</param>
        /// <param name="spawnDirection">Direction the MovableObjects that this Spawner creates will first move</param>
        public Spawner(int cellCol, int cellRow, Direction spawnDirection) : base(cellCol, cellRow)
        {
            this.SpawnDirection = spawnDirection;
        }

        /// <summary>
        /// Interact with a moving object that enters this Spawner's cell.
        /// </summary>
        /// <param name="movingObject">Object that enters this Spawner's cell.</param>
        public override void InteractWithMovingObject(MovableObject movingObject)
        {
            movingObject.Turn();
        }

        /// <summary>
        /// Start the Spawner.
        /// </summary>
        /// <param name="dispatch">Dispatch to kick off background tasks (for spawning / movement)</param>
        public async void Start(CoreDispatcher dispatch)
        {
            dispatch.RunAsync(CoreDispatcherPriority.Normal, () => this.StartSpawner());
            dispatch.RunAsync(CoreDispatcherPriority.Normal, () => this.MoveSpawnedObjects());
        }

        /// <summary>
        /// Start spawning objects.
        /// </summary>
        public async void StartSpawner()
        {
            while (Board.Instance.GameRunning)
            {
                this.Spawn();
                await System.Threading.Tasks.Task.Delay(GameSettings.SpawnDelayMs);
            }
        }

        /// <summary>
        /// Move spawned objects.
        /// </summary>
        public async void MoveSpawnedObjects()
        {
            while(Board.Instance.GameRunning)
            {
                foreach(MovableObject movableObject in this.SpawnedObjects)
                {
                    movableObject.Move();
                }
                await System.Threading.Tasks.Task.Delay(GameSettings.MoveDelayMs);
            }
        }
        
        /// <summary>
        /// Spawn a new MovableObject on the board.
        /// </summary>
        private void Spawn()
        {
            // Determine movement direction of the new MovableObject
            Direction objectMovementDirection = this.SpawnDirection;
            if (objectMovementDirection == Direction.Random)
            {
                objectMovementDirection = DirectionUtils.GetRandomDirection();
            }

            // Determine the turning direction of the new MovableObject
            Direction objectTurnDirection = GameSettings.TurnDirection;
            if (objectTurnDirection == Direction.Random)
            {
                objectTurnDirection = DirectionUtils.GetRandomHorizontalDirection();
            }

            // Calculate the starting cell based on the movement direction
            int objectStartCol = this.CellCol;
            int objectStartRow = this.CellRow;
            switch (objectMovementDirection)
            {
                case Direction.Up:
                    objectStartRow--;
                    break;
                case Direction.Down:
                    objectStartRow++;
                    break;
                case Direction.Right:
                    objectStartCol++;
                    break;
                case Direction.Left:
                    objectStartCol--;
                    break;
                default:
                    throw new Exception(String.Format("Spawner is using an unknown or invalid Direction value: {0}", objectMovementDirection));
            }

            // Determine if this is a positive or negative object
            int objectScore = GameSettings.ObjectScore;
            if (GameSettings.RANDOM.NextDouble() <= GameSettings.ProbabilityOfNegativeSpawn)
            {
                objectScore = GameSettings.NegativeObjectScore;
            }

            MovableObject newObject = new MovableObject(objectStartCol, objectStartRow, objectMovementDirection, objectTurnDirection, objectScore, GameSettings.CellsPerMove);

            if (this.ObjectSpawnedHandler != null)
            {
                this.ObjectSpawnedHandler(newObject, null);
            }

            this.SpawnedObjects.Add(newObject);
        }
    }
}
