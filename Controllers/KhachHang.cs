using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuaHangAPI.Data;
using CuaHangAPI.Dtos;
using CuaHangAPI.Models;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using CuaHangAPI.Helpers;


namespace CuaHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public partial class KhachHangController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public KhachHangController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET: api/KhachHang
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHangDto>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            var customerDtos = customers.Select(c => new KhachHangDto
            {
                CustomerID = c.CustomerID,
                CustomerName = c.CustomerName,
                Phone = c.Phone
            }).ToList();

            return Ok(customerDtos);
        }

        // GET: api/KhachHang/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<KhachHangDto>> GetCustomerById(string id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var customerDto = new KhachHangDto
            {
                CustomerID = customer.CustomerID,
                CustomerName = customer.CustomerName,
                Phone = customer.Phone
            };

            return Ok(customerDto);
        }

        [GeneratedRegex(@"^\d+$")]
        private partial Regex PhoneRegex();

        // POST: api/KhachHang
        [HttpPost]
        public async Task<ActionResult<KhachHangDto>> AddCustomer(ThemKhachHang customerDto)
        {
            if (string.IsNullOrEmpty(customerDto.CustomerID) || string.IsNullOrEmpty(customerDto.CustomerName) || string.IsNullOrEmpty(customerDto.Phone))
            {
                return BadRequest("Tất cả các trường đều bắt buộc.");
            }

            customerDto.CustomerID = customerDto.CustomerID.Trim().ToUpper();

            if (customerDto.CustomerID.Contains(' '))
            {
                return BadRequest("Mã khách hàng không được chứa khoảng trắng.");
            }

            if (!PhoneRegex().IsMatch(customerDto.Phone))
            {
                return BadRequest("Số điện thoại chỉ được chứa số và không có khoảng trắng.");
            }

            var customer = new ThongTinKhachHang
            {
                CustomerID = customerDto.CustomerID,
                CustomerName = customerDto.CustomerName,
                Phone = customerDto.Phone
            };

            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                return Conflict("Mã khách hàng đã tồn tại. Vui lòng nhập mã mới.");
            }

            return CreatedAtAction(nameof(GetCustomers), new { id = customer.CustomerID }, customerDto);
        }

        // PUT: api/KhachHang/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomer(string id, [FromBody] ChinhSuaKhachHang customerDto)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound("Không tìm thấy khách hàng.");
            }

            if (string.IsNullOrEmpty(customerDto.CustomerName) || string.IsNullOrEmpty(customerDto.Phone))
            {
                return BadRequest("Tất cả các trường đều bắt buộc.");
            }

            customer.CustomerName = customerDto.CustomerName;
            customer.Phone = customerDto.Phone;

            try
            {
                _context.Entry(customer).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(customer); // Return the updated product
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Customers.Any(e => e.CustomerID == id))
                {
                    return NotFound("Không tìm thấy khách hàng.");
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/KhachHang/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            if (CustomerHasInvoices(id))
            {
                return BadRequest("Không thể xóa khách hàng. Khách hàng này có liên quan đến một hoặc nhiều hóa đơn.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerHasInvoices(string customerId)
        {
            return _context.Invoices.Any(i => i.CustomerID == customerId);
        }
    }
}
