using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuaHangAPI.Data;
using CuaHangAPI.Dtos;
using CuaHangAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace CuaHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    [Authorize] // Apply authorization to the entire controller
    public class SanPhamController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        // GET: api/SanPham
        [HttpGet]
        [AllowAnonymous] // Allow anonymous access to this endpoint
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            var productDtos = products.Select(p => new SanPhamDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Price = Math.Round(p.Price, 2)
            }).ToList();

            return Ok(productDtos);
        }

        // GET: api/SanPham/{id}
        [HttpGet("{id}")]
       
        public async Task<ActionResult<SanPhamDto>> GetProduct(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            var productDto = new SanPhamDto
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Price = Math.Round(product.Price, 2)
            };

            return Ok(productDto);
        }

        // PUT: api/SanPham/{id}
        [HttpPut("{id}")]
        
        public async Task<IActionResult> EditProduct(string id, [FromBody] ChinhSuaSanPhamDto productInfo)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            if (string.IsNullOrEmpty(productInfo.ProductName) || productInfo.Price <= 0)
            {
                return BadRequest("Tất cả các trường đều bắt buộc và phải hợp lệ.");
            }

            product.ProductName = productInfo.ProductName;
            product.Price = productInfo.Price;

            try
            {
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(product); // Return the updated product
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.ProductID == id))
                {
                    return NotFound("Không tìm thấy sản phẩm.");
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/SanPham
        [HttpPost]
        
        public async Task<IActionResult> AddProduct([FromBody] AddProductDto productInfo)
        {
            if (productInfo == null)
            {
                return BadRequest("Thông tin sản phẩm là bắt buộc.");
            }

            // Trim spaces and convert ProductID to uppercase
            productInfo.ProductID = productInfo.ProductID?.Trim().ToUpper() ?? string.Empty;

            if (string.IsNullOrEmpty(productInfo.ProductID) || string.IsNullOrEmpty(productInfo.ProductName) || productInfo.Price <= 0)
            {
                return BadRequest("Tất cả các trường đều bắt buộc và phải hợp lệ.");
            }

            if (productInfo.ProductID.Contains(' '))
            {
                return BadRequest("ID sản phẩm không được chứa khoảng trắng.");
            }

            // Ensure the Price value has two decimal places
            productInfo.Price = Math.Round(productInfo.Price, 2);

            var product = new ThongTinSanPham
            {
                ProductID = productInfo.ProductID,
                ProductName = productInfo.ProductName,
                Price = productInfo.Price
            };

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductID }, product);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627) // SQL error code for primary key violation
            {
                return Conflict("ID sản phẩm đã tồn tại. Vui lòng sử dụng ID khác.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        // DELETE: api/SanPham/{id}
        [HttpDelete("{id}")]
        
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            var hasInvoicesResult = await ProductHasInvoices(id);
            if (hasInvoicesResult.Value)
            {
                return BadRequest("Không thể xóa sản phẩm. Sản phẩm này liên quan đến một hoặc nhiều hóa đơn.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("ProductHasInvoices/{productId}")]
        public async Task<ActionResult<bool>> ProductHasInvoices(string productId)
        {
            bool hasInvoices = await _context.InvoiceDetails.AnyAsync(id => id.ProductID == productId);
            return Ok(hasInvoices);
        }
    }
}
