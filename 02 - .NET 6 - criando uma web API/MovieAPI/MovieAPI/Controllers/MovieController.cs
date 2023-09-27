using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Data.Dtos;
using MovieAPI.Models;

namespace MovieAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly int itemsPerPage = 10;

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public MovieController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a movie
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /Movie
    ///     {
    ///        "title": "Movie title",
    ///        "genre": "Movie genre",
    ///        "duration": 120
    ///     }
    ///
    /// </remarks>
    /// <param name="movieDto"></param>
    /// <returns>A newly created movie</returns>
    /// <response code="201">Returns the newly created movie</response>
    /// <response code="400">If the item is null</response>
    [HttpPost, ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult<Movie> Create([FromBody] CreateMovieDto movieDto)
    {
        Movie movie = _mapper.Map<Movie>(movieDto); 

        _context.Movies.Add(movie);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetOne), new { id = movie.Id }, movie);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Movie>> Get([FromQuery] int page = 1, [FromQuery] string? cineName = null)
    {
        var movies = _context.Movies.AsQueryable();

        if (cineName != null)
        {
            movies = movies.Where(movie => movie.Sessions.Any(session => session.Cine.Name.Contains(cineName)));
        }

        return Ok(movies.Skip(--page * itemsPerPage).Take(itemsPerPage));
    }

    [HttpGet("{id}")]
    public ActionResult<Movie> GetOne(int id)
    {
        var movie = _context.Movies.FirstOrDefault(movie => movie.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    [HttpPatch("{id}")]
    public ActionResult<Movie> UpdatePatch(int id, [FromBody] JsonPatchDocument<UpdateMovieDto> patchDocument)
    {
        var movie = _context.Movies.FirstOrDefault(movie => movie.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        var movieDto = _mapper.Map<UpdateMovieDto>(movie);

        patchDocument.ApplyTo(movieDto, ModelState);

        if (!TryValidateModel(movieDto))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(movieDto, movie);

        _context.SaveChanges();

        return NoContent();
    }
}