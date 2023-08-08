using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaExpenBackend.DB;
using SeaExpenBackend.Models;

namespace SeaExpenBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly SeaExpenContext _context;

        public CategoryController(SeaExpenContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //var action = 4;
            //var categories = context.Database.ExecuteSql($"exec usp_Manage_ExpenseCategory @Action = {action}");
            var categories = _context.ExpenseCategories.Select(x => new
            {
                Id = x.Id,
                Category = x.Category,
            }).ToList();

            return Ok(categories);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult GetById(int id)
        {
            
                var data = _context.ExpenseCategories.SingleOrDefault(c => c.Id == id);
                return Ok(data);
            
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(ExpenseCategoryModel category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                
                    _context.Database.ExecuteSql($"exec usp_Manage_ExpenseCategory @Action=1, @Category={category.Category}");
                    return Ok(new {message = "Category created"});
                
            }
            catch (Exception)
            {
                return StatusCode(500, new {message="Server Error"});
            }
            
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(ExpenseCategoryModel category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                
                    _context.Database.ExecuteSql($"exec usp_ManageCategory @Action = 2, @Id = {category.Id}, @Category = {category.Category}");
                    return Ok(new { message = "Category Updated" });
                
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Server Error" });
            }
            
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                
                    _context.Database.ExecuteSql($"exec usp_ManageCategory @Action =3, @Id={id}");
                    return Ok(new { message = "Category Deleted" });
                
            }
            catch
            {
                return StatusCode(500, new { error = "Server Error" });
            }
        }
    }
}
