using System.Windows;

namespace CourseWork.Presentation.Views.Bus
{
    public partial class BusDetailsWindow : Window
    {
        public BusDetailsWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}