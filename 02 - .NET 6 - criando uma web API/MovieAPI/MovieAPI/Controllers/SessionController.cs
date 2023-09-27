using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data;
using MovieAPI.Data.Dtos;
using MovieAPI.Models;

namespace MovieAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
    private readonly int itemsPerPage = 10;

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SessionController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost, ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult<Session> Create([FromBody] CreateSessionDto sessionDto)
    {
        Session session = _mapper.Map<Session>(sessionDto); 

        _context.Sessions.Add(session);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetOne), new { cineId = session.CineId, movieId = session.MovieId }, session);
    }

    [HttpGet]
    public IEnumerable<Session> Get([FromQuery] int page = 1) 
    {

        return _context.Sessions.Skip(--page * itemsPerPage).Take(itemsPerPage);
    }

    [HttpGet("{cineId}/{movieId}")]
    public ActionResult<Session> GetOne(int cineId, int movieId)
    {
        var session = _context.Sessions.FirstOrDefault(session => session.CineId == cineId && session.MovieId == movieId);

        if (session == null)
        {
            return NotFound();
        }

        return Ok(session);
    }
}