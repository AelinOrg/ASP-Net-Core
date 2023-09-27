using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Data.Dtos;
using MovieAPI.Models;

namespace MovieAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CineController : ControllerBase
{
    private readonly int itemsPerPage = 10;

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CineController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost, ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult<Cine> Create([FromBody] CreateCineDto cineDto)
    {
        Cine cine = _mapper.Map<Cine>(cineDto); 

        _context.Cines.Add(cine);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetOne), new { id = cine.Id }, cine);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Cine>> Get([FromQuery] string? addressId = null)
    {

        if (addressId != null)
        {
            return Ok(_context.Cines.FromSqlRaw("SELECT * FROM Cines WHERE AddressId = {0}", addressId));
        }
        else
        {
            return Ok(_context.Cines.FromSqlRaw("SELECT * FROM Cines"));
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Cine> GetOne(int id)
    {
        var cine = _context.Cines.FirstOrDefault(cine => cine.Id == id);

        if (cine == null)
        {
            return NotFound();
        }

        return Ok(cine);
    }

    [HttpPut("{id}")]
    public ActionResult<Cine> Update(int id, [FromBody] UpdateCineDto cineDto)
    {
        var cine = _context.Cines.FirstOrDefault(cine => cine.Id == id);

        if (cine == null)
        {
            return NotFound();
        }

        _mapper.Map(cineDto, cine);

        _context.SaveChanges();

        return NoContent();
    }
}