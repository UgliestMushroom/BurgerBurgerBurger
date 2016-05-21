using System;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// Captures all the wall objects placed on a Board.
    /// </summary>
    public class BoardWalls
    {
        /// <summary>
        /// Number of Board columns 
        /// </summary>
        private int NumBoardCols { get; set; }

        /// <summary>
        /// Number of Board rows
        /// </summary>
        private int NumBoardRows { get; set; }

        /// <summary>
        /// Map where the walls are placed on the board.  Each cell contains information about what sides have walls.
        /// </summary>
        private WallPositionFlags[,] boardWallMap;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numBoardCols">Number of Board columns</param>
        /// <param name="numBoardRows">Number of Board rows</param>
        public BoardWalls(int numBoardCols, int numBoardRows)
        {
            this.NumBoardCols = numBoardCols;
            this.NumBoardRows = numBoardRows;

            boardWallMap = new WallPositionFlags[numBoardCols, numBoardRows];
        }

        /// <summary>
        /// Add a wall to the Board between two cells
        /// </summary>
        /// <param name="cell1Col">Column of the first cell the wall is between</param>
        /// <param name="cell1Row">Row of the first cell the wall is between</param>
        /// <param name="cell2Col">Column of the second cell the wall is between</param>
        /// <param name="cell2Row">Row of the second cell the wall is between</param>
        public void AddWall(int cell1Col, int cell1Row, int cell2Col, int cell2Row)
        {
            VerifyCellPositionParams(cell1Col, cell1Row);
            VerifyCellPositionParams(cell2Col, cell2Row);
            if (cell1Col == cell2Col && cell1Row == cell2Row)
            {
                throw new ArgumentException("Cells cannot be the same for wall placement!");
            }

            // Save the wall info on both cells, but opposite directions
            boardWallMap[cell1Col, cell1Row] &= CalculatePositionOfWall(cell1Col, cell1Row, cell2Col, cell2Row);
            boardWallMap[cell2Col, cell2Row] &= CalculatePositionOfWall(cell2Col, cell2Row, cell1Col, cell1Row);
        }

        /// <summary>
        /// Check if an object moving from one cell to another would run into a wall between those cells.
        /// </summary>
        /// <param name="fromCellCol">Column of the cell the object is moving from</param>
        /// <param name="fromCellRow">Row of the cell the object is moving from</param>
        /// <param name="toCellCol">Column of the cell the object is moving to</param>
        /// <param name="toCellRow">Row of the cell the object is moving to</param>
        /// <returns>True if there is a wall between the 'from cell' and 'to cell'</returns>
        public bool WouldHitWall(int fromCellCol, int fromCellRow, int toCellCol, int toCellRow)
        {
            VerifyCellPositionParams(fromCellCol, fromCellRow);
            VerifyCellPositionParams(toCellCol, toCellRow);

            WallPositionFlags expectedDirection = CalculatePositionOfWall(fromCellCol, fromCellRow, toCellCol, toCellRow);
            WallPositionFlags wallsForCell = boardWallMap[fromCellCol, fromCellRow];

            return wallsForCell.HasFlag(expectedDirection);
        }

        /// <summary>
        /// Calculate the position in the 'from cell' that a wall to the 'to cell' will be placed.
        /// </summary>
        /// <param name="fromCellCol">Column of the 'from cell' where the wall would be placed</param>
        /// <param name="fromCellRow">Row of the 'from cell' where the wall would be placed</param>
        /// <param name="toCellCol">Column of the 'to cell' where the wall would be placed</param>
        /// <param name="toCellRow">Row of the 'to cell' where the wall would be placed</param>
        /// <returns>Position of the wall in the 'from cell'</returns>
        private WallPositionFlags CalculatePositionOfWall(int fromCellCol, int fromCellRow, int toCellCol, int toCellRow)
        {
            if (fromCellCol < toCellCol)
            {
                return WallPositionFlags.Up;
            }
            else if (fromCellCol > toCellCol)
            {
                return WallPositionFlags.Down;
            }
            else if (fromCellRow < toCellRow)
            {
                return WallPositionFlags.Right;
            }
            else if (fromCellRow > toCellRow)
            {
                return WallPositionFlags.Left;
            }
            else
            {
                throw new ArgumentException("Cannot calculate wall direction between two cells.  Are the two cells equal?");
            }
        }

        /// <summary>
        /// Check that the column and row parameters are in bounds for the Board.  Throws <see cref="ArgumentOutOfRangeException"/> if they are not.
        /// </summary>
        /// <param name="col">Column parameter to check</param>
        /// <param name="row">Row parameter to check</param>
        private void VerifyCellPositionParams(int col, int row)
        {
            if (col < 0 || col >= this.NumBoardCols)
            {
                throw new ArgumentOutOfRangeException("Column parameter is out of range");
            }
            else if (row < 0 || row >= this.NumBoardRows)
            {
                throw new ArgumentOutOfRangeException("Row parameter is out of range.");
            }
        }
    }
}
