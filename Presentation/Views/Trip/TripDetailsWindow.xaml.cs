using System.Windows;

namespace CourseWork.Presentation.Views.Trip
{
    public partial class TripDetailsWindow : Window
    {
        public TripDetailsWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}