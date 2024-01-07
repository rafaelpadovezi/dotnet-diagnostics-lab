using Asp.Versioning;
using AspnetTemplate.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspnetTemplate.Controllers.v1;

[ApiVersion("1")]
[ApiController]
[Route("v{version:apiVersion}/[controller]")]
public class DiagnosticsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DiagnosticsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("sync")]
    public IActionResult GetSync()
    {
        _context.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:00.500'");
        return Ok();
    }

    [HttpGet("async")]
    public async Task<IActionResult> GetAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:00.500'");
        return Ok();
    }

    [HttpGet("enumeration")]
    public IActionResult GetMultipleEnumerations([FromQuery] bool multiple = false)
    {
        var enumerable = Enumerable.Range(0, 10000).Select(x => new SomeClass
        {
            Id = x,
            Name = $"Name {x}"
        });
        enumerable = enumerable.Where(x => x.Id % 10 == 0);

        if (multiple)
        {
            return Ok(GetValues(enumerable));
        }

        var list = enumerable.ToList();

        return Ok(GetValues(list));
    }

    private List<int> GetValues(IEnumerable<SomeClass> enumerable)
    {
        var returnValues = new List<int>();
        foreach (var i in Enumerable.Range(1, 1000))
        {
            var value = enumerable.SingleOrDefault(y => y.Id == i);
            if (value is not null)
                returnValues.Add(value.Id);
        }

        return returnValues;
    }
}

class SomeClass
{
    public int Id { get; set; }
    public string Name { get; set; }
}