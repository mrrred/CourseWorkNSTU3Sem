using CourseWork.Presentation.Common;

namespace CourseWork.Presentation.ViewModels.Trip
{
    public class TripDetailsViewModel : ObservableObject
    {
        private TripItemViewModel _trip;

        public TripItemViewModel Trip
        {
            get => _trip;
            set => SetProperty(ref _trip, value);
        }

        public TripDetailsViewModel(TripItemViewModel trip)
        {
            Trip = trip ?? throw new System.ArgumentNullException(nameof(trip));
        }
    }
}