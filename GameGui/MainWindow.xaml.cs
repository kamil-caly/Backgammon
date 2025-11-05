using GameLogic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Backgammon
{
    public partial class MainWindow : Window
    {
        private SolidColorBrush boardColor = new SolidColorBrush(Color.FromRgb(40, 80, 66));
        private SolidColorBrush redFieldColor = new SolidColorBrush(Color.FromRgb(170, 36, 36));
        private SolidColorBrush whiteFieldColor = new SolidColorBrush(Color.FromRgb(134, 144, 124));
        private SolidColorBrush redPawnColor = new SolidColorBrush(Color.FromRgb(238, 34, 17));
        private SolidColorBrush whitePawnColor = new SolidColorBrush(Color.FromRgb(238, 238, 238));
        private readonly string assetsPath = "assets/";
        private Random rand;
        private const int pawnSize = 70;
        private const int maxPawnLenInCol = 350;
        private const int pawnTopStart = 12;
        private const int pawnDownStart = 758;
        GameState gameState;

        private Image firstDice = default!;
        private Image secondDice = default!;
        //private List<Ellipse> pawns = default!;

        public MainWindow()
        {
            InitializeComponent();
            gameState = new GameState();
            rand = new Random();
            //pawns = new List<Ellipse>();
            DrawBoard();
            DrawPawns();
            DrawDice();
        }

        private void DrawBoard()
        {
            // Łączna szerokość -> 992 = 70 + 420 + 70 + 420 + 12
            // Lewy i prawy panel, lewa i prawa banda
            Rectangle leftPanel = new Rectangle
            {
                Width = 420,
                Height = 816,
                Fill = boardColor,
            };
            
            MyCanvas.Children.Add(leftPanel);
            Canvas.SetLeft(leftPanel, 70);
            Canvas.SetTop(leftPanel, 12);

            Rectangle rightPanel = new Rectangle
            {
                Width = 420,
                Height = 816,
                Fill = boardColor,
            };

            MyCanvas.Children.Add(rightPanel);
            Canvas.SetLeft(rightPanel, 140 + 420);
            Canvas.SetTop(rightPanel, 12);

            // pola - trójkąty
            for (int i = 0; i < 12; i++)
            {
                // górne pola
                Polygon triangle = new Polygon
                {
                    Fill = (i % 2 == 0) ? redFieldColor : whiteFieldColor,
                    Points = new PointCollection
                    {
                        new Point(0, 0),
                        new Point(70, 0),
                        new Point(35, 360)
                    }
                };
                MyCanvas.Children.Add(triangle);
                Canvas.SetLeft(triangle, 70 + (i < 6 ? i : i + 1) * 70);
                Canvas.SetTop(triangle, 12);

                // dolne pola
                triangle = new Polygon
                {
                    Fill = (i % 2 == 0) ? whiteFieldColor : redFieldColor,
                    Points = new PointCollection
                    {
                        new Point(0, 0),
                        new Point(70, 0),
                        new Point(35, -360)
                    }
                };
                MyCanvas.Children.Add(triangle);
                Canvas.SetLeft(triangle, 70 + (i < 6 ? i : i + 1) * 70);
                Canvas.SetTop(triangle, 828);
            }

            // pionki
            // test
            //Ellipse pawn = new Ellipse
            //{
            //    Width = 70,
            //    Height = 70,
            //    Stroke = Brushes.Black,
            //    StrokeThickness = 3,
            //    Fill = redPawnColor,
            //};

            //MyCanvas.Children.Add(pawn);
            //Canvas.SetLeft(pawn, 70);
            //Canvas.SetTop(pawn, 12);
        }

        private void DrawPawns()
        {
            for (int c = 0; c < 12; c++)
            {
                //góra
                BoardField fieldTop = gameState.getBoardField(0, c);
                for (int a = 1; a <= fieldTop.amount; a++)
                {
                    MyCanvas.Children.Add(CreatePawn(c, a, fieldTop.amount, fieldTop.player, true));
                }

                //dół
                BoardField fieldBottom = gameState.getBoardField(1, c);
                for (int a = 1; a <= fieldBottom.amount; a++)
                {
                    MyCanvas.Children.Add(CreatePawn(c, a, fieldBottom.amount, fieldBottom.player, false));
                }
            }
        }

        private Ellipse CreatePawn(int left, int who, int amount, Player player, bool isTop)
        {
            Ellipse pawn = new Ellipse
            {
                Width = pawnSize,
                Height = pawnSize,
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                Fill = player == Player.red ? redPawnColor : whitePawnColor,
            };

            int top = 0;
            if (amount <= 5)
            {
                if (isTop) top = pawnTopStart + (who - 1) * pawnSize;
                else top = pawnDownStart - (who - 1) * pawnSize;
            }
            else
            {
                double space = (double)(maxPawnLenInCol - pawnSize) / (double)(amount - 1);
                if (isTop) top = top + (int)Math.Round((who - 1) * space);
                else top = pawnDownStart - (int)Math.Round((who - 1) * space);
            }

            Canvas.SetLeft(pawn, pawnSize + (left < 6 ? left : left + 1) * pawnSize);
            Canvas.SetTop(pawn, top);

            return pawn;
        }

        private void DrawDice()
        {
            MyCanvas.Children.Add(CreateDice(690, 385, "d1", firstDice));
            MyCanvas.Children.Add(CreateDice(790, 385, "d2", secondDice));
        }

        private Image CreateDice(double left, double top, string name, Image dice)
        {
            int amount = rand.Next(1, 7);
            if (name == "d1") gameState.firstDice = amount;
            else gameState.secondDice = amount;

            dice = new Image
            {
                Width = 74,
                Height = 74,
                Name = name,
                Source = new BitmapImage(new Uri(assetsPath + GetDicePath(amount), UriKind.Relative))
            };

            dice.RenderTransform = new RotateTransform(15, dice.Width / 2, dice.Height / 2);

            Canvas.SetLeft(dice, left);
            Canvas.SetTop(dice, top);

            return dice;
        }

        private string GetDicePath(int amount)
        {
            switch (amount)
            {
                case 1: return "dice-one.png";
                case 2: return "dice-two.png";
                case 3: return "dice-three.png";
                case 4: return "dice-four.png";
                case 5: return "dice-five.png";
                case 6: return "dice-six.png";
                default: return "";
            }
        }
    }
}