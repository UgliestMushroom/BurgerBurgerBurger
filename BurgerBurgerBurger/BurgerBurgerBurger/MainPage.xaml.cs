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

        private Image[] Player1Arrows;

        private Player player1;

        private Dictionary<Player, Image> playerBaseMap;
        private Dictionary<Player, string> playerImageTagMap;
        private Dictionary<Player, Image[]> playerArrowsMap;

        private Image[] holes;
        private int holeIndex;
        private Object holeLock = new Object();

        public MainPage()
        {
            this.InitializeComponent();

            this.playerBaseMap = new Dictionary<Player, Image>();
            this.playerImageTagMap = new Dictionary<Player, string>();
            this.playerArrowsMap = new Dictionary<Player, Image[]>();

            this.holes = new Image[6];
            this.holes[0] = this.Hole1;
            this.holes[1] = this.Hole2;
            this.holes[2] = this.Hole3;
            this.holes[3] = this.Hole4;
            this.holes[4] = this.Hole5;
            this.holes[5] = this.Hole6;

            Board.ConfigureInstance(0, 0, 360, 360, 6, 6);
            Board.Instance.BaseAddedToBoardEvent += this.Base_AddedToBoardCallback;
            Board.Instance.HoleAddedToBoardEvent += this.Hole_AddedToBoardCallback;
            Board.Instance.WallAddedToBoardEvent += this.Wall_AddedToBoardCallback;

            player1 = new Player();
            player1.ArrowPlacementEvent += this.Arrow_AddedToBoardCallback;
            Player1Arrows = new Image[GameSettings.DEFAULT_MAX_ARROWS_PER_PLAYER];
            Player1Arrows[0] = Player1_Arrow1;
            Player1Arrows[1] = Player1_Arrow2;
            Player1Arrows[2] = Player1_Arrow3;

            this.playerBaseMap.Add(player1, this.Player1_Base);
            this.playerImageTagMap.Add(player1, "blue");
            this.playerArrowsMap.Add(player1, Player1Arrows);
            

            Board.Instance.AddBaseToBoard(2, 2, player1);

            Board.Instance.AddHoleToBoard(5, 1);
            
            // right
            Board.Instance.AddWallToBoard(0, 0, 1, 0);
            // down
            Board.Instance.AddWallToBoard(0, 0, 0, 1);
            // up 
            Board.Instance.AddWallToBoard(3, 3, 3, 2);
            // left
            Board.Instance.AddWallToBoard(5, 5, 4, 5);

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
            foreach (Image arrow in Player1Arrows)
            {
                arrow.Visibility = Visibility.Collapsed;
            }
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
            Image baseUi = this.playerBaseMap[baseAdded.Player];

            baseUi.Margin = new Thickness(baseAdded.CellCol * 60 + 5, baseAdded.CellRow * 60 + 5, 0, 0);
            baseUi.Source = GetImageForPlayerBase(baseAdded.Player);
            baseUi.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Get the Image source for a Player's Base.
        /// </summary>
        /// <param name="player">Player to get the Base image for</param>
        /// <returns>ImageSource for the Player's Base image</returns>
        private ImageSource GetImageForPlayerBase(Player player)
        {
            string imageFilename = String.Format("base_{0}.png", this.playerImageTagMap[player]);
            string imageFilepath = String.Format(@"ms-appx:/Assets/{0}", imageFilename);
            return new BitmapImage(new Uri(imageFilepath));
        }

        /// <summary>
        /// Callback for the event fired when an arrow is added to the game Board.
        /// </summary>
        /// <param name="player">Player who owns the added Arrow</param>
        /// <param name="arrow">Arrow added to the Board</param>
        /// <param name="eventArgs">Event args (unused)</param>
        private void Arrow_AddedToBoardCallback(Player player, Arrow arrow, EventArgs eventArgs)
        {
            Image[] playerArrows = this.playerArrowsMap[player];
            Image arrowImage = playerArrows[arrow.Index];

            arrowImage.Margin = new Thickness(arrow.CellCol * 60 + 15, arrow.CellRow * 60 + 15, 0, 0);
            arrowImage.Source = GetImageForPlayerAndDirection(player, arrow.PointDirection);
            arrowImage.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Get the Iamge source for a Player's arrow facing a given direction.
        /// </summary>
        /// <param name="player">Player who owns the arrow</param>
        /// <param name="direction">Direction the arrow is facing</param>
        /// <returns>ImageSource for the Player's arrow image facing a given direction</returns>
        private ImageSource GetImageForPlayerAndDirection(Player player, Direction direction)
        {
            string imageFilename = String.Format("arrow_{0}_{1}.png", this.playerImageTagMap[player], direction.ToString());
            string imageFilepath = String.Format(@"ms-appx:/Assets/{0}", imageFilename);
            return new BitmapImage(new Uri(imageFilepath));
        }

        /// <summary>
        /// Callback for the event fired when a hole is added to the game Board.
        /// </summary>
        /// <param name="boardObject">BoardObject (hole) added to the Board</param>
        /// <param name="eventArgs">Event args (unused)</param>
        private void Hole_AddedToBoardCallback(BoardObject boardObject, EventArgs eventArgs)
        {
            Hole holeAdded = boardObject as Hole;
            Image holeUi = null;

            lock(this.holeLock)
            {
                holeUi = this.holes[holeIndex];
                holeIndex++;
            }

            holeUi.Margin = new Thickness(holeAdded.CellCol * 60, holeAdded.CellRow * 60, 0, 0);
            holeUi.Source = new BitmapImage(new Uri(@"ms-appx:/Assets/hole.png"));
            holeUi.Visibility = Visibility.Visible;
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
                int xPosition = cellCol * 60 + (wallPosition == WallPositionFlags.Left ? 0 : 60) - 2;
                int yPosition = cellRow * 60;
                
                Image vertWall = new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/Assets/wall_vert.png")),
                    Width = 4,
                    Height = 60,
                    Visibility = Visibility.Visible,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(xPosition, yPosition, 0, 0)
                };
                this.DynamicVertWallPanel.Children.Add(vertWall);
            }
            else if (wallPosition == WallPositionFlags.Up || wallPosition == WallPositionFlags.Down)
            {
                int xPosition = cellCol * 60;
                int yPosition = cellRow * 60 + (wallPosition == WallPositionFlags.Up ? 0 : 60) - 2;

                Image horizWall = new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/Assets/wall_horiz.png")),
                    Width = 60,
                    Height = 4,
                    Visibility = Visibility.Visible,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(xPosition, yPosition, 0, 0)
                };
                this.DynamicHorizWallPanel.Children.Add(horizWall);
            }
        }

        #endregion

    }
}
