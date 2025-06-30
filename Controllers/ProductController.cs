using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class ProductRequest
        {
            public required string Name { get; set; }
            public decimal Price { get; set; }
            public int Qty { get; set; }
            public required string Description { get; set; }
            public Guid CategoryId { get; set; }
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            return Ok(products);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _context
                .Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            return Ok(new { message = "Product retrieved successfully.", data = product });
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] ProductRequest request)
        {
            // Validasi category exists
            if (!await _context.ProductCategories.AnyAsync(c => c.Id == request.CategoryId))
            {
                return BadRequest(new { message = "Invalid Category ID." });
            }

            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Qty = request.Qty,
                Description = request.Description,
                CategoryId = request.CategoryId,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                new { message = "Product created successfully.", data = product }
            );
        }

        // PUT: api/Product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Product updatedProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found." });
            }

            // Update hanya field yang diperlukan
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Qty = updatedProduct.Qty;
            existingProduct.Description = updatedProduct.Description;

            // Jika ingin mengupdate categoryId juga
            if (updatedProduct.CategoryId != Guid.Empty)
            {
                existingProduct.CategoryId = updatedProduct.CategoryId;
            }

            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    Message = "Product successfully updated.",
                    Data = new
                    {
                        existingProduct.Id,
                        existingProduct.Name,
                        existingProduct.Price,
                        existingProduct.Qty,
                        existingProduct.Description,
                        existingProduct.CategoryId,
                    },
                }
            );
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully." });
        }

        // GET: api/Product/ByCategory/5
        [HttpGet("ByCategory/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(Guid categoryId)
        {
            var products = await _context
                .Products.Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .ToListAsync();

            return Ok(
                new
                {
                    message = $"Products for category {categoryId} retrieved successfully.",
                    data = products,
                }
            );
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
