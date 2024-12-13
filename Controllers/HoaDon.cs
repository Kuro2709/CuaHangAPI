using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuaHangAPI.Data;
using CuaHangAPI.Dtos;
using CuaHangAPI.Models;

namespace CuaHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        // GET: api/HoaDon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDonDto>>> GetInvoices()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .ToListAsync();

            var invoiceDtos = invoices.Select(i => new HoaDonDto
            {
                InvoiceID = i.InvoiceID,
                CustomerID = i.CustomerID,
                InvoiceDate = i.InvoiceDate,
                TotalPrice = i.TotalPrice,
                Customer = new KhachHangDto
                {
                    CustomerID = i.Customer.CustomerID,
                    CustomerName = i.Customer.CustomerName,
                    Phone = i.Customer.Phone
                },
                ChiTietHoaDon = i.InvoiceDetails.Select(d => new ChiTietHoaDonDto
                {
                    InvoiceDetailID = d.InvoiceDetailID,
                    InvoiceID = d.InvoiceID,
                    ProductID = d.ProductID,
                    Quantity = d.Quantity,
                    TotalPrice = d.TotalPrice,
                    Product = new SanPhamDto
                    {
                        ProductID = d.Product.ProductID,
                        ProductName = d.Product.ProductName,
                        Price = d.Product.Price
                    }
                }).ToList()
            }).ToList();

            return Ok(invoiceDtos);
        }

        // GET: api/HoaDon/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HoaDonDto>> GetInvoiceById(string id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var invoiceDto = new HoaDonDto
            {
                InvoiceID = invoice.InvoiceID,
                CustomerID = invoice.CustomerID,
                InvoiceDate = invoice.InvoiceDate,
                TotalPrice = invoice.TotalPrice,
                Customer = new KhachHangDto
                {
                    CustomerID = invoice.Customer.CustomerID,
                    CustomerName = invoice.Customer.CustomerName,
                    Phone = invoice.Customer.Phone
                },
                ChiTietHoaDon = invoice.InvoiceDetails.Select(d => new ChiTietHoaDonDto
                {
                    InvoiceDetailID = d.InvoiceDetailID,
                    InvoiceID = d.InvoiceID,
                    ProductID = d.ProductID,
                    Quantity = d.Quantity,
                    TotalPrice = d.TotalPrice,
                    Product = new SanPhamDto
                    {
                        ProductID = d.Product.ProductID,
                        ProductName = d.Product.ProductName,
                        Price = d.Product.Price
                    }
                }).ToList()
            };

            return Ok(invoiceDto);
        }

        // POST: api/HoaDon
        [HttpPost]
        public async Task<ActionResult<HoaDonDto>> AddInvoice(ThemHoaDonDtos invoiceDto)
        {
            if (invoiceDto.InvoiceID == null)
            {
                return BadRequest("InvoiceID cannot be null.");
            }

            // Trim spaces and convert InvoiceID to uppercase
            invoiceDto.InvoiceID = invoiceDto.InvoiceID.Trim().ToUpper().Replace(" ", "");

            // Check for duplicate InvoiceID
            if (await _context.Invoices.AnyAsync(i => i.InvoiceID == invoiceDto.InvoiceID))
            {
                return Conflict("Mã hóa đơn đã tồn tại.");
            }

            // Find the customer
            var customer = await _context.Customers.FindAsync(invoiceDto.CustomerID);
            if (customer == null)
            {
                return BadRequest($"Customer with ID {invoiceDto.CustomerID} not found.");
            }

            // Create the invoice object first
            var invoice = new ThongTinHoaDon
            {
                InvoiceID = invoiceDto.InvoiceID,
                CustomerID = invoiceDto.CustomerID,
                InvoiceDate = invoiceDto.InvoiceDate,
                TotalPrice = 0, // Initialize TotalPrice to 0
                Customer = customer, // Set the required Customer property
                InvoiceDetails = { }
            };

            // Populate InvoiceDetails from DTO and calculate TotalPrice
            foreach (var detailDto in invoiceDto.InvoiceDetails)
            {
                var product = await _context.Products.FindAsync(detailDto.ProductID);
                if (product == null)
                {
                    return BadRequest($"Product with ID {detailDto.ProductID} not found.");
                }

                var detail = new ThongTinChiTietHoaDon
                {
                    InvoiceID = invoiceDto.InvoiceID,
                    ProductID = detailDto.ProductID,
                    Quantity = detailDto.Quantity,
                    TotalPrice = detailDto.Quantity * product.Price, // Calculate TotalPrice
                    Product = product, // Set the required Product property
                    Invoice = invoice // Set the required Invoice property
                };

                invoice.InvoiceDetails.Add(detail);
                invoice.TotalPrice += detail.TotalPrice; // Add to the overall TotalPrice
            }

            // Insert invoice and invoice details
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoices), new { id = invoice.InvoiceID }, invoiceDto);
        }

        // PUT: api/HoaDon/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(string id, ChinhSuaHoaDonDto invoiceDto)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Find the customer
            var customer = await _context.Customers.FindAsync(invoiceDto.CustomerID);
            if (customer == null)
            {
                return BadRequest($"Customer with ID {invoiceDto.CustomerID} not found.");
            }

            // Update invoice header
            invoice.CustomerID = invoiceDto.CustomerID;
            invoice.InvoiceDate = invoiceDto.InvoiceDate;
            invoice.TotalPrice = 0;
            invoice.Customer = customer;

            // Update invoice details
            _context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);
            invoice.InvoiceDetails.Clear();
            foreach (var detailDto in invoiceDto.InvoiceDetails)
            {
                var product = await _context.Products.FindAsync(detailDto.ProductID);
                if (product == null)
                {
                    return BadRequest($"Product with ID {detailDto.ProductID} not found.");
                }

                var detail = new ThongTinChiTietHoaDon
                {
                    InvoiceID = id,
                    ProductID = detailDto.ProductID,
                    Quantity = detailDto.Quantity,
                    TotalPrice = detailDto.Quantity * product.Price, // Calculate TotalPrice
                    Product = product, // Set the required Product property
                    Invoice = invoice // Set the required Invoice property
                };

                invoice.InvoiceDetails.Add(detail);
                invoice.TotalPrice += detail.TotalPrice; // Add to the overall TotalPrice
            }

            _context.Entry(invoice).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/HoaDon/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(string id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Remove related invoice details
            _context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);

            // Remove the invoice
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/HoaDon/Customers
        [HttpGet("Customers")]
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

        // GET: api/HoaDon/Products
        [HttpGet("Products")]
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            var productDtos = products.Select(p => new SanPhamDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Price = p.Price
            }).ToList();

            return Ok(productDtos);
        }

        // GET: api/HoaDon/ProductPrice/{productID}
        [HttpGet("ProductPrice/{productID}")]
        public async Task<ActionResult<decimal>> GetProductPriceByID(string productID)
        {
            var product = await _context.Products.FindAsync(productID);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.Price);
        }

        private decimal GetProductPrice(string productID)
        {
            var product = _context.Products.Find(productID);
            return product?.Price ?? 0;
        }
    }
}
