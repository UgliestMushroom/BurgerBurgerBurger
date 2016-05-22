using System;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// An Arrow on the Board that redirects all MovableObjects that move over it to the direction it is pointing.
    /// </summary>
    public class Arrow : BoardObject
    {
        /// <summary>
        /// Direction the Arrow is pointing.
        /// </summary>
        public Direction PointDirection { get; set; }

        public int Index { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cellCol">Cell column on the Board this Arrow is placed</param>
        /// <param name="cellRow">Cell row on the Board this Arrow is placed</param>
        /// <param name="pointDirection">Direction this Arrow is pointing</param>
        public Arrow(int cellCol, int cellRow, Direction pointDirection) : base(cellCol, cellRow)
        {
            this.PointDirection = pointDirection;
        }

        /// <summary>
        /// Interact with a moving object that enters this Base's cell.
        /// </summary>
        /// <param name="movingObject">Object that enters this Base's cell.</param>
        public override void InteractWithMovingObject(MovableObject movingObject)
        {
            movingObject.MovingDirection = this.PointDirection;
        }
    }
}
