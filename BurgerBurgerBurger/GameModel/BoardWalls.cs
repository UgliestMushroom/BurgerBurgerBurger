using System;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    public class BoardWalls
    {
        private int NumBoardCols { get; set; }
        private int NumBoardRows { get; set; }

        private WallDirectionFlags[,] boardWallMap;

        public BoardWalls(int numBoardCols, int numBoardRows)
        {
            this.NumBoardCols = numBoardCols;
            this.NumBoardRows = numBoardRows;

            boardWallMap = new WallDirectionFlags[numBoardCols, numBoardRows];
        }

        public void AddWall(int cell1Col, int cell1Row, int cell2Col, int cell2Row)
        {
            // Validate params
            VerifyCellPositionParams(cell1Col, cell1Row);
            VerifyCellPositionParams(cell2Col, cell2Row);
            if (cell1Col == cell2Col && cell1Row == cell2Row)
            {
                throw new ArgumentException("Cells cannot be the same for wall placement!");
            }

            // Save the wall info on both cells, but opposite directions
            boardWallMap[cell1Col, cell1Row] &= CalculateDirectionOfWall(cell1Col, cell1Row, cell2Col, cell2Row);
            boardWallMap[cell2Col, cell2Row] &= CalculateDirectionOfWall(cell2Col, cell2Row, cell1Col, cell1Row);
        }

        public bool WouldHitWall(int fromCellCol, int fromCellRow, int toCellCol, int toCellRow)
        {
            VerifyCellPositionParams(fromCellCol, fromCellRow);
            VerifyCellPositionParams(toCellCol, toCellRow);

            WallDirectionFlags expectedDirection = CalculateDirectionOfWall(fromCellCol, fromCellRow, toCellCol, toCellRow);
            WallDirectionFlags wallsForCell = boardWallMap[fromCellCol, fromCellRow];

            return wallsForCell.HasFlag(expectedDirection);
        }

        private WallDirectionFlags CalculateDirectionOfWall(int fromCellCol, int fromCellRow, int toCellCol, int toCellRow)
        {
            if (fromCellCol < toCellCol)
            {
                return WallDirectionFlags.Up;
            }
            else if (fromCellCol > toCellCol)
            {
                return WallDirectionFlags.Down;
            }
            else if (fromCellRow < toCellRow)
            {
                return WallDirectionFlags.Right;
            }
            else if (fromCellRow > toCellRow)
            {
                return WallDirectionFlags.Left;
            }
            else
            {
                throw new ArgumentException("Cannot calculate wall direction between two cells.  Are the two cells equal?");
            }
        }

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
