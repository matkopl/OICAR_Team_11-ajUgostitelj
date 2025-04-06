using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public NotificationService(IRepositoryFactory repositoryFactory, IMapper mapper, AppDbContext context)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
        {
            var repo = _repositoryFactory.GetRepository<Notification>();
            var notifications = await repo.GetAllAsync();
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Notification>();
            var notification = await repo.GetByIdAsync(id);
            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(int userId)
        {
            var repo = _repositoryFactory.GetRepository<Notification>();
            var notifications = await repo.FindAsync(n => n.UserId == userId);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<NotificationDto> CreateNotificationAsync(NotificationDto notificationDto)
        {
            var repo = _repositoryFactory.GetRepository<Notification>();
            var notification = _mapper.Map<Notification>(notificationDto);
            notification.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(notification);
            await repo.SaveChangesAsync();

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task UpdateNotificationAsync(int id, NotificationDto notificationDto)
        {
            var repo = _repositoryFactory.GetRepository<Notification>();
            var existingNotification = await repo.GetByIdAsync(id);

            if (existingNotification == null)
                throw new KeyNotFoundException("Notification not found");

            _mapper.Map(notificationDto, existingNotification);
            repo.Update(existingNotification);
            await repo.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Notification>();
            var notification = await repo.GetByIdAsync(id);

            if (notification == null)
                throw new KeyNotFoundException("Notification not found");

            repo.Remove(notification);
            await repo.SaveChangesAsync();
        }
    }
}
