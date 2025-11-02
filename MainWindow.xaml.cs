using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        public MainWindow()
        {
            InitializeComponent();
            DrawBoard();
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
                        new Point(35, 370)
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
                        new Point(35, -370)
                    }
                };
                MyCanvas.Children.Add(triangle);
                Canvas.SetLeft(triangle, 70 + (i < 6 ? i : i + 1) * 70);
                Canvas.SetTop(triangle, 828);
            }
        }
    }
}