#nullable disable
using Microsoft.AspNetCore.Mvc;
using CompanyInformation.Data;

namespace CompanyInformation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataStoreController : ControllerBase
    {
        private readonly CompanyInformationContext _context;

        public DataStoreController(CompanyInformationContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(IFormFile csvContent)
        {
            using(var stream = new MemoryStream())
            {
                await csvContent.CopyToAsync(stream);
                try
                {
                    _context.ImportCsv(stream);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return Accepted();
        }
    }
}
