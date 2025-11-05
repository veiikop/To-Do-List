using AutoMapper;
using To_Do_List.Models;
using To_Do_List.Models.DTO;
using To_Do_List.Repositories;

namespace To_Do_List.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ITodoItemRepository _repository;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IMapper _mapper;

        public TodoItemService(ITodoItemRepository repository, ITodoListRepository todoListRepository, IMapper mapper)
        {
            _repository = repository;
            _todoListRepository = todoListRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// Получить все задачи
        /// </summary>
        public IEnumerable<TodoItemDTO> GetAll()
        {
            return _mapper.Map<IEnumerable<TodoItemDTO>>(_repository.GetAll());
        }

        /// <summary>
        /// Получить задачу по ID
        /// </summary>
        public TodoItemDTO? GetById(int id)
        {
            var item = _repository.GetById(id);
            return item == null ? null : _mapper.Map<TodoItemDTO>(item);
        }

        /// <summary>
        /// Получить все задачи из списка
        /// </summary>
        public IEnumerable<TodoItemDTO> GetItemsByListId(int todoListId)
        {
            return _mapper.Map<IEnumerable<TodoItemDTO>>(_repository.GetItemsByListId(todoListId));
        }

        /// <summary>
        /// Создать новую задачу
        /// </summary>
        public TodoItemDTO Create(CreateTodoItemDTO dto)
        {
            if (!_todoListRepository.Exists(dto.TodoListId))
                throw new KeyNotFoundException("Список не найден");

            var item = _mapper.Map<TodoItem>(dto);
            item.CreatedAt = DateTime.UtcNow;
            return _mapper.Map<TodoItemDTO>(_repository.Create(item));
        }

        /// <summary>
        /// Обновить задачу
        /// </summary>
        public TodoItemDTO? Update(int id, UpdateTodoItemDTO dto)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);

            if (dto.IsCompleted == true && !existing.IsCompleted)
            {
                existing.CompletedAt = DateTime.UtcNow;
            }
            else if (dto.IsCompleted == false && existing.IsCompleted)
            {
                existing.CompletedAt = null;
            }

            var updated = _repository.Update(existing);
            return _mapper.Map<TodoItemDTO>(updated);
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
        public bool Delete(int id)
        {
            return _repository.Delete(id);
        }
    }
}