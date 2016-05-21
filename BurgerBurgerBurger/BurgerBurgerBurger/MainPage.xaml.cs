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
        private SolidColorBrush cellHighlightWhenSelected;

        private string clickTargetTextTextFormat = "Last click was on: {0}, {1}";

        private List<Ellipse> Player1Arrows;

        private Player player1;
        

        public MainPage()
        {
            this.InitializeComponent();

            cellHighlightWhenSelected = new SolidColorBrush(Colors.Aquamarine);

            Board.ConfigureInstance(0, 0, 360, 360, 6, 6);
            player1 = new Player();
            //Board.Instance.AddBaseToBoard(2, 2, player1);

            player1.ArrowPlacementEvent += this.Player1_ArrowAdded;

            Player1Arrows = new List<Ellipse>();
            Player1Arrows.Add(Player1_Arrow1);
            Player1Arrows.Add(Player1_Arrow2);
            Player1Arrows.Add(Player1_Arrow3);
        }

        private void Player1_ArrowAdded(Player player, Arrow arrow, EventArgs eventArgs)
        {
            switch (arrow.Index)
            {
                case 0:
                    this.UpdateArrow(Player1_Arrow1, arrow);
                    break;
                case 1:
                    this.UpdateArrow(Player1_Arrow2, arrow);
                    break;
                case 2:
                    this.UpdateArrow(Player1_Arrow3, arrow);
                    break;
                default:
                    break;
            }
        }

        private void UpdateArrow(Ellipse arrowVisual, Arrow arrow)
        {
            arrowVisual.Margin = new Thickness(arrow.CellCol * 60 + 5, arrow.CellRow * 60 + 5, 0, 0);
            arrowVisual.Visibility = Visibility.Visible;
        }

        private void GridCellPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Rectangle cell = sender as Rectangle;
            cell.Stroke = cellHighlightWhenSelected;
            cell.StrokeThickness = 5.0;
        }

        private void GridCellPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Rectangle cell = sender as Rectangle;
            cell.Stroke = null;
        }

        private void GridCellPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Rectangle cell = sender as Rectangle;

            string name = cell.Name;
            string[] nameTokens = cell.Name.Split('_');
            int col = Int32.Parse(nameTokens[1]);
            int row = Int32.Parse(nameTokens[2]);

            ClickTargetTestText.Text = String.Format(clickTargetTextTextFormat, col, row);

            player1.PlaceArrow(col, row, Direction.Up);
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(Ellipse arrow in Player1Arrows)
            {
                arrow.Visibility = Visibility.Collapsed;
            }
        }

    }
}
