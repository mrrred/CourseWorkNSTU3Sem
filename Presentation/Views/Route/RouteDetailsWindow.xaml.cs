using System.Windows;

namespace CourseWork.Presentation.Views.Route
{
    public partial class RouteDetailsWindow : Window
    {
        public RouteDetailsWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}