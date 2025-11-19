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

        /// <summary>
        /// Получить все задачи (User — свои, Admin — все)
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<TodoItemDTO>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        /// <summary>
        /// Получить задачу по ID
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<TodoItemDTO> GetById(int id)
        {
            var item = _service.GetById(id);
            if (item == null)
                return NotFound(new { Success = false, ErrorMessage = "Задача не найдена" });

            return Ok(item);
        }

        /// <summary>
        /// Получить задачи из конкретного списка
        /// </summary>
        [HttpGet("by-list/{todoListId}")]
        public ActionResult<IEnumerable<TodoItemDTO>> GetItemsByListId(int todoListId)
        {
            var items = _service.GetItemsByListId(todoListId);
            return Ok(items);
        }

        /// <summary>
        /// Создать новую задачу
        /// </summary>
        [HttpPost]
        public ActionResult<TodoItemDTO> Create([FromBody] CreateTodoItemDTO dto)
        {
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Обновить задачу
        /// </summary>
        [HttpPut("{id}")]
        public ActionResult<TodoItemDTO> Update(int id, [FromBody] UpdateTodoItemDTO dto)
        {
            var updated = _service.Update(id, dto);
            if (updated == null)
                return NotFound(new { Success = false, ErrorMessage = "Задача не найдена" });

            return Ok(updated);
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
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