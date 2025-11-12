using Microsoft.AspNetCore.Mvc;
using To_Do_List.Models.DTO;
using To_Do_List.Services;

namespace To_Do_List.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly ITodoListService _service;

        public TodoListsController(ITodoListService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TodoListDTO>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<TodoListDTO> GetById(int id)
        {
            var list = _service.GetById(id);
            if (list == null)
                return NotFound(new { Success = false, ErrorMessage = "Список не найден" });
            return Ok(list);
        }

        [HttpPost]
        public ActionResult<TodoListDTO> Create([FromBody] CreateTodoListDTO dto)
        {
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public ActionResult<TodoListDTO> Update(int id, [FromBody] CreateTodoListDTO dto)
        {
            var updated = _service.Update(id, dto);
            if (updated == null)
                return NotFound(new { Success = false, ErrorMessage = "Список не найден" });
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var result = _service.Delete(id);
            if (!result)
                return NotFound(new { Success = false, ErrorMessage = "Список не найден" });
            return NoContent();
        }
    }
}