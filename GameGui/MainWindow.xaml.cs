using GameGui;
using GameLogic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

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
        private const int pawnInCourtWidth = 60;
        private const int pawnInCourtHeight = 24;
        private const int maxPawnLenInCol = 350;
        private const int triangleFieldLen = 360;
        private const int pawnTopStart = 12;
        private const int pawnDownStart = 758;
        private const int noMoveBreakDelay = 3000;

        GameState gameState;
        private Image firstDice = default!;
        private Image secondDice = default!;
        private Ellipse currPlayerInfo;
        private List<Rectangle> possibleMovesMarks;
        private List<Ellipse> pawns;
        private List<Ellipse> beatenPawns;
        private List<Rectangle> courtPawns;
        private Move? currentMove = null;
        private Ellipse? draggedPawn;
        private Image sadFace = default!;

        private bool isDragging = false;
        private Position? capturingPawn = null;
        private bool firstDiceUsed = false;
        private bool secondDiceUsed = false;
        private int movesForCurrPlayerLeft = 0;
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
            beatenPawns = new List<Ellipse>();
            courtPawns = new List<Rectangle>();
            DrawBoard();
            DrawPawns();
            DrawDice();
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
            beatenPawns.ForEach(MyCanvas.Children.Remove);
            beatenPawns.Clear();

            // plansza
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

            // zbite piony
            CreateBeatenPawns(Player.red);
            CreateBeatenPawns(Player.white);

            // wyprowadzone na dwór piony
            DrawCourtPawns();
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
                if (isTop) top = pawnTopStart + top + (int)Math.Round((who - 1) * space);
                else top = pawnDownStart - (int)Math.Round((who - 1) * space);
            }

            Canvas.SetLeft(pawn, pawnSize + (left < 6 ? left : left + 1) * pawnSize);
            Canvas.SetTop(pawn, top);

            return pawn;
        }

        private void CreateBeatenPawns(Player player)
        {
            int amount = gameState.beatenPawns[player];
            for (int i = 1; i <= amount; i++)
            {
                Ellipse bPawn = new Ellipse
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
                    if (player == Player.red) top = pawnTopStart + (i - 1) * pawnSize;
                    else top = pawnDownStart - (i - 1) * pawnSize;
                }
                else
                {
                    double space = (double)(maxPawnLenInCol - pawnSize) / (double)(amount - 1);
                    if (player == Player.red) top = pawnTopStart + top + (int)Math.Round((i - 1) * space);
                    else top = pawnDownStart - (int)Math.Round((i - 1) * space);
                }

                bPawn.MouseLeftButtonDown += Pawn_MouseLeftButtonDown;
                bPawn.MouseMove += Pawn_MouseMove;
                bPawn.MouseLeftButtonUp += Pawn_MouseLeftButtonUp;

                Canvas.SetLeft(bPawn, 7 * pawnSize);
                Canvas.SetTop(bPawn, top);
                MyCanvas.Children.Add(bPawn);
                beatenPawns.Add(bPawn);
            }
        }

        private void DrawCourtPawns()
        {
            courtPawns.ForEach(MyCanvas.Children.Remove);
            courtPawns.Clear();

            foreach (var player in new Player[] {Player.white, Player.red})
            {
                for (int i = 0; i < gameState.courtPawns[player]; i++)
                {
                    Rectangle pawn = new Rectangle
                    {
                        Width = pawnInCourtWidth,
                        Height = pawnInCourtHeight,
                        Fill = player == Player.white ? whitePawnColor : redPawnColor,
                        Stroke = Brushes.Black,
                        StrokeThickness = 3
                    };

                    int top = 0;
                    if (player == Player.white) top = pawnTopStart + i * pawnInCourtHeight + 1 * i;
                    else top = pawnDownStart + pawnSize - pawnInCourtHeight - i * pawnInCourtHeight - 1 * i;

                    Canvas.SetLeft(pawn, 5);
                    Canvas.SetTop(pawn, top);
                    MyCanvas.Children.Add(pawn);
                    courtPawns.Add(pawn);
                }
            }
        }

        private void DrawDice()
        {
            MyCanvas.Children.Add(CreateDice(690, 385, "d1", ref firstDice));
            MyCanvas.Children.Add(CreateDice(790, 385, "d2", ref secondDice));

            int amount = (int)firstDice.DataContext;
            int amount2 = (int)secondDice.DataContext;

            if (amount == amount2) movesForCurrPlayerLeft = 4;
            else movesForCurrPlayerLeft = 2;
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
                Source = GetDiceBitmapImg(amount)
            };

            dice.RenderTransform = new RotateTransform(15, dice.Width / 2, dice.Height / 2);

            Canvas.SetLeft(dice, left);
            Canvas.SetTop(dice, top);

            return dice;
        }

        private void MarkPossibleMoves(Position? choosenPawn)
        {
            possibleMovesMarks.ForEach(MyCanvas.Children.Remove);
            possibleMovesMarks.Clear();

            int amount = -1;
            int amount2 = -1;
            if ((int)firstDice.DataContext == (int)secondDice.DataContext) amount = amount2 = (int)firstDice.DataContext;
            else
            {
                if (!firstDiceUsed) amount = (int)firstDice.DataContext;
                if (!secondDiceUsed) amount2 = (int)secondDice.DataContext;
            }

            IEnumerable<Move> possibleMoves;
            if (choosenPawn != null)
            {
                possibleMoves = gameState.GetPossibleMoves(choosenPawn, amount, amount2);
            }
            else
            {
                possibleMoves = gameState.GetPossibleBackOnBoardMoves(amount, amount2);
            }

            foreach (var move in possibleMoves)
            {
                Rectangle moveMark = new Rectangle
                {
                    DataContext = move,
                    Name = $"d{move.dice}",
                    Width = pawnSize,
                    Height = triangleFieldLen,
                    Stroke = Brushes.White,
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection { 1, 1.5 }, // 1j kreska, 1j przerwa
                    Fill = Brushes.Transparent,
                    Cursor = Cursors.Hand
                };

                Canvas.SetLeft(moveMark, pawnSize + (move.to.col < 6 ? move.to.col : move.to.col + 1) * pawnSize);
                Canvas.SetTop(moveMark, move.to.row == 0 ? pawnTopStart : pawnDownStart - 290);
                MyCanvas.Children.Add(moveMark);
                possibleMovesMarks.Add(moveMark);
            }

            // ruch przypisany do zmiennej ma być ruchem o większym zasięgu (jeśli dwa to wybieramy odpowiedni)
            if (possibleMoves.Count() > 0)
            {
                if (possibleMoves.Count() == 1) currentMove = possibleMoves.First();
                else
                {
                    currentMove = gameState.GetLongestMove(choosenPawn, possibleMoves.ElementAt(0).to, possibleMoves.ElementAt(1).to);
                    currentMove.SetDice(currentMove.to == possibleMoves.ElementAt(0).to ? 1 : 2);
                }
            }
            else
            {
                currentMove = null;
            }
        }

        private void DrawSadFace()
        {
            sadFace = new Image
            {
                Width = 55,
                Height = 55,
                Source = GetFaceBitmapImg()
            };

            Canvas.SetLeft(sadFace, 890);
            Canvas.SetTop(sadFace, 400);
            MyCanvas.Children.Add(sadFace);
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

        private async Task NextPlayerTurn()
        {
            int amount = rand.Next(1, 7);
            int amount2 = rand.Next(1, 7);
            firstDice.DataContext = amount;
            firstDice.Source = GetDiceBitmapImg(amount);
            secondDice.DataContext = amount2;
            secondDice.Source = GetDiceBitmapImg(amount2);
            if (amount == amount2) movesForCurrPlayerLeft = 4;
            else movesForCurrPlayerLeft = 2;

            gameState.SwitchPlayer();
            currPlayerInfo.Fill = gameState.currentPlayer == Player.red ? redPawnColor : whitePawnColor;
            firstDiceUsed = false;
            secondDiceUsed = false;

            if (!gameState.IsAnyMove(amount, amount2))
            {
                await NextPlayerTurnWhenNoMoveLogic();
                return;
            }

            // od razu rysujemy możliwe pola do wyprowadzenia zbitych pionów (jeśli są)
            if (gameState.beatenPawns[gameState.currentPlayer] > 0)
            {
                MarkPossibleMoves(null);
            }
        }

        private async Task NextPlayerTurnWhenNoMoveLogic()
        {
            DrawSadFace();
            MyCanvas.IsHitTestVisible = false;
            possibleMovesMarks.ForEach(MyCanvas.Children.Remove);
            possibleMovesMarks.Clear();
            await Task.Delay(noMoveBreakDelay);
            MyCanvas.IsHitTestVisible = true;
            MyCanvas.Children.Remove(sadFace);
            await NextPlayerTurn();
        }

        private void RestartGame()
        {
            gameState = new GameState();

            int amount = rand.Next(1, 7);
            int amount2 = rand.Next(1, 7);
            firstDice.DataContext = amount;
            firstDice.Source = GetDiceBitmapImg(amount);
            secondDice.DataContext = amount2;
            secondDice.Source = GetDiceBitmapImg(amount2);
            if (amount == amount2) movesForCurrPlayerLeft = 4;
            else movesForCurrPlayerLeft = 2;

            currPlayerInfo.Fill = gameState.currentPlayer == Player.red ? redPawnColor : whitePawnColor;
            MyCanvas.Children.Remove(sadFace);
            firstDiceUsed = false;
            secondDiceUsed = false;
            DrawPawns();
        }

        #region Events
        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // jeżeli zbite piony to najpierw trzeba je wprowadzić na planszę
            if (gameState.beatenPawns[gameState.currentPlayer] > 0) return;

            var hovered = e.OriginalSource as Ellipse;
            if (hovered != null
                && (string)hovered.Tag == "Pawn"
                && hovered.Fill == (gameState.currentPlayer == Player.red ? redPawnColor : whitePawnColor)
            )
            {
                Position hoveredLeft = (Position)hovered.DataContext;
                if (capturingPawn == null || hoveredLeft.col != capturingPawn.col)
                {
                    capturingPawn = hoveredLeft;
                    Debug.WriteLine($"Najechano na pionek {rand.Next(1000)}");
                    MarkPossibleMoves((Position)hovered.DataContext);
                }
            }
            else
            {
                capturingPawn = null;
                possibleMovesMarks.ForEach(MyCanvas.Children.Remove);
                possibleMovesMarks.Clear();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (Menu.Content != null)
                {
                    Menu.Content = null;
                    MyCanvas.IsHitTestVisible = true;
                }
                else
                {
                    PauseMenu pauseMenu = new PauseMenu();
                    Menu.Content = pauseMenu;
                    MyCanvas.IsHitTestVisible = false;

                    pauseMenu.ClickedAction += option =>
                    {
                        if (option == PauseAction.Continue) Menu.Content = null;
                        else
                        {
                            RestartGame();
                            Menu.Content = null;
                        }

                        MyCanvas.IsHitTestVisible = true;
                    };
                }
            }
        }

        private void Pawn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedPawn = sender as Ellipse;
            // jeżeli zbite piony -> wychodzimy z metody gdy kliknięty inny pion niż zbity
            if (gameState.beatenPawns[gameState.currentPlayer] > 0 && Canvas.GetLeft(draggedPawn) != 7 * pawnSize)
            {
                return;
            }

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

        private async void Pawn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
                        movesForCurrPlayerLeft--;
                        if (currentMove.dice != 0 && amount != amount2)
                        {
                            if (currentMove.dice == 1) firstDiceUsed = true;
                            else secondDiceUsed = true;
                        }
                    }
                }
                // przeciągnięcie na inne pole
                else
                {
                    foreach (var mark in possibleMovesMarks)
                    {
                        if (IsDraggedPawnInsideMark(mark))
                        {
                            gameState.MakeMove((Move)mark.DataContext);
                            movesForCurrPlayerLeft--;
                            if (amount == amount2) continue;

                            if (mark.Name.Contains("1")) firstDiceUsed = true;
                            else if (mark.Name.Contains("2")) secondDiceUsed = true;
                        }
                    }
                }

                draggedPawn.ReleaseMouseCapture();
                draggedPawn = null;
                DrawPawns();

                if (movesForCurrPlayerLeft <= 0)
                {
                    await NextPlayerTurn();
                    return;
                }

                bool sameAmount = amount == amount2;
                if (sameAmount 
                    ? !gameState.IsAnyMove(amount, amount2) 
                    : !gameState.IsAnyMove(firstDiceUsed ? -1 : amount, secondDiceUsed ? -1 : amount2)
                )
                {
                    await NextPlayerTurnWhenNoMoveLogic();
                }
            }
        }
        #endregion

        private BitmapImage GetDiceBitmapImg(int amount)
        {
            string path = "";
            switch (amount)
            {
                case 1: path = "dice-one.png"; break;
                case 2: path = "dice-two.png"; break;
                case 3: path = "dice-three.png"; break;
                case 4: path = "dice-four.png"; break;
                case 5: path = "dice-five.png"; break;
                case 6: path = "dice-six.png"; break;
                default: break;
            }

            return new BitmapImage(new Uri(assetsPath + path, UriKind.Relative));
        }

        private BitmapImage GetFaceBitmapImg()
        {
            string path = gameState.currentPlayer == Player.white ? "sad_red.png" : "sad_white.png";
            return new BitmapImage(new Uri(assetsPath + path, UriKind.Relative));
        }
    }
}