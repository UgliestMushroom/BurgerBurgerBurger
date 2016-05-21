using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// Currently working with 0,0 as TOP LEFT
    /// </summary>
    public class Board
    {
        private int GameBoardStartX { get; set; }
        private int GameBoardStartY { get; set; }
        private int GameBoardEndX { get; set; }
        private int GameBoardEndY { get; set; }
        private int GameBoardWidth { get; set; }
        private int GameBoardHeight { get; set; }
        private int NumBoardCols { get; set; }
        private int NumBoardRows { get; set; }
        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }

        public bool GameStarted = false;

        //private List<Spawner> Spawners;
        //private List<Thread> SpawnerThreads;
        //private Dictionary<Player, Queue<Arrow>> PlayerArrows;

        private List<BoardObject> BoardObjects;

        public Board(int gameBoardStartX, int gameBoardStartY, int gameBoardWidth, int gameBoardHeight, int numBoardCols, int numBoardRows)
        {
            this.GameBoardStartX = gameBoardStartX;
            this.GameBoardStartY = gameBoardStartY;
            this.GameBoardWidth = gameBoardWidth;
            this.GameBoardHeight = gameBoardHeight;
            this.NumBoardCols = numBoardCols;
            this.NumBoardRows = numBoardRows;
            this.CellWidth = gameBoardWidth / numBoardCols;
            this.CellHeight = gameBoardHeight / numBoardRows;
            this.GameBoardEndX = this.GameBoardStartX + this.GameBoardWidth;
            this.GameBoardEndY = this.GameBoardStartY + this.GameBoardHeight;

            this.BoardObjects = new List<BoardObject>();
        }

        public void HandleScoreObjectOnBoard(ScoreObject movingObject, EventArgs moveArgs)
        {
            foreach(BoardObject boardObject in this.BoardObjects)
            {
                if (boardObject.CellCol == movingObject.CurrentCellCol &&
                    boardObject.CellRow == movingObject.CurrentCellRow)
                {
                    boardObject.InteractWithMovingObject(movingObject);
                    break;
                }
            }
        }

        public bool IsPositionOutOfBounds(int x, int y)
        {
            return (x < this.GameBoardStartX || y < this.GameBoardStartY || x > this.GameBoardEndX || y > this.GameBoardEndY);
        }

        /// <summary>
        /// Convert absolute coordinates (like a click) to cells on a the game board.
        /// </summary>
        /// <param name="absoluteXCoord">Absolute X coordinate</param>
        /// <param name="absoluteYCoord">Absolute Y coordinate</param>
        /// <returns>An int array with [cell column, cell row]</returns>
        public int[] CalculateCellFromPosition(int absoluteXCoord, int absoluteYCoord)
        {
            return new int[] { ConvertAbsoluteXToCellCol(absoluteXCoord), ConvertAbsoluteYToCellRow(absoluteYCoord) };
        }

        /// <summary>
        /// Convert an absolute X coordinate value into the corresponding cell column on the board.
        /// </summary>
        /// <param name="absoluteXCoord">Absolute X coordinate</param>
        /// <returns>Cell column</returns>
        private int ConvertAbsoluteXToCellCol(int absoluteXCoord)
        {
            if (absoluteXCoord <= this.GameBoardStartX)
            {
                return 0;
            }
            else if (absoluteXCoord >= this.GameBoardEndX)
            {
                return this.NumBoardCols - 1;
            }
            else
            {
                // First, if the board isn't 100% of the game window, translate the click into board coordinates
                int boardXCoordinate = absoluteXCoord - this.GameBoardStartX;
                // Second, math the board coordinates into a cell, using int truncation
                int cellCol = boardXCoordinate / this.CellWidth;
                return cellCol;
            }
        }

        /// <summary>
        /// Convert an absolute Y coordinate value into the corresponding cell row on the board.
        /// </summary>
        /// <param name="absoluteYCoord">Absolute Y coordinate</param>
        /// <returns>Cell row</returns>
        private int ConvertAbsoluteYToCellRow(int absoluteYCoord)
        {
            if (absoluteYCoord <= this.GameBoardStartY)
            {
                return 0;
            }
            else if (absoluteYCoord >= this.GameBoardEndY)
            {
                return this.NumBoardRows - 1;
            }
            else
            {
                // First, if the board isn't 100% of the game window, translate the click into board coordinates
                int boardYCoordinate = absoluteYCoord - this.GameBoardStartY;
                // Second, math the board coordinates into a cell, using int truncation
                int cellRow = boardYCoordinate / this.CellHeight;
                return cellRow;
            }
        }
    }
}
