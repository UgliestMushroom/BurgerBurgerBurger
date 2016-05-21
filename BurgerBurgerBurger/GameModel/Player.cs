using System;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    public class Player
    {
        /// <summary>
        /// Delegate type for handling events when the Player places Arrows.
        /// </summary>
        /// <param name="player">Player who placed the arrow</param>
        /// <param name="arrow">Arrow that was placed on the board</param>
        /// <param name="eventArgs">Additional event args</param>
        public delegate void ArrowHandler(Player player, Arrow arrow, EventArgs eventArgs);

        /// <summary>
        /// Event for when the Player places an Arrow.
        /// </summary>
        public event ArrowHandler ArrowPlacementEvent;

        /// <summary>
        /// Player's current score for the game
        /// </summary>
        private int Score { get; set; }

        /// <summary>
        /// Index of the next arrow to place.
        /// </summary>
        private int arrowToPlaceIndex;

        /// <summary>
        /// Number of arrows the player has placed so far.
        /// </summary>
        private int placedArrowCount;

        /// <summary>
        /// Arrows the Player currently has placed on the board.
        /// </summary>
        private Arrow[] arrows;

        /// <summary>
        /// Initialze the Player with no score or arrows on the Board.
        /// </summary>
        public Player()
        {
            this.Reset();
        }

        /// <summary>
        /// Reset the player to defaults for a new game.
        /// </summary>
        public void Reset()
        {
            this.Score = 0;
            this.arrowToPlaceIndex = 0;
            this.placedArrowCount = 0;
            this.arrows = new Arrow[GameSettings.DEFAULT_MAX_ARROWS_PER_PLAYER];
        }
        

        public void PlaceArrow(int cellCol, int cellRow, Direction pointDirection)
        {
            if (Board.Instance.IsCellOccupied(cellCol, cellRow))
            {
                return;
            }

            // If we haven't placed all our arrows yet, add a new one
            if (this.placedArrowCount < this.arrows.Length)
            {
                Arrow newArrow = new Arrow(cellCol, cellRow, pointDirection);
                this.arrows[this.arrowToPlaceIndex] = newArrow;

                this.placedArrowCount++;

                Board.Instance.AddObjectToBoard(newArrow, true, false);
            }
            // If we already placed max arrows, update the one that's next in line
            else
            {
                Arrow arrowToMove = this.arrows[this.arrowToPlaceIndex];
                int[] previousArrowPosition = new int[] { arrowToMove.CellCol, arrowToMove.CellRow };

                arrowToMove.CellCol = cellCol;
                arrowToMove.CellRow = cellRow;
                arrowToMove.PointDirection = pointDirection;

                Board.Instance.UpdateObjectOnBoard(previousArrowPosition[0], previousArrowPosition[1], arrowToMove);
            }

            this.arrowToPlaceIndex = (this.arrowToPlaceIndex + 1) % this.arrows.Length;
        }

        /// <summary>
        /// Update the player's score.
        /// Note: the score cannot go below 0.
        /// </summary>
        /// <param name="scoreAddition">Points to add (or subtract) from the current score</param>
        public void UpdateScore(int scoreAddition)
        {
            lock(this)
            {
                this.Score += scoreAddition;

                if (this.Score < 0)
                {
                    this.Score = 0;
                }
            }
        }

        /// <summary>
        /// Remove the player's arrows from the board.
        /// </summary>
        private void ResetArrows()
        {
            for (int i = 0; i < this.arrows.Length; i++)
            {
                if (this.arrows[i] != null)
                {
                    this.arrows[i].Kill();
                }

                arrows[i] = null;
            }
        }
    }
}
