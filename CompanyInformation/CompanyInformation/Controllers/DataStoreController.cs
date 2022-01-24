#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyInformation;
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

        // POST: api/DataStore
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Company>> Post(IFormFile csvContent)
        {
            using(var stream = new MemoryStream())
            {
                await csvContent.CopyToAsync(stream);
                _context.Import(stream);
            }
            return CreatedAtAction("GetCompanies", new { id = "" }, "");
        }
    }
}
