using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFirstWebApi.Dto;
using MyFirstWebApi.Interfaces;
using MyFirstWebApi.Models;
using MyFirstWebApi.Repository;

namespace MyFirstWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController: Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository,IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(
                _categoryRepository.GetCategories());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetDog(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var category = _mapper.Map<CategoryDto>(
                _categoryRepository.GetCategory(categoryId));

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            return Ok(category);
        }

        [HttpGet("dog/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Dog>))]
        [ProducesResponseType(400)]
        public IActionResult GetDogByCategoryId(int categoryId)
        {
            var dogs = _mapper.Map<List<DogDto>>(
                _categoryRepository.GetDogsByCategory(categoryId));
            if(!ModelState.IsValid) { return BadRequest(); }
            return Ok(dogs);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            if(categoryCreate == null)
                return BadRequest(ModelState);
            var category = _categoryRepository.GetCategories().Where(
                c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();
            if(category != null)
            {
                ModelState.AddModelError("", "Category Already Exist");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) { return BadRequest(); }
            var categoryMap = _mapper.Map<Category>(categoryCreate);

            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Created");
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategory) 
        {
            if (updatedCategory == null)
                return BadRequest(ModelState);
            if(categoryId != updatedCategory.Id) 
                return BadRequest(ModelState);
            if(!_categoryRepository.CategoryExists(categoryId))
                return NotFound();
            if(!ModelState.IsValid) 
                return BadRequest();

            var categoryMap = _mapper.Map<Category>(updatedCategory);

            if(!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something Went Wrong Updating Category");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Updated Category");
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();
            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            if(!ModelState.IsValid) return BadRequest(ModelState);
            if(_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something Went Wrong while Deleting.");
            }
            return Ok("Successfully Deleted.");
        }

    }
}
