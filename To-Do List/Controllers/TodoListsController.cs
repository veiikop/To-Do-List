using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using To_Do_List.Models.DTO;
using To_Do_List.Services;

namespace To_Do_List.Controllers
{
    [Route("api/[controller]")]
    [Authorize] // Все методы требуют аутентификации
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly ITodoListService _service;

        public TodoListsController(ITodoListService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить все списки (User — только свои, Admin — все)
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<TodoListDTO>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        /// <summary>
        /// Получить список по ID
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<TodoListDTO> GetById(int id)
        {
            var list = _service.GetById(id);
            if (list == null)
                return NotFound(new { Success = false, ErrorMessage = "Список не найден" });

            return Ok(list);
        }

        /// <summary>
        /// Создать новый список (доступно всем аутентифицированным)
        /// </summary>
        [HttpPost]
        public ActionResult<TodoListDTO> Create([FromBody] CreateTodoListDTO dto)
        {
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Обновить список (только владелец или Admin)
        /// </summary>
        [HttpPut("{id}")]
        public ActionResult<TodoListDTO> Update(int id, [FromBody] CreateTodoListDTO dto)
        {
            var updated = _service.Update(id, dto);
            if (updated == null)
                return NotFound(new { Success = false, ErrorMessage = "Список не найден" });

            return Ok(updated);
        }

        /// <summary>
        /// Удалить список (только владелец или Admin)
        /// </summary>
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