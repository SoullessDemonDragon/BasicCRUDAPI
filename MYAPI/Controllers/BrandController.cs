using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYAPI.Data;
using MYAPI.Models;

namespace MYAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly MyDBContext _context;

        public BrandController(MyDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands() {
            if(_context.Brands == null)
                return NotFound();
            
            return await _context.Brands.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            if(_context.Brands == null)
                return NotFound();
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                return NotFound();
            return brand;
        }

        [HttpPost]
        public async Task<ActionResult<Brand>> PostBrand(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brand);
        }

        [HttpPut]
        public async Task<IActionResult>PutBrand(int id,Brand brand)
        {
            if(id != brand.Id)
                return BadRequest();
            _context.Entry(brand).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandAvail(id))
                    return NotFound();
                else throw;
            }
            return Ok("Successful");
        }

        private bool BrandAvail(int id)
        {
            return (_context.Brands?.Any(x => x.Id == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBrand(int id) { 
            if(_context.Brands == null)
                return NotFound();
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                return NotFound();
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return Ok("Success");

        }
    }
}
