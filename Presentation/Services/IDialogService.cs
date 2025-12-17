using Microsoft.Win32;

namespace CourseWork.Presentation.Services
{
    public interface IDialogService
    {
        string ShowOpenFileDialog(string filter);
        string ShowSaveFileDialog(string filter, string defaultFileName = null);
        bool ShowConfirmationDialog(string message, string title = "Подтверждение");
        void ShowMessageDialog(string message, string title = "Информация");
        void ShowErrorDialog(string message, string title = "Ошибка");
    }
}