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
    [Authorize(Roles = "User")]
    public class ExpenseController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            using (var context = new SeaExpenContext())
            {
                var data = from a in context.Expenses
                           join c in context.ExpenseCategories on a.Category equals c.Id
                           select new ExpenseModel
                           {
                               Id = a.Id,
                               Category = a.Category,
                               CategoryName = c.Category,
                               Amount = a.Amount,
                               RecordedDate = a.ExpenseDate,
                               UserId = a.UserId,
                           };
                
                return Ok(data.ToList());
            }
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            using (var context = new SeaExpenContext()) {
                var data = (from a in context.Expenses
                           join c in context.ExpenseCategories on a.Category equals c.Id
                           where a.Id == id
                           select new ExpenseModel
                           {
                               Id = a.Id,
                               Category = a.Category,
                               CategoryName = c.Category,
                               Amount = a.Amount,
                               RecordedDate = a.ExpenseDate,
                               UserId = a.UserId,
                           }).SingleOrDefault();

                return Ok(data);
            }
            
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExpenseModel model)
        {
            using (var context = new SeaExpenContext())
            {
                var i = context.Database.ExecuteSql($"exec usp_Manage_");
            }
        }
    }
}
