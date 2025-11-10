using GameLogic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Backgammon
{
    public partial class MainWindow : Window
    {
        private readonly SolidColorBrush boardColor = new SolidColorBrush(Color.FromRgb(40, 80, 66));
        private readonly SolidColorBrush redFieldColor = new SolidColorBrush(Color.FromRgb(170, 36, 36));
        private readonly SolidColorBrush whiteFieldColor = new SolidColorBrush(Color.FromRgb(134, 144, 124));
        private readonly SolidColorBrush redPawnColor = new SolidColorBrush(Color.FromRgb(238, 34, 17));
        private readonly SolidColorBrush whitePawnColor = new SolidColorBrush(Color.FromRgb(238, 238, 238));
        private readonly string assetsPath = "assets/";
        private readonly Random rand;
        private const int pawnSize = 70;
        private const int maxPawnLenInCol = 350;
        private const int triangleFieldLen = 360;
        private const int pawnTopStart = 12;
        private const int pawnDownStart = 758;

        GameState gameState;
        private Image firstDice = default!;
        private Image secondDice = default!;
        private Ellipse currPlayerInfo;
        private List<Rectangle> possibleMovesMarks;
        private List<Ellipse> pawns;
        private Move? currentMove = null;
        private int currentMoveDice = 0;
        private Ellipse? draggedPawn;

        private bool isDragging = false;
        private bool isCapturingPawn = false;
        private bool firstDiceUsed = false;
        private bool secondDiceUsed = false;
        private Point mouseOffsetForDragLogic;
        private Point mouseOffsetWhenPawnClick;
        
        public MainWindow()
        {
            InitializeComponent();
            MyCanvas.MouseMove += MyCanvas_MouseMove;
            gameState = new GameState();
            rand = new Random();
            currPlayerInfo = new Ellipse();
            possibleMovesMarks = new List<Rectangle>();
            pawns = new List<Ellipse>();
            DrawBoard();
            DrawPawns();
            DrawDice();
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var hovered = e.OriginalSource as Ellipse;
            if (hovered != null
                && (string)hovered.Tag == "Pawn"
                && hovered.Fill == (gameState.currentPlayer == Player.red ? redPawnColor : whiteFieldColor)
            )
            {
                if (!isCapturingPawn)
                {
                    isCapturingPawn = true;
                    Debug.WriteLine($"Najechano na pionek {rand.Next(1000)}");
                    MarkPossibleMoves((Position)hovered.DataContext);
                }
            }
            else
            {
                isCapturingPawn = false;
                possibleMovesMarks.ForEach(MyCanvas.Children.Remove);
                possibleMovesMarks.Clear();
            }
        }

        #region Drawing
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

            // info czyj ruch
            currPlayerInfo = new Ellipse
            {
                Width = 35,
                Height = 35,
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                Fill = gameState.currentPlayer == Player.red ? redPawnColor : whitePawnColor,
            };

            MyCanvas.Children.Add(currPlayerInfo);
            Canvas.SetLeft(currPlayerInfo, 630);
            Canvas.SetTop(currPlayerInfo, 405);

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
                        new Point(35, triangleFieldLen)
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
                        new Point(35, -triangleFieldLen)
                    }
                };
                MyCanvas.Children.Add(triangle);
                Canvas.SetLeft(triangle, 70 + (i < 6 ? i : i + 1) * 70);
                Canvas.SetTop(triangle, 828);
            }
        }

        private void DrawPawns()
        {
            // usuwamy poprzednio dodane piony z Canvas'a
            pawns.ForEach(MyCanvas.Children.Remove);
            pawns.Clear();

            for (int c = 0; c < 12; c++)
            {
                //góra
                BoardField fieldTop = gameState.GetBoardField(new Position(0, c));
                for (int a = 1; a <= fieldTop.amount; a++)
                {
                    var upPawn = CreatePawn(c, a, fieldTop.amount, fieldTop.player, true);
                    pawns.Add(upPawn);
                    MyCanvas.Children.Add(upPawn);
                }

                //dół
                BoardField fieldBottom = gameState.GetBoardField(new Position(1, c));
                for (int a = 1; a <= fieldBottom.amount; a++)
                {
                    var downPawn = CreatePawn(c, a, fieldBottom.amount, fieldBottom.player, false);
                    pawns.Add(downPawn);
                    MyCanvas.Children.Add(downPawn);
                }
            }
        }

        private Ellipse CreatePawn(int left, int who, int amount, Player player, bool isTop)
        {
            Ellipse pawn = new Ellipse
            {
                DataContext = new Position(isTop ? 0 : 1, left),
                Tag = "Pawn",
                Width = pawnSize,
                Height = pawnSize,
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                Cursor = Cursors.Hand,
                Fill = player == Player.red ? redPawnColor : whitePawnColor,
            };

            pawn.MouseLeftButtonDown += Pawn_MouseLeftButtonDown;
            pawn.MouseMove += Pawn_MouseMove;
            pawn.MouseLeftButtonUp += Pawn_MouseLeftButtonUp;

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
            MyCanvas.Children.Add(CreateDice(690, 385, "d1", ref firstDice));
            MyCanvas.Children.Add(CreateDice(790, 385, "d2", ref secondDice));

            int amount = (int)firstDice.DataContext;
            int amount2 = (int)secondDice.DataContext;

            if (amount == amount2) gameState.movesForCurrPlayerLeft = 4;
            else gameState.movesForCurrPlayerLeft = 2;
        }

        private Image CreateDice(double left, double top, string name, ref Image dice)
        {
            int amount = rand.Next(1, 7);

            dice = new Image
            {
                DataContext = amount,
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

        private void MarkPossibleMoves(Position choosenPawn)
        {
            int amount = (int)firstDice.DataContext;
            int amount2 = (int)secondDice.DataContext;
            var possibleMoves = gameState.GetPossibleMoves(choosenPawn, amount, amount2);

            int m = 1;
            foreach (var move in possibleMoves)
            {
                Rectangle moveMark = new Rectangle
                {
                    DataContext = move,
                    Name = possibleMoves.Count() == 1 ? "" : $"d{m}",
                    Width = pawnSize,
                    Height = triangleFieldLen,
                    Stroke = Brushes.White,
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection { 1, 1.5 }, // 1j kreska, 1j przerwa
                    Fill = Brushes.Transparent
                };

                Canvas.SetLeft(moveMark, pawnSize + (move.to.col < 6 ? move.to.col : move.to.col + 1) * pawnSize);
                Canvas.SetTop(moveMark, move.to.row == 0 ? pawnTopStart : pawnDownStart - 290);
                MyCanvas.Children.Add(moveMark);
                possibleMovesMarks.Add(moveMark);
                m++;
            }

            // ruch przypisany do zmiennej ma być ruchem o większym zasięgu (jeśli dwa to wybieramy odpowiedni)
            if (possibleMoves.Count() > 0)
            {
                if (possibleMoves.Count() == 1) currentMove = possibleMoves.First();
                else
                {
                    currentMove = gameState.GetLongestMove(choosenPawn, possibleMoves.ElementAt(0).to, possibleMoves.ElementAt(1).to);
                    currentMoveDice = currentMove.to == possibleMoves.ElementAt(0).to ? 1 : 2;
                }
            }
            else
            {
                currentMove = null;
            }
        }
        #endregion

        private bool IsDraggedPawnInsideMark(Rectangle mark)
        {
            int pl = (int)Canvas.GetLeft(draggedPawn);
            int pt = (int)Canvas.GetTop(draggedPawn);

            int ml = (int)Canvas.GetLeft(mark);
            int mt = (int)Canvas.GetTop(mark);

            if (pl + 35 > ml && pl < ml + 35
                && pt + 70 >= mt && pt <= mt + triangleFieldLen)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void NextPlayerTurn()
        {
            int amount = rand.Next(1, 7);
            int amount2 = rand.Next(1, 7);
            firstDice.DataContext = amount;
            secondDice.DataContext = amount2;
            if (amount == amount2) gameState.movesForCurrPlayerLeft = 4;
            else gameState.movesForCurrPlayerLeft = 2;

            gameState.SwitchPlayer();
            currPlayerInfo.Fill = gameState.currentPlayer == Player.red ? redPawnColor : whitePawnColor;
        }

        private void Pawn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedPawn = sender as Ellipse;
            var currPlayerColor = gameState.currentPlayer == Player.red ? redPawnColor : whitePawnColor;
            if (draggedPawn != null && draggedPawn.Fill == currPlayerColor)
            {
                isDragging = true;
                Panel.SetZIndex(draggedPawn, 1000);
                mouseOffsetForDragLogic = e.GetPosition(MyCanvas);
                mouseOffsetWhenPawnClick = e.GetPosition(MyCanvas);
                mouseOffsetForDragLogic.X -= Canvas.GetLeft(draggedPawn);
                mouseOffsetForDragLogic.Y -= Canvas.GetTop(draggedPawn);
                draggedPawn.CaptureMouse();
            }
        }

        private void Pawn_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedPawn != null)
            {
                Point position = e.GetPosition(MyCanvas);
                Canvas.SetLeft(draggedPawn, position.X - mouseOffsetForDragLogic.X);
                Canvas.SetTop(draggedPawn, position.Y - mouseOffsetForDragLogic.Y);

                // zaznaczenie pola, nad którym jest przeciągany pion (jeżeli ruch jest możliwy)
                foreach (var mark in possibleMovesMarks)
                {
                    if (IsDraggedPawnInsideMark(mark))
                    {
                        mark.StrokeDashArray = null;
                    }
                    else
                    {
                        mark.StrokeDashArray = new DoubleCollection { 1, 1.5 };
                    }
                }
            }
        }

        private void Pawn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging && draggedPawn != null)
            {
                isDragging = false;
                Panel.SetZIndex(draggedPawn, 0);
                Point position = e.GetPosition(MyCanvas);
                int amount = (int)firstDice.DataContext;
                int amount2 = (int)secondDice.DataContext;

                // samo kliknięcie
                if (mouseOffsetWhenPawnClick.X == position.X && mouseOffsetWhenPawnClick.Y == position.Y)
                {
                    Debug.Write("Nastąpiło kliknięcie na piona");
                    if (currentMove != null)
                    {
                        gameState.MakeMove(currentMove);
                        
                    }

                    //if (currentMove != null)
                    //{
                    //    gameState.MakeMove(currentMove);
                    //    if (amount != amount2)
                    //    {
                    //        if (currentMove.dice == 1) firstDice.DataContext = new DiceContext(d1.amount, false);
                    //        else secondDice.DataContext = new DiceContext(d2.amount, false);
                    //    }
                    //    if (gameState.movesForCurrPlayerLeft <= 0) NextPlayerTurn();
                    //}
                }
                // przeciągnięcie na inne pole
                else
                {
                    foreach (var mark in possibleMovesMarks)
                    {
                        if (IsDraggedPawnInsideMark(mark))
                        {
                            gameState.MakeMove((Move)mark.DataContext);
                            //Move markMove = (Move)mark.DataContext;
                            //gameState.MakeMove(markMove);
                            //if (d1.amount != d2.amount)
                            //{
                            //    if (markMove.dice == 1) firstDice.DataContext = new DiceContext(d1.amount, false);
                            //    else secondDice.DataContext = new DiceContext(d2.amount, false);
                            //}
                            //if (gameState.movesForCurrPlayerLeft <= 0) NextPlayerTurn();
                        }
                    }
                }

                draggedPawn.ReleaseMouseCapture();
                draggedPawn = null;
                DrawPawns();
            }
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