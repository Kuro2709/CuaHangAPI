using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuaHangAPI.Data;
using CuaHangAPI.Models;


namespace CuaHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChiTietHoaDonController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        // GET: api/ChiTietHoaDon/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ThongTinChiTietHoaDon>>> GetInvoiceDetails(string id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null)
            {
                return NotFound("Invoice not found.");
            }

            var invoiceDetails = invoice.InvoiceDetails;

            return Ok(invoiceDetails);
        }
    }
}
