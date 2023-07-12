using MyFirstWebApi.Models;

namespace MyFirstWebApi.Interfaces
{
    public interface IDogRepository
    {
        ICollection<Dog> GetDogs();
        Dog GetDog(int id);
        Dog GetDog(string name);
        bool DogExists(int dogid);
        bool CreateDog(int ownerId, int categoryId, Dog dog);
        bool UpdateDog(int ownerId, int categoryId, Dog dog);
        bool DeleteDog(Dog dog);
        bool Save();
    }
}
