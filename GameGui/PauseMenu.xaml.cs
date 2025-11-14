using System.Windows;
using System.Windows.Controls;

namespace GameGui
{
    public partial class PauseMenu : UserControl
    {
        public event Action<PauseAction>? ClickedAction;
        public PauseMenu()
        {
            InitializeComponent();
        }

        private void RestartBtnOnClick(object sender, RoutedEventArgs e)
        {
            ClickedAction?.Invoke(PauseAction.Restart);
        }

        private void ContinueBtnOnClick(object sender, RoutedEventArgs e)
        {
            ClickedAction?.Invoke(PauseAction.Continue);
        }
    }
}
