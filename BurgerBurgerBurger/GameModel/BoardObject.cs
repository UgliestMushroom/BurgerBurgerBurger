using System;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// Static object that is placed in a cell on the Board.  Interacts with ScoreObjects moving across the Board.
    /// </summary>
    public abstract class BoardObject
    {
        /// <summary>
        /// Cell column on the Board this object is placed
        /// </summary>
        public int CellCol { get; set; }

        /// <summary>
        /// Cell row on the Board this object is placed
        /// </summary>
        public int CellRow { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cellCol">Cell column on the Board this object is placed</param>
        /// <param name="cellRow">Cell row on the Board this object is placed</param>
        public BoardObject(int cellCol, int cellRow)
        {
            this.CellCol = cellCol;
            this.CellRow = cellRow;
        }

        /// <summary>
        /// Interact with a moving object that enters this object's Board cell
        /// </summary>
        /// <param name="movingObject"></param>
        public abstract void InteractWithMovingObject(MovableObject movingObject);
    }
}
