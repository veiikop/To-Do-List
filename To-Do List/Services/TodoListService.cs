using AutoMapper;
using To_Do_List.Models;
using To_Do_List.Models.DTO;
using To_Do_List.Repositories;

namespace To_Do_List.Services
{
    public class TodoListService : ITodoListService
    {
        private readonly ITodoListRepository _repository;
        private readonly IMapper _mapper;

        public TodoListService(ITodoListRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить все списки дел
        /// </summary>
        public IEnumerable<TodoListDTO> GetAll()
        {
            return _mapper.Map<IEnumerable<TodoListDTO>>(_repository.GetAll());
        }

        /// <summary>
        /// Получить список по ID
        /// </summary>
        public TodoListDTO? GetById(int id)
        {
            var list = _repository.GetById(id);
            return list == null ? null : _mapper.Map<TodoListDTO>(list);
        }

        /// <summary>
        /// Создать новый список
        /// </summary>
        public TodoListDTO Create(CreateTodoListDTO dto)
        {
            var list = _mapper.Map<TodoList>(dto);
            list.CreatedAt = DateTime.UtcNow;
            return _mapper.Map<TodoListDTO>(_repository.Create(list));
        }

        /// <summary>
        /// Обновить существующий список
        /// </summary>
        public TodoListDTO? Update(int id, CreateTodoListDTO dto)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;
            return _mapper.Map<TodoListDTO>(_repository.Update(existing));
        }

        /// <summary>
        /// Удалить список
        /// </summary>
        public bool Delete(int id)
        {
            return _repository.Delete(id);
        }
    }
}