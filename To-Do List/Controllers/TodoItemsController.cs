using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using To_Do_List.Models.DTO;
using To_Do_List.Services;

namespace To_Do_List.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemService _service;

        public TodoItemsController(ITodoItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TodoItemDTO>> GetAll()                                 
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<TodoItemDTO> GetById(int id)
        {
            var item = _service.GetById(id);
            if (item == null)
                return NotFound(
                    new {
                        Success = false,
                        ErrorMessage = "Задача не найдена" 
                    });

            return Ok(item);
        }

        [HttpGet("by-list/{todoListId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<TodoItemDTO>> GetItemsByListId(int todoListId)
        {
            try
            {
                var items = _service.GetItemsByListId(todoListId);
                return Ok(items);
            }
            catch (KeyNotFoundException)
            {
                throw; 
            }
        }

        [HttpPost]
        public ActionResult<TodoItemDTO> Create([FromBody] CreateTodoItemDTO dto)
        {
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public ActionResult<TodoItemDTO> Update(int id, [FromBody] UpdateTodoItemDTO dto)
        {
            var updated = _service.Update(id, dto);
            if (updated == null)
                return NotFound(new { Success = false, ErrorMessage = "Задача не найдена" });
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var result = _service.Delete(id);
            if (!result)
                return NotFound(new { Success = false, ErrorMessage = "Задача не найдена" });
            return NoContent();
        }
    }
}