using System.Windows;
using System.Windows.Input;

using AlterEgoEditor.ViewModels;

namespace AlterEgoEditor.Views
{
    public partial class CreateProjectWindow : Window
    {
        public CreateProjectWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        // We use this method to pass the ViewModel to the Window *after* creation
        public CreateProjectWindow(CreateProjectViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            // Connect the ViewModel's action hook to the window's close method.
            vm.CloseWindow = Close;
        }
    }
}