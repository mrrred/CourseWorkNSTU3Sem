using CourseWork.Presentation.Common;

namespace CourseWork.Presentation.ViewModels
{
    public class PlaceholderViewModel : ObservableObject
    {
        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public PlaceholderViewModel(string title)
        {
            Title = title;
        }
    }
}