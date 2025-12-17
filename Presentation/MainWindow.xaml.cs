using System.Windows;

namespace CourseWork.Presentation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // DataContext будет установлен в App.xaml.cs после создания окна
            // Не устанавливаем его здесь, чтобы избежать ошибки при вызове InitializeComponent()
        }
    }
}