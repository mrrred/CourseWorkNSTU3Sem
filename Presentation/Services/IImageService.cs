namespace CourseWork.Presentation.Services
{
    public interface IImageService
    {
        string CopyImageToAppData(string sourceImagePath);
        void DeleteImage(string imagePath);
        string GetPlaceholderImagePath();
        bool IsValidImageFile(string filePath);
    }
}