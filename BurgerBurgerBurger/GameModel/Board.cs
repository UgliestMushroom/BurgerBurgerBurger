﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// Currently working with 0,0 as TOP LEFT
    /// </summary>
    public class Board
    {

        #region Events

        /// <summary>
        /// Delegate type for handling events from the BoardObject.
        /// </summary>
        /// <param name="boardObject">Object that acted</param>
        /// <param name="eventArgs">Event args</param>
        public delegate void BoardObjectHandler(BoardObject boardObject, EventArgs eventArgs);

        /// <summary>
        /// Event for when a Base is added to the Board.
        /// </summary>
        public event BoardObjectHandler BaseAddedToBoardEvent;

        /// <summary>
        /// Event for when a Spawner is added to the Board.
        /// </summary>
        public event BoardObjectHandler SpawnerAddedToBoardEvent;

        /// <summary>
        /// Event for when a Hole is added to the Board.
        /// </summary>
        public event BoardObjectHandler HoleAddedToBoardEvent;
        
        /// <summary>
        /// Delegate type for handling events about walls.
        /// </summary>
        /// <param name="cellCol">Column for one of the cells the wall borders</param>
        /// <param name="cellRow">Row for one of the cells the wall borders</param>
        /// <param name="wallPosition">Position the wall is relative to the given cell</param>
        /// <param name="eventArgs">Event args</param>
        public delegate void WallHandler(int cellCol, int cellRow, WallPositionFlags wallPosition, EventArgs eventArgs);

        /// <summary>
        /// Event for when a wall is added to the Board.
        /// </summary>
        public event WallHandler WallAddedToBoardEvent;

        /// <summary>
        /// Event for when a MovableObject is spawned on the Board.
        /// </summary>
        public event MovableObject.MovableObjectHandler ObjectSpawnedHandler;

        #endregion

        #region Board Properties

        /// <summary>
        /// Game state, whether it's running or not
        /// </summary>
        public bool GameRunning { get; set; }

        /// <summary>
        /// X position where the Board starts on the screen (does not assume start at 0); 0,0 is top left
        /// </summary>
        public int GameBoardStartX { get; private set; }

        /// <summary>
        /// Y position where Board starts on the screen (does not assume start at 0); 0,0 is top left
        /// </summary>
        public int GameBoardStartY { get; private set; }

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
        public int NumBoardCols { get; private set; }

        /// <summary>
        /// Number of Board rows
        /// </summary>
        public int NumBoardRows { get; private set; }

        /// <summary>
        /// Width of each cell in x/y units
        /// </summary>
        public int CellWidth { get; private set; }

        /// <summary>
        /// Height of each cell in x/y units
        /// </summary>
        public int CellHeight { get; private set; }

        /// <summary>
        /// Map of BoardObjects on the Board's grid of cells
        /// </summary>
        private BoardObject[,] boardObjectMap;

        /// <summary>
        /// Object encapsulating all the wall objects on the Board
        /// </summary>
        private BoardWalls BoardWalls { get; set; }

        /// <summary>
        /// List of Spawners on the Board
        /// </summary>
        private List<Spawner> Spawners { get; set; }

        private Object lockObject = new Object();

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

            this.BoardWalls = new BoardWalls(this.NumBoardCols, this.NumBoardRows);
        }

        /// <summary>
        /// Add an object to the Board.
        /// </summary>
        /// <param name="boardObject">Object to place on the Board</param>
        /// <param name="throwIfOccupied">True to throw if an object is already at that cell; false to ignore</param>
        public void AddObjectToBoard(BoardObject boardObject, bool throwIfOccupied = true)
        {
            if (IsCellOccupied(boardObject.CellCol, boardObject.CellRow))
            {
                if (throwIfOccupied)
                {
                    throw new ArgumentException(String.Format("Item is already present on the board at [{0},{1}].", boardObject.CellCol, boardObject.CellRow));
                }
                else
                {
                    return;
                }
            }

            lock (this.lockObject)
            {
                this.boardObjectMap[boardObject.CellCol, boardObject.CellRow] = boardObject;
            }
        }

        /// <summary>
        /// Update an object on the Board, either moving it from by cellCol/cellRow to the location specified by boardObject, 
        /// or simply updating the properties of the object in place.
        /// If no object is present at cellCol,cellRow, this function throws.
        /// </summary>
        /// <param name="cellCol">Cell column where the object is currently placed</param>
        /// <param name="cellRow">Cell row where the object is currently placed</param>
        /// <param name="boardObject">Object to update (update the col/row fields of this param to move it on the board)</param>
        public void UpdateObjectOnBoard(int cellCol, int cellRow, BoardObject boardObject)
        {
            if (!IsCellOccupied(cellCol, cellRow))
            {
                throw new ArgumentException("Cannot update cell [{0},{1}] because nothing is there yet!");
            }

            // If the "update" is really a "move", then the board removes then re-adds it
            // If it's just changing non-positional properties in the same cell, we just overwrite it
            // Either way, we present it to the world as an update, not a remove+add or just add
            if (boardObject.CellCol != cellCol || boardObject.CellRow != cellRow)
            {
                this.RemoveObjectFromBoard(cellCol, cellRow);
            }

            this.AddObjectToBoard(boardObject, false);  // TODO: arrows shouldn't throw...need to be careful with what else use update for!
        }

        /// <summary>
        /// Remove an object from the Board map.  If nothing is at the given cell, it continues silently.
        /// </summary>
        /// <param name="cellCol">Cell column to remove object from</param>
        /// <param name="cellRow">Cell row to remove object from</param>
        public void RemoveObjectFromBoard(int cellCol, int cellRow)
        {
            BoardObject objectToRemove;

            lock (this.lockObject)
            {
                objectToRemove = this.boardObjectMap[cellCol, cellRow];
                if (objectToRemove != null)
                {
                    this.boardObjectMap[cellCol, cellRow] = null;
                }
            }
        }

        /// <summary>
        /// Add a Base object to the Board.
        /// </summary>
        /// <param name="cellCol">Cell column where the Base will be placed</param>
        /// <param name="cellRow">Cell row where the Base will be placed</param>
        /// <param name="owner">Player who owns this base</param>
        public void AddBaseToBoard(int cellCol, int cellRow, Player owner)
        {
            Base playerBase = new Base(cellCol, cellRow, owner);
            this.AddObjectToBoard(playerBase);

            if (this.BaseAddedToBoardEvent != null)
            {
                this.BaseAddedToBoardEvent(playerBase, null);
            }
        }
        
        /// <summary>
        /// Add a Spawner object to the Board.
        /// </summary>
        /// <param name="cellCol">Cell column where the Spawner will be placed</param>
        /// <param name="cellRow">Cell row where the Spawner will be placed</param>
        /// <param name="spawnDirection">Direction the Spawner will spawn new objects</param>
        public void AddSpawnerToBoard(int cellCol, int cellRow, Direction spawnDirection = GameSettings.DEFAULT_SPAWNER_DIRECTION)
        {
            Spawner spawner = new Spawner(cellCol, cellRow, spawnDirection);
            spawner.ObjectSpawnedHandler += this.ObjectSpawnedHandler;
            this.AddObjectToBoard(spawner);
            this.Spawners.Add(spawner);

            if (this.SpawnerAddedToBoardEvent != null)
            {
                this.SpawnerAddedToBoardEvent(spawner, null);
            }
        }

        /// <summary>
        /// Add a wall between two cells to the Board.
        /// </summary>
        /// <param name="cell1Col">Column of the first cell adjacent to the wall</param>
        /// <param name="cell1Row">Row of the first cell adjacent to the wall</param>
        /// <param name="cell2Col">Column of the second cell adjacent to the wall</param>
        /// <param name="cell2Row">Row of the second cell adjacent to the wall</param>
        public void AddWallToBoard(int cell1Col, int cell1Row, int cell2Col, int cell2Row)
        {
            this.BoardWalls.AddWall(cell1Col, cell1Row, cell2Col, cell2Row);

            if (this.WallAddedToBoardEvent != null)
            {
                this.WallAddedToBoardEvent(cell1Col, cell1Row, BoardWalls.CalculatePositionOfWall(cell1Col, cell1Row, cell2Col, cell2Row), null);
            }
        }

        /// <summary>
        /// Add a Hole object to the Board.
        /// </summary>
        /// <param name="cellCol">Cell column where the Hole will be placed</param>
        /// <param name="cellRow">Cell row where the Hole will be placed</param>
        public void AddHoleToBoard(int cellCol, int cellRow)
        {
            Hole hole = new Hole(cellCol, cellRow);
            this.AddObjectToBoard(hole);

            if (this.HoleAddedToBoardEvent != null)
            {
                this.HoleAddedToBoardEvent(hole, null);
            }
        }

        /// <summary>
        /// Check if the cell at a given location already has a BoardObject present.
        /// </summary>
        /// <param name="cellCol">Board cell column to check</param>
        /// <param name="cellRow">Board cell row to check</param>
        /// <returns>True if the cell is empty; false otherwise</returns>
        public bool IsCellOccupied(int cellCol, int cellRow)
        {
            lock (this.lockObject)
            {
                return (this.boardObjectMap[cellCol, cellRow] != null);
            }
        }

        #endregion

        /// <summary>
        /// Start a new game.
        /// </summary>
        /// <param name="dispatch">Dispatcher to run background tasks.</param>
        public void StartNewGame(CoreDispatcher dispatch)
        {
            this.GameRunning = true;
            foreach(Spawner spawner in this.Spawners)
            {
                dispatch.RunAsync(CoreDispatcherPriority.Normal, () => spawner.Start(dispatch));
            }
        }

        /// <summary>
        /// End the current game.
        /// </summary>
        public void EndGame()
        {
            this.GameRunning = false;
        }

        #region Board Interaction

        /// <summary>
        /// Check if a given X and Y position is out of bounds for the Board
        /// </summary>
        /// <param name="x">X coordinate to check</param>
        /// <param name="y">Y coordinate to check</param>
        /// <returns>True if the position is out of bounds, false otherwise</returns>
        public bool IsPositionOutOfBounds(int x, int y)
        {
            return (x < this.GameBoardStartX || y < this.GameBoardStartY || x > this.GameBoardEndX || y > this.GameBoardEndY);
        }

        /// <summary>
        /// Get the cell coordiantes if a given object moved to a given X and Y location from their current location.
        /// If there's a wall in the way, throw <see cref="WouldHitWallException"/>.
        /// </summary>
        /// <param name="movableObject">Object that is moving</param>
        /// <param name="xToMoveTo">X coordinate it would move to</param>
        /// <param name="yToMoveTo">Y coordinate it would move to</param>
        /// <returns>Cell location if the move is allowed; otherwise throws</returns>
        public int[] GetCellIfMovedTo(MovableObject movableObject, int xToMoveTo, int yToMoveTo)
        {
            int[] newCell = CalculateCellFromPosition(xToMoveTo, yToMoveTo);

            if (this.BoardWalls.WouldHitWall(movableObject.CurrentCellCol, movableObject.CurrentCellRow, newCell[0], newCell[1]))
            {
                throw new WouldHitWallException();
            }

            return newCell;
        }

        /// <summary>
        /// Event that handles when a MovableObject moves across the Board.
        /// </summary>
        /// <param name="movingObject">Object moving across the Board.</param>
        /// <param name="moveArgs">Event arguments</param>
        public void HandleMovingObjectOnBoard(MovableObject movingObject, EventArgs moveArgs)
        {
            lock (this.lockObject)
            {
                BoardObject objectAtCell = this.boardObjectMap[movingObject.CurrentCellCol, movingObject.CurrentCellRow];
                if (objectAtCell != null)
                {
                    objectAtCell.InteractWithMovingObject(movingObject);
                }
            }
        }

        #endregion

        #region Board Position Calculation

        /// <summary>
        /// Convert absolute coordinates (like a click) to cells on a the game Board.
        /// </summary>
        /// <param name="absoluteXCoord">Absolute X coordinate</param>
        /// <param name="absoluteYCoord">Absolute Y coordinate</param>
        /// <returns>An int array with [cell column, cell row]</returns>
        private int[] CalculateCellFromPosition(int absoluteXCoord, int absoluteYCoord)
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
        
        /// <summary>
        /// Convert cell positions into absolute x and y coordinates.
        /// </summary>
        /// <param name="cellCol">Cell col</param>
        /// <param name="cellRow">Cell row</param>
        /// <returns>An int array with [absolute x coordinate, absolute y coordinate]</returns>
        public int[] ConvertCellCoordinatesToAbsolute(int cellCol, int cellRow)
        {
            return new int[] { ConvertCellColToXAbsolute(cellCol), ConvertCellRowToYAbsolute(cellRow) };
        }

        /// <summary>
        /// Convert a cell column position into an absolute X coordinate.  The coordinate is at the middle of the cell column.
        /// </summary>
        /// <param name="cellCol">Cell col</param>
        /// <returns>Absolute x coordinate at the middle of the cell column</returns>
        private int ConvertCellColToXAbsolute(int cellCol)
        {
            double boardXCoordinate = ((double)cellCol + 0.5) * this.CellWidth;
            int absoluteXCoordinate = ((int)boardXCoordinate) + this.GameBoardStartX;
            return absoluteXCoordinate;
        }

        /// <summary>
        /// Convert a cell row position into an absolute Y coordinate.  The coordinate is at the middle of the cell row.
        /// </summary>
        /// <param name="cellRow">Cell row</param>
        /// <returns>Absolute y coordinate at the middle of the cell row</returns>
        private int ConvertCellRowToYAbsolute(int cellRow)
        {
            double boardYCoordinate = ((double)cellRow + 0.5) * this.CellHeight;
            int absoluteYCoordinate = ((int)boardYCoordinate) + this.GameBoardStartY;
            return absoluteYCoordinate;
        }
        #endregion
    }
}
