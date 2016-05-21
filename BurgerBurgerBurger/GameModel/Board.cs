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
        #region Board Properties

        /// <summary>
        /// X position where the Board starts on the screen (does not assume start at 0); 0,0 is top left
        /// </summary>
        private int GameBoardStartX { get; set; }

        /// <summary>
        /// Y position where Board starts on the screen (does not assume start at 0); 0,0 is top left
        /// </summary>
        private int GameBoardStartY { get; set; }

        /// <summary>
        /// X position where Board ends on the screen (does not assume end at max X); 0,0 is top left
        /// </summary>
        private int GameBoardEndX { get; set; }

        /// <summary>
        /// Y position where Board ends on the screen (does not assume end at max Y); 0,0 is top left
        /// </summary>
        private int GameBoardEndY { get; set; }
        
        /// <summary>
        /// Number of Board columns 
        /// </summary>
        private int NumBoardCols { get; set; }

        /// <summary>
        /// Number of Board rows
        /// </summary>
        private int NumBoardRows { get; set; }

        /// <summary>
        /// Width of each cell in x/y units
        /// </summary>
        public int CellWidth { get; private set; }

        /// <summary>
        /// Height of each cell in x/y units
        /// </summary>
        public int CellHeight { get; private set; }

        /// <summary>
        /// Game state
        /// </summary>
        public bool GameStarted = false;

        /// <summary>
        /// Map of BoardObjects on the Board's grid of cells
        /// </summary>
        private BoardObject[,] boardObjectMap;

        /// <summary>
        /// Object encapsulating all the wall objects on the Board
        /// </summary>
        private BoardWalls boardWalls;

        #endregion

        #region Board Setup

        /// <summary>
        /// Singleton Board instance
        /// </summary>
        private static Board instance;
        public static Board Instance
        {
            get
            {
                if (Board.instance == null)
                {
                    throw new Exception("Must call Board.ConfigureInstance first!");
                }
                return Board.instance;
            }
        } 

        /// <summary>
        /// Configure the Board's instance
        /// </summary>
        /// <param name="gameBoardStartX">X position where the Board starts on the screen</param>
        /// <param name="gameBoardStartY">Y position where the Board starts on the screen</param>
        /// <param name="gameBoardWidth">Width of the Board on the screen</param>
        /// <param name="gameBoardHeight">Height of the Board on the screen</param>
        /// <param name="numBoardCols">Number of columns for the Board</param>
        /// <param name="numBoardRows">Number of rows for the Board</param>
        public static void ConfigureInstance(int gameBoardStartX, int gameBoardStartY, int gameBoardWidth, int gameBoardHeight, int numBoardCols, int numBoardRows)
        {
            Board.instance = new Board(gameBoardStartX, gameBoardStartY, gameBoardWidth, gameBoardHeight, numBoardCols, numBoardRows);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameBoardStartX">X position where the Board starts on the screen</param>
        /// <param name="gameBoardStartY">Y position where the Board starts on the screen</param>
        /// <param name="gameBoardWidth">Width of the Board on the screen</param>
        /// <param name="gameBoardHeight">Height of the Board on the screen</param>
        /// <param name="numBoardCols">Number of columns for the Board</param>
        /// <param name="numBoardRows">Number of rows for the Board</param>
        private Board(int gameBoardStartX, int gameBoardStartY, int gameBoardWidth, int gameBoardHeight, int numBoardCols, int numBoardRows)
        {
            this.GameBoardStartX = gameBoardStartX;
            this.GameBoardStartY = gameBoardStartY;
            this.NumBoardCols = numBoardCols;
            this.NumBoardRows = numBoardRows;
            this.CellWidth = gameBoardWidth / numBoardCols;
            this.CellHeight = gameBoardHeight / numBoardRows;
            this.GameBoardEndX = this.GameBoardStartX + gameBoardWidth;
            this.GameBoardEndY = this.GameBoardStartY + gameBoardHeight;

            this.boardObjectMap = new BoardObject[this.NumBoardCols, this.NumBoardRows];

            this.boardWalls = new BoardWalls(this.NumBoardCols. this.NumBoardRows);
        }

        public void AddWallToBoard(int cell1Col, int cell1Row, int cell2Col, int cell2Row)
        {
            this.boardWalls.AddWall(cell1Col, cell1Row, cell2Col, cell2Row);
        }

        public void AddHoleToBoard(int cellCol, int cellRow)
        {
            VerifyCellUnoccupied(cellCol, cellRow);
        }

        public void AddBaseToBoard(int cellCol, int cellRow /* owner player */)
        {
            VerifyCellUnoccupied(cellCol, cellRow);

        }

        public void AddSpawnerToBoard(int cellCol, int cellRow /* spawn direction */)
        {
            VerifyCellUnoccupied(cellCol, cellRow);

        }

        /// <summary>
        /// Verify that the cell at a given location does not already have a BoardObject present.
        /// If another object is present, this throws <see cref="ArgumentException"/>
        /// </summary>
        /// <param name="cellCol">Board cell column to check</param>
        /// <param name="cellRow">Board cell row to check</param>
        private void VerifyCellUnoccupied(int cellCol, int cellRow)
        {
            if (this.boardObjectMap[cellCol, cellRow] != null)
            {
                throw new ArgumentException(String.Format("Cannot place item at [{0},{1}]; another item is already there.", cellCol, cellRow));
            }
        }

        #endregion

        #region Board Interaction

        /// <summary>
        /// Event that handles when a MovableObject moves across the Board.
        /// </summary>
        /// <param name="movingObject">Object moving across the Board.</param>
        /// <param name="moveArgs">Event arguments</param>
        public void HandleScoreObjectOnBoard(MovableObject movingObject, EventArgs moveArgs)
        {
            BoardObject objectAtCell = this.boardObjectMap[movingObject.CurrentCellCol, movingObject.CurrentCellRow];
            if (objectAtCell != null)
            {
                objectAtCell.InteractWithMovingObject(movingObject);
            }
        }

        #endregion

        #region Board Position Calculation

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

        #endregion
    }
}
