using CourseWork.Domain.Interfaces;
using CourseWork.Data.Exceptions;
using CourseWork.Services.Exceptions;

namespace CourseWork.Services.Services
{
    public abstract class BaseService
    {
        protected T HandleRepositoryOperation<T>(Func<T> operation, string errorMessage)
        {
            try
            {
                return operation();
            }
            catch (DataException ex)
            {
                throw new ServiceException(errorMessage, ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Произошла непредвиденная ошибка: {ex.Message}", ex);
            }
        }

        protected void HandleRepositoryOperation(Action operation, string errorMessage)
        {
            try
            {
                operation();
            }
            catch (DataException ex)
            {
                throw new ServiceException(errorMessage, ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Произошла непредвиденная ошибка: {ex.Message}", ex);
            }
        }
    }
}