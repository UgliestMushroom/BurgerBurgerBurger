using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    public class MovableObject
    {
        /// <summary>
        /// Delegate type for handling events from the MovableObject.
        /// </summary>
        /// <param name="movingObject">Object that acted</param>
        /// <param name="moveArgs">Event args</param>
        public delegate void MoveHandler(MovableObject movingObject, EventArgs moveArgs);

        /// <summary>
        /// Event for when the MovableObject moves.
        /// </summary>
        public event MoveHandler MoveEvent;

        private int X { get; set; }
        private int Y { get; set; }

        internal int CurrentCellCol { get; set; }
        internal int CurrentCellRow { get; set; }

        public const float DEFAULT_SPEED = 1f;
        public float Speed { set; get; }

        public const int DEFAULT_POINT_VALUE = 1;
        public int PointValue { set; get; }

        /// <summary>
        /// Direction that the MovableObject is currently moving in.
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
        /// Direction the MovableObject will turn when it hits a wall.
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

        public void Move()
        {
            // Calculate proposed X / Y based on Speed and Direction
            int[] proposedMovePoint = CalculateProposedPositionAfterMove();
            
            if (Board.Instance.IsPositionOutOfBounds(proposedMovePoint[0], proposedMovePoint[1]))
            {
                this.Turn();
                return;
            }

            // Calculate proposed cell from proposed X / Y
            int[] proposedMoveCell = new int[] { this.CurrentCellCol, this.CurrentCellRow };
            try
            {
                proposedMoveCell = Board.Instance.CalculateCellFromPosition(proposedMovePoint[0], proposedMovePoint[1]);
            } catch (WouldHitWallException e)
            {
                this.Turn();
                return;
            }
            
            // Actually move and notify
            this.X = proposedMovePoint[0];
            this.Y = proposedMovePoint[1];
            this.CurrentCellCol = proposedMoveCell[0];
            this.CurrentCellRow = proposedMoveCell[1];

            // Notify anyone that I've moved.
            if (this.MoveEvent != null)
            {
                MoveEvent(this, null);
            }
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

        private int[] CalculateProposedPositionAfterMove()
        {
            int proposedX = this.X;
            int proposedY = this.Y;
            
            int movementValue = (int)Math.Ceiling(GameSettings.PxPerMove * this.Speed);
            if (this.MovingDirection == Direction.Left || this.MovingDirection == Direction.Up)
            {
                movementValue = -1 * movementValue;
            }

            if (this.MovingDirection == Direction.Left || this.MovingDirection == Direction.Right)
            {
                proposedX += movementValue;
            }
            else
            {
                proposedY += movementValue;
            }

            return new int[] { proposedX, proposedY };
        }



        public void Kill()
        {
            throw new NotImplementedException();
        }
    }
}
