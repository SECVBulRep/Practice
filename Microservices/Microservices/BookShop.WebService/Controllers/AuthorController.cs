using AutoMapper;
using BookShop.WebService.Dtos;
using BookShop.WebService.Models;
using BookShop.WebService.Repository;
using BookShop.WebService.SyncDataServices.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.WebService.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthorController : ControllerBase
{
    private IAuthorRepository _authorRepository;
    private IMapper _mapper;
    private readonly IManagingDataClient _managingDataClient;
    private readonly ILogger<AuthorController> _logger;

    public AuthorController(ILogger<AuthorController> logger, IAuthorRepository authorRepository, IMapper mapper,
        IManagingDataClient managingDataClient)
    {
        _logger = logger;
        _authorRepository = authorRepository;
        _mapper = mapper;
        _managingDataClient = managingDataClient;
    }

    [HttpGet(Name = "Get")]
    public ActionResult<AuthorReadDto> Get(int id)
    {
        var auth = _authorRepository.GetWithInclude(x => x.Id == id, y => y.Books).SingleOrDefault();
        if (auth == null)
            return NotFound();
        var ret = _mapper.Map<AuthorReadDto>(auth);
        return Ok(ret);
    }

    [HttpGet(Name = "GetAll")]
    public ActionResult<IEnumerable<AuthorReadDto>> GetAll()
    {
        var all = _authorRepository.GetWithInclude(x => x.Books).ToList();
        var ret = _mapper.Map<List<AuthorReadDto>>(all);
        return Ok(ret);
    }

    [HttpPut(Name = "Create")]
    public async Task<ActionResult<AuthorReadDto>> Create(AuthorCreateDto authorCreateDto)
    {
        Random rd = new Random();
        var auth = BookShopFaker.GetAuthorGenerator().Generate();
        var books = BookShopFaker.GetBookGenerator().Generate(rd.Next(1, 3));
        auth.Books = books;
        _authorRepository.Create(auth);
        var ret = _mapper.Map<AuthorReadDto>(auth);

        try
        {
            await _managingDataClient.SendShopToManaging(ret);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return CreatedAtRoute(nameof(Get), new {Id = auth.Id}, auth);
    }
}