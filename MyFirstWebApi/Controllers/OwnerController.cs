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
    public class OwnerController :  Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository,IMapper mapper,ICountryRepository countryRepository)
        {
            _ownerRepository = ownerRepository;
            _mapper = mapper;
            _countryRepository = countryRepository;
        }
        
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(
                _ownerRepository.GetOwners());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
                return NotFound();

            var owner = _mapper.Map<OwnerDto>(
                _ownerRepository.GetOwner(ownerId));

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            return Ok(owner);
        }

        [HttpGet("{ownerId}/dog")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetDogByOwner(int ownerId)
        {
            if(!_ownerRepository.OwnerExists(ownerId))
                return NotFound();

            var owner = _mapper.Map<List<DogDto>>(
                _ownerRepository.GetDogByOwner(ownerId));

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            return Ok(owner);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
        {
            if(ownerCreate == null)
                return BadRequest(ModelState);
            var owners = _ownerRepository.GetOwners().Where(
                c => c.LastName.Trim().ToUpper() == ownerCreate.LastName.TrimEnd().ToUpper())
                .FirstOrDefault();
            
            if(owners != null)
            {
                ModelState.AddModelError("", "Owner already Exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var ownerMap = _mapper.Map<Owner>(ownerCreate);
            ownerMap.Country = _countryRepository.GetCountry(countryId);

            if (!_ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Added");
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updatedOwner)
        {
            if (updatedOwner == null)
                return BadRequest(ModelState);
            if (ownerId != updatedOwner.Id)
                return BadRequest(ModelState);
            if (!_ownerRepository.OwnerExists(ownerId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest();

            var ownerMap = _mapper.Map<Owner>(updatedOwner);

            if (!_ownerRepository.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something Went Wrong Updating Owner");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Updated Owner");
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
                return NotFound();
            var ownerToDelete = _ownerRepository.GetOwner(ownerId);

            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (_ownerRepository.DeleteOwner(ownerToDelete))
            {
                ModelState.AddModelError("", "Something Went Wrong while Deleting.");
            }
            return Ok("Successfully Deleted.");
        }
    }
}
