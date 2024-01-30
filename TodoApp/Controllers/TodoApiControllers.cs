using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Authorize]
    [Route("api/TodoApi")]
    [ApiController]
    public class TodoApiControllers : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public TodoApiControllers(ApplicationDbContext db)
        {
            _db= db;
        }

        [HttpGet]
        //[Route("GetData")]
        public ActionResult GetTasks()
        {
            var uid = HttpContext.User.FindFirst("uid");
            int id = int.Parse(uid.Value);
            List<Tasks> tasks = _db.Tasks.Where(t => t.UserId == id).ToList();
            return Ok(tasks);
        }

        [HttpGet("Name" , Name ="GetTask")]
        public ActionResult GetTaskByName(string name)
        {
            if (name.Length == 0)
            {
                return BadRequest();
            }

            var uid = HttpContext.User.FindFirst("uid");
            int id = int.Parse(uid.Value);
            List<Tasks> tasks = _db.Tasks.Where(t => t.UserId == id).ToList();
            
            Tasks task = tasks.FirstOrDefault(t => t.TaskName.ToLower() == name.ToLower());
            if (task == null)
            {
                ModelState.AddModelError("CustomError", "Task With this :"+name+" is not Exists");
                return BadRequest(ModelState);
            }
            return Ok(task);
        }

        [HttpPost]
        public ActionResult PostTask([FromBody] AddTask task)
        {
            if (task == null)
            {
                return BadRequest();
            }

            var uid = HttpContext.User.FindFirst("uid");
            List<Tasks> tasks = _db.Tasks.Where(t => t.UserId == int.Parse(uid.Value)).ToList();

            if (tasks.FirstOrDefault(t => t.TaskName.ToLower() == task.name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Task already Exists");
                return BadRequest(ModelState);
            }
            //if (_db.Tasks.FirstOrDefault(u => u.TaskId == task.TaskId) != null)
            //{
            //    ModelState.AddModelError("CustomError", "TaskId already Exists");
            //    return BadRequest(ModelState);
            //}
            
            //int id = tasks.Max(t => t.TaskId);
            Tasks model = new Tasks()
            {
                UserId = int.Parse(uid.Value),
                TaskName = task.name,
                Description = task.description,
                Status = (Models.Options)task.status
            };

            _db.Tasks.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetTask", new { id = model.TaskId}, model);
        }

        [HttpDelete]
        public IActionResult DeleteTask(string name)
        {
            if(name == null) {
            return BadRequest();
            }
            var uid = HttpContext.User.FindFirst("uid");
            List<Tasks> tasks = _db.Tasks.Where(t => t.UserId == int.Parse(uid.Value)).ToList();

            var task = tasks.FirstOrDefault(t => t.TaskName.ToLower() == name.ToLower());

            if(task == null)
            {
                return BadRequest("Task that you want to delete is not exist");
            }

            _db.Remove(task);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut]
        public IActionResult UpdateTask(string name, [FromBody]AddTask utask)
        {
            if (name == null)
            {
                return BadRequest();
            }
            
            var uid = HttpContext.User.FindFirst("uid");
            var tasks = _db.Tasks.AsNoTracking().Where(t => t.UserId == int.Parse(uid.Value));


            if (tasks.FirstOrDefault(t => t.TaskName.ToLower() == name.ToLower()) == null)
            {
                return BadRequest("Task that you want to update is not exist");
            }
            Console.WriteLine(utask.name);
            var ExistingTask = tasks.Where(t => t.TaskName.ToLower() == utask.name.ToLower());
            Console.WriteLine("this is new task "+ ExistingTask.Count());
            if (ExistingTask.Count() != 0)
            {
                return BadRequest("New Task Name Already exist");
            }
            ExistingTask.AsNoTracking();
            var task = tasks.AsNoTracking().FirstOrDefault(t => t.TaskName.ToLower() == name.ToLower());

            Tasks model = new()
            {
                TaskId = task.TaskId,
                TaskName = utask.name,
                UserId = task.UserId,
                Description = utask.description,
                Status = (Models.Options)utask.status
            };
            _db.Tasks.Update(model);
            _db.SaveChanges();
            return CreatedAtRoute("GetTask" ,new {name = name} , model);
        }
    }
}
