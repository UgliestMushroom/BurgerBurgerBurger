using System;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    public class MovableObject
    {
        /// <summary>
        /// Delegate type for handling events from the MovableObject.
        /// </summary>
        /// <param name="movingObject">Object that acted</param>
        /// <param name="moveArgs">Event args</param>
        public delegate void MovableObjectHandler(MovableObject movingObject, EventArgs moveArgs);

        /// <summary>
        /// Event for when the MovableObject moves.
        /// </summary>
        public event MovableObjectHandler MoveEvent;

        /// <summary>
        /// Event for when the MovableObject turns.
        /// </summary>
        public event MovableObjectHandler TurnEvent;

        /// <summary>
        /// Event for when the MovableObject dies.
        /// </summary>
        public event MovableObjectHandler KillEvent;

        #region Properties

        /// <summary>
        /// Cell column on the Board this object is is placed
        /// </summary>
        internal int CurrentCellCol { get; set; }

        /// <summary>
        /// Cell row on the Board this object is is placed
        /// </summary>
        internal int CurrentCellRow { get; set; }

        /// <summary>
        /// Absolute X coordinate on the UI where this object is placed
        /// </summary>
        private int X { get; set; }

        /// <summary>
        /// Absolute Y coordinate on the UI where this object is placed
        /// </summary>
        private int Y { get; set; }

        /// <summary>
        /// Point value this object gives to a Player if it reaches the Player's Base
        /// </summary>
        public int PointValue { set; get; }

        /// <summary>
        /// Number of cells this object moves on each call to Move
        /// </summary>
        public double CellsPerMove { set; get; }

        /// <summary>
        /// Number of pixels this object moves when moving on the X axis
        /// </summary>
        private int XPxPerMove { get; set; }

        /// <summary>
        /// Number of pixels this object moves when moving on the Y axis
        /// </summary>
        private int YPxPerMove { get; set; }

        /// <summary>
        /// Direction that the MovableObject is currently moving in
        /// </summary>
        private Direction movingDirection;
        internal Direction MovingDirection
        {
            get
            {
                return this.movingDirection;
            }
            set
            {
                if (value == Direction.Random)
                {
                    throw new NotSupportedException("MovingObject.MovingDirection cannot be Random.");
                }
                this.movingDirection = value;
            }
        }

        /// <summary>
        /// Direction the MovableObject will turn when it hits a wall
        /// </summary>
        private Direction turnDirection;
        Direction TurnDirection
        {
            get
            {
                return this.turnDirection;
            }
            set
            {
                if (value == Direction.Up || value == Direction.Down)
                {
                    throw new NotSupportedException("MovableObject.TurnDirection cannot be Up or Down.");
                }
                this.turnDirection = value;
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cellCol">Cell column on the Board this object is placed</param>
        /// <param name="cellRow">Cell row on the Board this object is placed</param>
        /// <param name="moveDirection">Direction this object will travel when moving</param>
        /// <param name="turnDirection">Direction this object will turn when turning</param>
        /// <param name="pointValue">Points this object will give when interacting with a Player's Base</param>
        /// <param name="cellsPerMove">Distance in cells this object will move on each movement</param>
        public MovableObject(int cellCol, int cellRow, Direction moveDirection, Direction turnDirection, int pointValue, double cellsPerMove)
        {
            this.CurrentCellCol = cellCol;
            this.CurrentCellRow = cellRow;
            this.MovingDirection = moveDirection;
            this.TurnDirection = turnDirection;
            this.PointValue = pointValue;
            this.CellsPerMove = cellsPerMove;

            int[] xyCoordinates = Board.Instance.ConvertCellCoordinatesToAbsolute(this.CurrentCellCol, this.CurrentCellRow);
            this.X = xyCoordinates[0];
            this.Y = xyCoordinates[1];

            this.XPxPerMove = (int) (this.CellsPerMove * Board.Instance.CellWidth);
            this.YPxPerMove = (int) (this.CellsPerMove * Board.Instance.CellHeight);
        }

        #region Movement

        /// <summary>
        /// Move the object across the Board, if possible.  If the location it would move to is out of bounds, of if 
        /// it would hit a wall, it turns.  If it does move and doesn't turn, it notifies listeners of its updated position.
        /// </summary>
        public void Move()
        {
            // Calculate proposed X / Y based on Speed and Direction
            int[] proposedMovePoint = CalculateProposedPositionAfterMove();
            
            // Check if the proposed location is out of bounds
            if (Board.Instance.IsPositionOutOfBounds(proposedMovePoint[0], proposedMovePoint[1]))
            {
                this.Turn();
                return;
            }

            // Calculate proposed cell from proposed X / Y and check walls in the way
            int[] proposedMoveCell = new int[] { this.CurrentCellCol, this.CurrentCellRow };
            try
            {
                proposedMoveCell = Board.Instance.GetCellIfMovedTo(this, proposedMovePoint[0], proposedMovePoint[1]);
            } catch (WouldHitWallException)
            {
                this.Turn();
                return;
            }
            
            // The move is valid, so update the position
            this.X = proposedMovePoint[0];
            this.Y = proposedMovePoint[1];
            this.CurrentCellCol = proposedMoveCell[0];
            this.CurrentCellRow = proposedMoveCell[1];

            // Notify anyone that I've moved
            if (this.MoveEvent != null)
            {
                MoveEvent(this, null);
            }
        }

        /// <summary>
        /// Calculate the X and Y location if this object were to move in it's current direction.
        /// </summary>
        /// <returns>X,Y coordinates where the object, as is, would move to</returns>
        private int[] CalculateProposedPositionAfterMove()
        {
            int proposedX = this.X;
            int proposedY = this.Y;

            // Moving left and up is reducing the x/y coordinates (respectively); moving right and up increases them
            int axisFactor = (this.MovingDirection == Direction.Left || this.MovingDirection == Direction.Up) ? -1 : 1;

            if (this.MovingDirection == Direction.Left || this.MovingDirection == Direction.Right)
            {
                proposedX += axisFactor * this.XPxPerMove;
            }
            else
            {
                proposedY += axisFactor * this.YPxPerMove;
            }

            return new int[] { proposedX, proposedY };
        }

        /// <summary>
        /// Turn to modify the direction this object is now moving.
        /// </summary>
        internal void Turn()
        {
            switch (this.TurnDirection)
            {
                case (Direction.Left):
                    this.TurnLeft();
                    break;
                case (Direction.Right):
                    this.TurnRight();
                    break;
                case (Direction.Random):
                    this.TurnRandom();
                    break;
                default:
                    throw new NotSupportedException("MovingObject has invalid TurnDirection!");
            }

            if (this.TurnEvent != null)
            {
                this.TurnEvent(this, null);
            }
        }

        /// <summary>
        /// Turn left to modify the direction this object is moving.
        /// </summary>
        private void TurnLeft()
        {
            switch (this.MovingDirection)
            {
                case (Direction.Up):
                    this.MovingDirection = Direction.Left;
                    break;
                case (Direction.Left):
                    this.MovingDirection = Direction.Down;
                    break;
                case (Direction.Down):
                    this.MovingDirection = Direction.Right;
                    break;
                case (Direction.Right):
                    this.MovingDirection = Direction.Up;
                    break;
                default:
                    throw new NotSupportedException("MovingObject has invalid MovingDirection!");
            }
        }

        /// <summary>
        /// Turn right to modify the direction this object is moving.
        /// </summary>
        private void TurnRight()
        {
            switch (this.MovingDirection)
            {
                case (Direction.Up):
                    this.MovingDirection = Direction.Right;
                    break;
                case (Direction.Right):
                    this.MovingDirection = Direction.Down;
                    break;
                case (Direction.Down):
                    this.MovingDirection = Direction.Left;
                    break;
                case (Direction.Left):
                    this.MovingDirection = Direction.Up;
                    break;
                default:
                    throw new NotSupportedException("MovingObject has invalid MovingDirection!");
            }
        }

        /// <summary>
        /// Turn a random direction to modify the direction this object is moving.
        /// </summary>
        private void TurnRandom()
        {
            Direction newDirection = this.MovingDirection;
            while (newDirection == this.MovingDirection)
            {
                int randomDirectionInt = GameSettings.RANDOM.Next(4); // Will give values 0-3 inclusive; we don't want 4 (random)
                newDirection = (Direction)randomDirectionInt;
            }

            this.MovingDirection = newDirection;
        }

        #endregion

    }
}
