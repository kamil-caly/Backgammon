using GameLogic;
using System.Windows;
using System.Windows.Controls;

namespace GameGui
{
    public partial class GameOverMenu : UserControl
    {
        public event Action<GameOverAction>? ClickedAction;
        public GameOverMenu(Player winner)
        {
            InitializeComponent();
            ResultLabel.Content = GetResultText(winner);
        }

        private string GetResultText(Player winner)
        {
            return $"{(winner == Player.red ? "Red" : "White")} Won!";
        }

        private void RestartBtnOnClick(object sender, RoutedEventArgs e)
        {
            ClickedAction?.Invoke(GameOverAction.Restart);
        }

        private void QuitBtnOnClick(object sender, RoutedEventArgs e)
        {
            ClickedAction?.Invoke(GameOverAction.Quit);
        }
    }
}
