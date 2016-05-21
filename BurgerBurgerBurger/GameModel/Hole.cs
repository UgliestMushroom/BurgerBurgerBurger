using System;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// A Hole on the board will remove all objects that move into it.
    /// </summary>
    public class Hole : BoardObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cellCol">Cell column on the board this Hole is placed</param>
        /// <param name="cellRow">Cell row on the board this Hole is placed</param>
        public Hole(int cellCol, int cellRow) : base(cellCol, cellRow)
        {
        }

        /// <summary>
        /// Interact with a moving object that enters this Hole's cell.
        /// </summary>
        /// <param name="movingObject">Object that enters this Hole's cell.</param>
        public override void InteractWithMovingObject(MovableObject movingObject)
        {
            movingObject.Kill();
        }
    }
}
