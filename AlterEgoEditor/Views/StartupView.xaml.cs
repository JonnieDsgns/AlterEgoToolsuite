using System.Windows;
using System.Windows.Controls;


namespace AlterEgoEditor.Views
{
    public partial class StartupView : UserControl
    {
        // Event to notify MainWindow
        public event EventHandler RequestNewProject;

        public StartupView()
        {
            InitializeComponent();
        }
    }
}
