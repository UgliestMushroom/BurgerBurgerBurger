using Philhuge.Projects.BurgerBurgerBurger.GameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Philhuge.Projects.BurgerBurgerBurger
{
    public class UiBuilder
    {

        private int CellWidthInPx { get; set; } 
        private int CellHeightInPx { get; set; }
        private Dictionary<Player, string> PlayerImageTagMap { get; set; }
        private Dictionary<Player, Image[]> PlayerArrowsMap { get; set; }

        public UiBuilder()
        {
            this.CellWidthInPx = Board.Instance.CellWidth;
            this.CellHeightInPx = Board.Instance.CellHeight;
            this.PlayerImageTagMap = new Dictionary<Player, string>();
            this.PlayerArrowsMap = new Dictionary<Player, Image[]>();
        }

        public void AddPlayer(Player player, string imageTag, int numArrows)
        {
            this.PlayerImageTagMap.Add(player, imageTag);
            this.PlayerArrowsMap.Add(player, new Image[numArrows]);
        }

        /// <summary>
        /// Create an Image object representing a Base at a position.
        /// </summary>
        /// <param name="baseAdded">Base object to create an image for</param>
        /// <returns>Image object</returns>
        public Image CreateBaseImage(Base baseAdded)
        {
            string baseImageFilePath = String.Format(@"ms-appx:/Assets/base_{0}.png", this.PlayerImageTagMap[baseAdded.Player]);

            Image baseImage = new Image
            {
                Source = new BitmapImage(new Uri(baseImageFilePath)),
                Width = this.CellWidthInPx - 10,
                Height = this.CellHeightInPx - 10,
                Visibility = Visibility.Visible,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(baseAdded.CellCol * this.CellWidthInPx + 5, baseAdded.CellRow * this.CellHeightInPx + 5, 0, 0)
            };
            return baseImage;
        }

        /// <summary>
        /// Create an Image object representing a Hole at a position.
        /// </summary>
        /// <param name="hole">Hole object to create an image for</param>
        /// <returns>Image object</returns>
        public Image CreateHoleImage(Hole hole)
        {
            Image holeImage = new Image
            {
                Source = new BitmapImage(new Uri(@"ms-appx:/Assets/hole.png")),
                Width = this.CellWidthInPx,
                Height = this.CellHeightInPx,
                Visibility = Visibility.Visible,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(hole.CellCol * this.CellWidthInPx, hole.CellRow * this.CellHeightInPx, 0, 0)
            };
            return holeImage;
        }

        /// <summary>
        /// Create an Image object representing a vertical wall at a position.
        /// </summary>
        /// <param name="cellCol">Column of the cell where the wall lives</param>
        /// <param name="cellRow">Row of the cell where the wall lives</param>
        /// <param name="wallPosition">Position in the cell where the wall lives</param>
        /// <returns>Image object</returns>
        public Image CreateVerticalWallImage(int cellCol, int cellRow, WallPositionFlags wallPosition)
        {
            int xPosition = cellCol * this.CellWidthInPx + (wallPosition == WallPositionFlags.Left ? 0 : this.CellWidthInPx) - 2;
            int yPosition = cellRow * this.CellHeightInPx;

            Image vertWall = new Image
            {
                Source = new BitmapImage(new Uri(@"ms-appx:/Assets/wall_vert.png")),
                Width = 4,
                Height = this.CellHeightInPx,
                Visibility = Visibility.Visible,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
            return vertWall;
        }

        /// <summary>
        /// Create an Image object representing a horizontal wall at a position.
        /// </summary>
        /// <param name="cellCol">Column of the cell where the wall lives</param>
        /// <param name="cellRow">Row of the cell where the wall lives</param>
        /// <param name="wallPosition">Position in the cell where the wall lives</param>
        /// <returns>Image object</returns>
        public Image CreateHorizontalWallImage(int cellCol, int cellRow, WallPositionFlags wallPosition)
        {
            int xPosition = cellCol * this.CellWidthInPx;
            int yPosition = cellRow * this.CellHeightInPx + (wallPosition == WallPositionFlags.Up ? 0 : this.CellHeightInPx) - 2;

            Image horizWall = new Image
            {
                Source = new BitmapImage(new Uri(@"ms-appx:/Assets/wall_horiz.png")),
                Width = this.CellWidthInPx,
                Height = 4,
                Visibility = Visibility.Visible,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
            return horizWall;
        }

        /// <summary>
        /// If the Player has placed max Arrows already, update the oldest existing arrow and return that image.
        /// If the Player has not, create a new Image.
        /// </summary>
        /// <param name="player">Player who owns the Arrow</param>
        /// <param name="arrow">Arrow object to create an Image for</param>
        /// <returns>Image object, if a new image was created.  If it's only updating, returns null.</returns>
        public Image CreateOrUpdateArrowImage(Player player, Arrow arrow)
        {
            Image[] playerArrows = this.PlayerArrowsMap[player];
            if (playerArrows == null)
            {
                throw new ArgumentException("Player has not been added to UI builder yet, so CreateArrowImage failed.");
            }

            string imageFilepath = String.Format(@"ms-appx:/Assets/arrow_{0}_{1}.png", this.PlayerImageTagMap[player], arrow.PointDirection.ToString());
            BitmapImage imageSource = new BitmapImage(new Uri(imageFilepath));
            Thickness margin = new Thickness(arrow.CellCol * this.CellWidthInPx + 15, arrow.CellRow * this.CellHeightInPx + 15, 0, 0);

            Image arrowImage = playerArrows[arrow.Index];
            // Create new image
            if (arrowImage == null)
            {
                arrowImage = new Image
                {
                    Source = imageSource,
                    Width = 30,
                    Height = 30,
                    Visibility = Visibility.Visible,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = margin
                };

                playerArrows[arrow.Index] = arrowImage;
                return arrowImage;
            }
            // Update existing image
            else
            {
                arrowImage.Source = imageSource;
                arrowImage.Margin = margin;
                playerArrows[arrow.Index] = arrowImage;
                return null;
            }
        }

        /// <summary>
        /// Add an Image to a Grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="image"></param>
        public static void AddItemToGrid(Grid grid, Image image)
        {
            grid.Children.Add(image);
        }
    }
}
