using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MyFirstWebApi.Interfaces;
using MyFirstWebApi.Models;
using AutoMapper;
using System.Collections.Generic;
using MyFirstWebApi.Dto;
using MyFirstWebApi.Repository;

namespace MyFirstWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DogController:Controller
    {
        private readonly IDogRepository _dogRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public DogController(IDogRepository dogRepository,
             IReviewRepository reviewRepository, IMapper mapper)
        {
            _dogRepository = dogRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Dog>))]
        public IActionResult GetDogs()
        {
            var dogs = _mapper.Map < List < DogDto >>( _dogRepository.GetDogs());
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(dogs);
        }

        [HttpGet("{dogId}")]
        [ProducesResponseType(200, Type = typeof(Dog))]
        [ProducesResponseType(400)]
        public IActionResult GetDog(int dogId)
        {
            if(!_dogRepository.DogExists(dogId))
                return NotFound();

            var dog = _mapper.Map<DogDto>(_dogRepository.GetDog(dogId));

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            return Ok(dog);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateDog([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] DogDto dogCreate)
        {
            if (dogCreate == null)
                return BadRequest(ModelState);

            var dogs = _dogRepository.GetDogs().Where(
                c => c.Name.Trim().ToUpper() == dogCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();
            if(dogs != null)
            {
                ModelState.AddModelError("", "Dog already Exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) { return BadRequest(); }

            var dogMap = _mapper.Map<Dog>(dogCreate);
            


            if (!_dogRepository.CreateDog(ownerId,catId,dogMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Created");
        }

        [HttpPut("{dogId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateDog(int dogId, 
            [FromQuery] int ownerId, [FromQuery] int catId, 
            [FromBody] DogDto updatedDog)
        {
            if (updatedDog == null)
                return BadRequest(ModelState);
            if (dogId != updatedDog.Id)
                return BadRequest(ModelState);
            if (!_dogRepository.DogExists(dogId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest();

            var dogMap = _mapper.Map<Dog>(updatedDog);

            if (!_dogRepository.UpdateDog(ownerId,catId,dogMap))
            {
                ModelState.AddModelError("", "Something Went Wrong Updating Owner");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Updated Owner");
        }

        [HttpDelete("{dogId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteDog(int dogId)
        {
            if (!_dogRepository.DogExists(dogId))
                return NotFound();

            var reviewsToDelete = _reviewRepository.GetReviewsOfADog(dogId);

            var dogToDelete = _dogRepository.GetDog(dogId);

            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
                ModelState.AddModelError("", "Something Went Wrong while Deleting.");
            if (_dogRepository.DeleteDog(dogToDelete))
            {
                ModelState.AddModelError("", "Something Went Wrong while Deleting.");
            }
            return Ok("Successfully Deleted.");
        }
    }

}
