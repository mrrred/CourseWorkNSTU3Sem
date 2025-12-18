using System.Windows;

namespace CourseWork.Presentation.Views.Driver
{
    public partial class DriverDetailsWindow : Window
    {
        public DriverDetailsWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}