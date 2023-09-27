// Ignore Spelling: Dto

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data;
using MovieAPI.Data.Dtos;
using MovieAPI.Models;

namespace MovieAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AddressController : ControllerBase
{
    private readonly int itemsPerPage = 10;

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AddressController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost, ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult<Address> Create([FromBody] CreateAddressDto addressDto)
    {
        Address address = _mapper.Map<Address>(addressDto); 

        _context.Addresses.Add(address);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetOne), new { id = address.Id }, address);
    }

    [HttpGet]
    public IEnumerable<Address> Get([FromQuery] int page = 1) 
    {

        return _context.Addresses.Skip(--page * itemsPerPage).Take(itemsPerPage);
    }

    [HttpGet("{id}")]
    public ActionResult<Address> GetOne(int id)
    {
        var address = _context.Addresses.FirstOrDefault(address => address.Id == id);

        if (address == null)
        {
            return NotFound();
        }

        return Ok(address);
    }

    [HttpPut("{id}")]
    public ActionResult<Address> Update(int id, [FromBody] UpdateAddressDto addressDto)
    {
        var address = _context.Addresses.FirstOrDefault(address => address.Id == id);

        if (address == null)
        {
            return NotFound();
        }

        _mapper.Map(addressDto, address);

        _context.SaveChanges();

        return NoContent();
    }
}