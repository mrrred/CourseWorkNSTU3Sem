using CourseWork.Presentation.Common;

namespace CourseWork.Presentation.ViewModels.Driver
{
    public class DriverDetailsViewModel : ObservableObject
    {
        private DriverItemViewModel _driver;

        public DriverItemViewModel Driver
        {
            get => _driver;
            set => SetProperty(ref _driver, value);
        }

        public DriverDetailsViewModel(DriverItemViewModel driver)
        {
            Driver = driver ?? throw new System.ArgumentNullException(nameof(driver));
        }
    }
}