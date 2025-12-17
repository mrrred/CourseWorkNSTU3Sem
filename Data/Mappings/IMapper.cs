namespace CourseWork.Data.Mappings
{
    public interface IMapper<TDomain, TDto>
    {
        TDto ToDto(TDomain domain);
        TDomain ToDomain(TDto dto);
    }
}