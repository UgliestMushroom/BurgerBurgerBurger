using System;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// A Base on the board is owned by a Player.
    /// When an object interacts with the Base, the owner Player will get the score of the MovableObject.
    /// </summary>
    public class Base : BoardObject
    {
        public Player Player { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cellCol">Cell column on the board this Base is placed</param>
        /// <param name="cellRow">Cell row on the board this Base is placed</param>
        public Base(int cellCol, int cellRow, Player player) : base(cellCol, cellRow)
        {
            this.Player = player;
        }

        /// <summary>
        /// Interact with a moving object that enters this Base's cell.
        /// </summary>
        /// <param name="movingObject">Object that enters this Base's cell.</param>
        public override void InteractWithMovingObject(MovableObject movingObject)
        {
            this.Player.UpdateScore(movingObject.PointValue);
        }
    }
}
