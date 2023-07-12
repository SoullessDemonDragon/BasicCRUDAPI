using MyFirstWebApi.Data;
using MyFirstWebApi.Interfaces;
using MyFirstWebApi.Models;

namespace MyFirstWebApi.Repository
{
    public class DogRepository : IDogRepository
    {
        private readonly DataContext _context;
        public DogRepository(DataContext context) 
        {
            _context = context;
        }

        public bool CreateDog(int ownerId, int categoryId, Dog dog)
        {
            var dogOwnerEntity = _context.Owners.Where(
                a => a.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(
                a => a.Id == categoryId).FirstOrDefault();
            var dogOwner = new DogOwner()
            {
                Owner = dogOwnerEntity,
                Dog = dog,
            };
            _context.Add(dogOwner);

            var dogCategory = new DogCategory()
            {
                Category = category,
                Dog = dog,

            };
            _context.Add(dogCategory);

            _context.Add(dog);

            return Save();
        }

        public bool DeleteDog(Dog dog)
        {
            _context.Remove(dog);
            return Save();
        }

        public bool DogExists(int dogid)
        {
            return _context.Dogs.Any(p=>p.Id == dogid);
        }

        public Dog GetDog(int id)
        {
            return _context.Dogs.Where(p => p.Id == id).FirstOrDefault(); ;
        }

        public Dog GetDog(string name)
        {
            return _context.Dogs.Where(p => p.Name == name).FirstOrDefault();
        }

        public ICollection<Dog> GetDogs()
        {
            return _context.Dogs.OrderBy(p=>p.Id).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0? true: false;
        }

        public bool UpdateDog(int ownerId, int categoryId, Dog dog)
        {
            _context.Update(dog);
            return Save();
        }
    }
}
