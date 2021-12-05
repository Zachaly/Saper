using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace SaperWPF
{
    /// <summary>
    /// Logika interakcji dla klasy GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        int _flags, _bombs;

        static public bool gameEnd { get; set; }

        int xAxis, yAxis, bombs;

        public int Flags
        {
            get { return _flags; }
            set
            {
                _flags = value;
                FlagCount.Content = "Remaining flags: " + _flags.ToString();
            }
        }

        public int Bombs { get { return _bombs; } }
        public int BombsToFlag { get; set; }

        int GameWidth { get { return Tiles.Length; } }
        int GameHeight { get { return Tiles[0].Length; } }

        int Time = 0;
        System.Windows.Threading.DispatcherTimer Timer;

        public Tile[][] Tiles;

        public GameWindow()
        {
            InitializeComponent();
        }


        public GameWindow(int xAxis, int yAxis, int bombs)
        {
            InitializeComponent();

            _bombs = bombs;
            Flags = bombs;
            BombsToFlag = bombs;

            this.bombs = bombs;
            this.xAxis = xAxis;
            this.yAxis = yAxis;

            gameEnd = false;
            Tile.FirstClicked = false;


            CreateGame();
        }

        void CreateGame()
        {
            CreateTiles(xAxis, yAxis);
            SetNearbyTiles();
        }

        //Just generates gametiles and sets window width/height based on number of them
        void CreateTiles(int x, int y)
        {
            Tiles = new Tile[x][];

            Tile newTile = null;
            for(int i=0; i < x; i++)
            {
                Tiles[i] = new Tile[y];
                TilesManager.ColumnDefinitions.Insert(i, new ColumnDefinition());
                for(int j = 0; j < y; j++)
                {
                    newTile = new Tile(i, j, this);
                    Tiles[i][j] = newTile;
                    TilesManager.RowDefinitions.Insert(j, new RowDefinition());
                    TilesManager.Children.Add(newTile);
                    
                }
            }


            Width = newTile.Width * x;
            Height = newTile.Height * y + FlagCount.Height;
        }

        //Uses SetNearbyTiles function on every gametile
        void SetNearbyTiles()
        {
            for(int i=0; i < GameWidth; i++)
            {
                for(int j=0; j < GameHeight; j++)
                {
                    Tiles[i][j].SetNearbyTiles();
                }
            }
        }

        //Sets whitch tile has a bomb, updates info about bombs around a tile, and starts timer with game time
        public void GenerateBombs()
        {
            Random rand = new Random();

            int x = 0, y = 0, bombsplaced = 0;

            Tile tile = null;

            while(bombsplaced < bombs)
            {
                x = rand.Next(GameWidth) - 1;
                y = rand.Next(GameHeight) - 1;

                
                try
                {
                    tile = Tiles[x][y];
                }
                catch(IndexOutOfRangeException e) { continue; }

                foreach (var item in tile.nearbyTiles)
                    if (item.FirstClickedTile)
                    {
                        tile = item;
                        break;
                    }

                if (tile.Bombed || tile.FirstClickedTile) continue;


                tile.setNearbyBombs();

                if (tile.NearbyBombs >= 5) continue;

                tile.Bomb();
                bombsplaced++;
            }

            for(int i=0; i < GameWidth; i++)
            {
                for (int j = 0; j < GameHeight; j++)
                    Tiles[i][j].setNearbyBombs();
            }

            Timer = new System.Windows.Threading.DispatcherTimer();
            Timer.Tick += (sender, e) =>
            {
                Time += 1;
                TimerLabel.Content = $"Time: {Time}";
            };
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        //Stops the game
        public void GameOver()
        {
            Timer.Stop();
            gameEnd = true;
            Tile tile = null;
            for (int i = 0; i < GameWidth; i++)
            {
                for (int j = 0; j < GameHeight; j++)
                {
                    tile = Tiles[i][j];
                    tile.Uncovered = true;
                }
            }

            MessageBox.Show("Saper, podobnie jak trakciarz, myli sie tylko raz...");
        }
    }
}
