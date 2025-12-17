using CourseWork.Presentation.Common;
using CourseWork.Presentation.Services;

namespace CourseWork.Presentation.ViewModels.Bus
{
    public class BusDetailsViewModel : ObservableObject
    {
        private readonly IImageService _imageService;
        private BusItemViewModel _bus;

        public BusItemViewModel Bus
        {
            get => _bus;
            set => SetProperty(ref _bus, value);
        }

        public BusDetailsViewModel(IImageService imageService, BusItemViewModel bus)
        {
            _imageService = imageService ?? new ImageService();
            Bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }
    }
}