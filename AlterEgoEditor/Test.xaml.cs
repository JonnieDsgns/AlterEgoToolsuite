using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

using AlterEgoEditor.Views;

namespace AlterEgoEditor
{
    public partial class Test : Window
    {
        public Test()
        {
            InitializeComponent();
            ShowStartView();
        }

        private void ShowStartView()
        {
            var startView = new StartView();
            // Subscribe to the event from StartView
            startView.RequestNewProject += (s, e) => OpenNewProjectDialog();
            MainViewArea.Content = startView;
        }

        private void ShowEditorView()
        {
            MainViewArea.Content = new EditorView();
        }

        private void OpenNewProjectDialog()
        {
            var newProjectWindow = new NewProjectModal();
            newProjectWindow.Owner = this;

            // If user clicks "Create Mod" (DialogResult == true)
            if (newProjectWindow.ShowDialog() == true)
            {
                ShowEditorView();
            }
        }

        // Menu Event Handlers
        private void MenuNewProject_Click(object sender, RoutedEventArgs e)
        {
            OpenNewProjectDialog();
        }

        private void MenuCloseProject_Click(object sender, RoutedEventArgs e)
        {
            ShowStartView();
        }

        // Existing Window Logic
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            var chrome = WindowChrome.GetWindowChrome(this);
            if (chrome != null)
            {
                chrome.CornerRadius = WindowState == WindowState.Maximized
                    ? new CornerRadius(0)
                    : new CornerRadius(20);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void btnMaximize_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }
}