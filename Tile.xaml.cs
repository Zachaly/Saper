using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace SaperWPF
{
    /// <summary>
    /// Logika interakcji dla klasy Tile.xaml
    /// </summary>
    
    public partial class Tile : UserControl
    {
        //private fields used by properties
        bool bombed = false, _uncovered = false, _flagged = false, _firstClickedTile = false; 
        int xAxis, yAxis, nearbybombs = 0;

        public int NearbyBombs { get { return nearbybombs; } }
        public bool Bombed { get { return bombed; } }

        static public bool FirstClicked { get; set; } = false;
        public bool FirstClickedTile { get { return _firstClickedTile; } }

        GameWindow Game;
        public List<Tile> nearbyTiles;

        //Color change while clicking the tile, gameover screen while clicking tile with a bomb
        public bool Uncovered
        {
            get { return _uncovered; }
            set
            {
                if (_uncovered) return;
                _uncovered = value;
                Resources.Remove("BG");
                Color color = Colors.Green;

                if (nearbybombs != 0 && !bombed)
                {
                    Resources.Remove("Text");
                    Resources.Add("Text", nearbybombs.ToString());
                }

                if (bombed)
                {
                    color = Colors.Red;
                    if(!GameWindow.gameEnd) Game.GameOver();
                }

                Resources.Add("BG", new SolidColorBrush(color));
                
            }
        }

        public bool Flagged
        {
            get { return _flagged; }
            set
            {
                Color color;
                if (_flagged)
                {
                    if (bombed) Game.BombsToFlag++;
                    color = Colors.Gray;
                    Game.Flags++;
                }
                else
                {
                    if (bombed) Game.BombsToFlag--;
                    color = Colors.Gold;
                    Game.Flags--;
                }

                Resources.Remove("BG");
                Resources.Add("BG", new SolidColorBrush(color));

                _flagged = value;
            }
        }

        public Tile(int xAxis, int yAxis, GameWindow game)
        {
            InitializeComponent();
            this.xAxis = xAxis;
            this.yAxis = yAxis;
            Game = game;

            Grid.SetColumn(this, xAxis);
            Grid.SetRow(this, yAxis);
        }

        public void Bomb()
        {
            bombed = true;
        }

        public void SetNearbyTiles()
        {
            int GridWidth = Game.Tiles.Length - 1;
            int GridHeight = Game.Tiles[0].Length - 1;

            nearbyTiles = new List<Tile>();

            if (xAxis > 0)
                nearbyTiles.Add(Game.Tiles[xAxis - 1][yAxis]);

            if (xAxis < GridWidth)
                nearbyTiles.Add(Game.Tiles[xAxis + 1][yAxis]);

            if (yAxis > 0)
                nearbyTiles.Add(Game.Tiles[xAxis][yAxis - 1]);

            if (yAxis < GridHeight)
                nearbyTiles.Add(Game.Tiles[xAxis][yAxis + 1]);

            if (xAxis > 0 && yAxis > 0)
                nearbyTiles.Add(Game.Tiles[xAxis - 1][yAxis - 1]);

            if (xAxis < GridWidth && yAxis > 0)
                nearbyTiles.Add(Game.Tiles[xAxis + 1][yAxis - 1]);

            if (xAxis > 0 && yAxis < GridHeight)
                nearbyTiles.Add(Game.Tiles[xAxis - 1][yAxis + 1]);

            if (xAxis < GridWidth && yAxis < GridHeight)
                nearbyTiles.Add(Game.Tiles[xAxis + 1][yAxis + 1]);
        }

        //Uncover tile when left mouse button is clicked, generate bombs during first click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!FirstClicked)
            {
                FirstClicked = true;
                _firstClickedTile = true;
                Game.GenerateBombs();
            }

            if (Flagged) return;
            else
            {
                Uncovered = true;
                if (nearbybombs != 0) return;

                //Uncovers surrounding tiles when this tile has no bombs around
                foreach(var tile in nearbyTiles)
                {
                    if (tile.Uncovered) continue;
                    if (!tile.Bombed) tile.Button_Click(tile, e);
                }
            }

        }

        //Flag a tile, win a game while all bombed tiles are flagged
        private void Button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Uncovered) return;
            if (Flagged) Flagged = false;
            else Flagged = true;

            if(Game.Flags == 0 && Game.BombsToFlag == 0)
            {
                MessageBox.Show("Dobry saper myli się tylko raz, a ty się jeszcze nie pomyliłeś");
            }
        }

        //Sets number of bombs around
        public void setNearbyBombs()
        {
            foreach(var tile in nearbyTiles)
            {
                if (tile.bombed) nearbybombs++;
            }
        }
    }
}
