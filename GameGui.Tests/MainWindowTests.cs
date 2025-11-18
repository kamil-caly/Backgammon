using Backgammon;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameGui.Tests
{
    public class MainWindowTests
    {
        [WpfFact]
        public void MainWindow_InitializesCorrectly_DrawsBoardPawnsAndDice()
        {
            // arrange & act
            var window = new MainWindow();
            window.Show();

            var canvas = (Canvas)window.FindName("MyCanvas");

            // assert: board exists
            Assert.NotNull(canvas);

            // assert: triangles drawn (top + bottom = 24)
            var triangles = canvas.Children.OfType<Polygon>();
            Assert.Equal(24, triangles.Count());
            Assert.Equal(12, triangles.Where(t => t.Points.Any(p => p.X == 35 && p.Y == 360)).Count());
            Assert.Equal(12, triangles.Where(t => t.Points.Any(p => p.X == 35 && p.Y == -360)).Count());

            // assert: some pawns exist
            int pawns = canvas.Children.OfType<Ellipse>()
                                       .Count(e => (string?)e.Tag == "Pawn");
            Assert.True(pawns == 30);

            // assert: dice exist and have valid images
            var diceImages = canvas.Children.OfType<Image>()
                                            .Where(i => i.Name is "d1" or "d2")
                                            .ToList();
            Assert.Equal(2, diceImages.Count);

            foreach (var img in diceImages)
            {
                Assert.IsType<BitmapImage>(img.Source);
                Assert.InRange((int)img.DataContext, 1, 6);
            }

            window.Close();
        }
    }
}
