using AutoMapper;
using To_Do_List.Models;
using To_Do_List.Models.DTO;
using To_Do_List.Repositories;
using System.Security.Claims;

namespace To_Do_List.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ITodoItemRepository _repository;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TodoItemService(
            ITodoItemRepository repository,
            ITodoListRepository todoListRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _todoListRepository = todoListRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int CurrentUserId => int.Parse(
            _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        private string CurrentUserRole =>
            _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

        private bool IsAdmin => CurrentUserRole == "Admin";

        /// <summary>
        /// Получить все задачи (Admin — все, User — только свои)
        /// </summary>
        public IEnumerable<TodoItemDTO> GetAll()
        {
            var query = _repository.GetAll();

            if (!IsAdmin)
            {
                query = query.Where(i => i.TodoList.UserId == CurrentUserId);
            }

            return _mapper.Map<IEnumerable<TodoItemDTO>>(query.ToList());
        }

        /// <summary>
        /// Получить задачу по ID
        /// </summary>
        public TodoItemDTO? GetById(int id)
        {
            var item = _repository.GetById(id)
                       ?? throw new KeyNotFoundException("Задача не найдена");

            if (!IsAdmin && item.TodoList.UserId != CurrentUserId)
                throw new UnauthorizedAccessException("Доступ к чужой задаче запрещён");

            return _mapper.Map<TodoItemDTO>(item);
        }

        /// <summary>
        /// Получить задачи из конкретного списка
        /// </summary>
        public IEnumerable<TodoItemDTO> GetItemsByListId(int todoListId)
        {
            var todoList = _todoListRepository.GetById(todoListId)
                           ?? throw new KeyNotFoundException("Список не найден");

            if (!IsAdmin && todoList.UserId != CurrentUserId)
                throw new UnauthorizedAccessException("Доступ к чужому списку запрещён");

            var items = _repository.GetItemsByListId(todoListId);
            return _mapper.Map<IEnumerable<TodoItemDTO>>(items);
        }

        /// <summary>
        /// Создать новую задачу
        /// </summary>
        public TodoItemDTO Create(CreateTodoItemDTO dto)
        {
            var todoList = _todoListRepository.GetById(dto.TodoListId)
                           ?? throw new KeyNotFoundException("Список не найден");

            if (!IsAdmin && todoList.UserId != CurrentUserId)
                throw new UnauthorizedAccessException("Нельзя добавлять задачи в чужой список");

            var item = _mapper.Map<TodoItem>(dto);
            item.CreatedAt = DateTime.UtcNow;

            var created = _repository.Create(item);
            return _mapper.Map<TodoItemDTO>(created);
        }

        /// <summary>
        /// Обновить задачу
        /// </summary>
        public TodoItemDTO? Update(int id, UpdateTodoItemDTO dto)
        {
            var existing = _repository.GetById(id)
                           ?? throw new KeyNotFoundException("Задача не найдена");

            if (!IsAdmin && existing.TodoList.UserId != CurrentUserId)
                throw new UnauthorizedAccessException("Нельзя редактировать чужую задачу");

            _mapper.Map(dto, existing);

            if (dto.IsCompleted == true && !existing.IsCompleted)
                existing.CompletedAt = DateTime.UtcNow;
            else if (dto.IsCompleted == false && existing.IsCompleted)
                existing.CompletedAt = null;

            var updated = _repository.Update(existing);
            return _mapper.Map<TodoItemDTO>(updated);
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
        public bool Delete(int id)
        {
            var item = _repository.GetById(id);
            if (item == null) return false;

            if (!IsAdmin && item.TodoList.UserId != CurrentUserId)
                throw new UnauthorizedAccessException("Нельзя удалять чужую задачу");

            return _repository.Delete(id);
        }
    }
}