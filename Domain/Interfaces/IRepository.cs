using System.Collections.Generic;

namespace CourseWork.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Add(T item);
        void Update(T item);
        void Remove(T item);
        T GetById(object id);
        IEnumerable<T> GetAll();
        bool Exists(object id);
    }
}