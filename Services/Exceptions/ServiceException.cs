namespace CourseWork.Services.Exceptions
{
    public class ServiceException : Exception
    {
        public ServiceException() { }

        public ServiceException(string message) : base(message) { }

        public ServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class ValidationException : ServiceException
    {
        public ValidationException(string message) : base(message) { }
    }

    public class BusinessRuleException : ServiceException
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}