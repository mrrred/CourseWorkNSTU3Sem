using CourseWork.Presentation.Common;

namespace CourseWork.Presentation.ViewModels.Route
{
    public class RouteDetailsViewModel : ObservableObject
    {
        private RouteItemViewModel _route;

        public RouteItemViewModel Route
        {
            get => _route;
            set => SetProperty(ref _route, value);
        }

        public RouteDetailsViewModel(RouteItemViewModel route)
        {
            Route = route ?? throw new System.ArgumentNullException(nameof(route));
        }
    }
}