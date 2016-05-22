using Philhuge.Projects.BurgerBurgerBurger;
using Philhuge.Projects.BurgerBurgerBurger.GameModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BurgerBurgerBurger
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SolidColorBrush cellHighlightWhenSelected = new SolidColorBrush(Colors.Aquamarine);
        private double cellHighlightThickness = 5.0;
        private Player player1;
        private UiBuilder uiBuilder;

        public MainPage()
        {
            this.InitializeComponent();

            // Configure the Board and register callbacks for static objects
            Board.ConfigureInstance(0, 0, 360, 360, 6, 6);
            Board.Instance.BaseAddedToBoardEvent += this.Base_AddedToBoardCallback;
            Board.Instance.HoleAddedToBoardEvent += this.Hole_AddedToBoardCallback;
            Board.Instance.WallAddedToBoardEvent += this.Wall_AddedToBoardCallback;
            
            // Create the UI Builder object and have it create the grid
            this.uiBuilder = new UiBuilder();
            this.uiBuilder.CreateGrid(this.GameHolderGrid, this.DynamicCheckerBoardPanel, this.GridCellPointerEntered, this.GridCellPointerExited, this.GridCellPointerPressed);

            // Add the Players, register callbacks, and inform the UI builder of the player
            player1 = new Player();
            player1.ArrowPlacementEvent += this.Arrow_AddedToBoardCallback;
            this.uiBuilder.AddPlayer(player1, "blue", GameSettings.MaxArrowsPerPlayer);

            // Set up the Board
            // Bases
            Board.Instance.AddBaseToBoard(2, 2, player1);

            // Spawners

            // Walls
            Board.Instance.AddWallToBoard(0, 0, 1, 0); // right
            Board.Instance.AddWallToBoard(5, 5, 4, 5); // left
            Board.Instance.AddWallToBoard(3, 3, 3, 2); // up 
            Board.Instance.AddWallToBoard(0, 0, 0, 1); // down
            
            // Holes
            Board.Instance.AddHoleToBoard(5, 1);

            // Start the game
        }

        #region UI Interaction Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridCellPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Rectangle cell = sender as Rectangle;
            string name = cell.Name;
            string[] nameTokens = cell.Name.Split('_');
            int col = Int32.Parse(nameTokens[1]);
            int row = Int32.Parse(nameTokens[2]);

            Direction randomDirection;
            Enum.TryParse(GameSettings.RANDOM.Next(0, 4).ToString(), out randomDirection);

            player1.PlaceArrow(col, row, randomDirection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridCellPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Rectangle cell = sender as Rectangle;
            cell.Stroke = cellHighlightWhenSelected;
            cell.StrokeThickness = cellHighlightThickness;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridCellPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Rectangle cell = sender as Rectangle;
            cell.Stroke = null;
        }

        #endregion

        #region GameModel Callbacks and Callback Helpers

        /// <summary>
        /// Callback for the event fired when a base is added to the game Board.
        /// </summary>
        /// <param name="boardObject">BoardObject (base) added to the Board</param>
        /// <param name="eventArgs">Event args (unused)</param>
        private void Base_AddedToBoardCallback(BoardObject boardObject, EventArgs eventArgs)
        {
            Base baseAdded = boardObject as Base;
            UiBuilder.AddItemToGrid(this.DynamicBasePanel, this.uiBuilder.CreateBaseImage(baseAdded));
        }

        /// <summary>
        /// Callback for the event fired when an arrow is added to the game Board.
        /// </summary>
        /// <param name="player">Player who owns the added Arrow</param>
        /// <param name="arrow">Arrow added to the Board</param>
        /// <param name="eventArgs">Event args (unused)</param>
        private void Arrow_AddedToBoardCallback(Player player, Arrow arrow, EventArgs eventArgs)
        {
            // CreateOrUpdateArrowImage will return null if it updated an existing arrow image
            Image newArrowImage = this.uiBuilder.CreateOrUpdateArrowImage(player, arrow);
            if (newArrowImage != null)
            {
                UiBuilder.AddItemToGrid(this.DynamicArrowPanel, newArrowImage);
            }
        }

        /// <summary>
        /// Callback for the event fired when a hole is added to the game Board.
        /// </summary>
        /// <param name="boardObject">BoardObject (hole) added to the Board</param>
        /// <param name="eventArgs">Event args (unused)</param>
        private void Hole_AddedToBoardCallback(BoardObject boardObject, EventArgs eventArgs)
        {
            Hole holeAdded = boardObject as Hole;
            UiBuilder.AddItemToGrid(this.DynamicHolePanel, this.uiBuilder.CreateHoleImage(holeAdded));
        }

        /// <summary>
        /// Callback for the event fired when a wall is added to the game Board.
        /// </summary>
        /// <param name="cellCol">Column for one of the cells the wall borders</param>
        /// <param name="cellRow">Row for one of the cells the wall borders</param>
        /// <param name="wallPosition">Position the wall is relative to the given cell</param>
        /// <param name="eventArgs">Event args (unused)</param>
        private void Wall_AddedToBoardCallback(int cellCol, int cellRow, WallPositionFlags wallPosition, EventArgs eventArgs)
        {
            if (wallPosition == WallPositionFlags.Left || wallPosition == WallPositionFlags.Right)
            {
                UiBuilder.AddItemToGrid(this.DynamicVertWallPanel,
                    this.uiBuilder.CreateVerticalWallImage(cellCol, cellRow, wallPosition));
            }
            else if (wallPosition == WallPositionFlags.Up || wallPosition == WallPositionFlags.Down)
            {
                UiBuilder.AddItemToGrid(this.DynamicHorizWallPanel,
                    this.uiBuilder.CreateHorizontalWallImage(cellCol, cellRow, wallPosition));
            }
        } 
        
        #endregion

    }
}

