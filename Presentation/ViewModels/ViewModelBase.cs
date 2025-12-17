using CourseWork.Presentation.Common;
using System.Runtime.CompilerServices;

namespace CourseWork.Presentation.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            return base.SetProperty(ref field, value, propertyName);
        }
    }
}