using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Repository;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

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

        public async Task<(List<NotificationDto> Notifications, int TotalCount)> GetNotificationsPagedAsync(NotificationQueryDto query)
        {
            query ??= new NotificationQueryDto();
            query.Page = Math.Max(1, query.Page);
            query.PageSize = GetAvailablePageSizes().Contains(query.PageSize)
                ? query.PageSize
                : 10;

            var baseQuery = _context.Notifications.AsQueryable();

            // Filtriranje po korisniku (ako je navedeno)
            if (query.UserId.HasValue)
            {
                baseQuery = baseQuery.Where(n => n.UserId == query.UserId.Value);
            }

            // Sortiranje
            var validSortColumns = GetAvailableSortColumns();
            if (!string.IsNullOrWhiteSpace(query.SortBy) && validSortColumns.Contains(query.SortBy))
            {
                var sortDirection = query.SortDescending ? "descending" : "ascending";
                baseQuery = baseQuery.OrderBy($"{query.SortBy} {sortDirection}");
            }
            else
            {
                // Default sort
                baseQuery = baseQuery.OrderByDescending(n => n.CreatedAt);
            }

            // Paginacija
            var totalCount = await baseQuery.CountAsync();
            var notifications = await baseQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return (notifications, totalCount);
        }

        public List<int> GetAvailablePageSizes()
        {
            return new List<int> { 5, 10, 15, 20, 50 };
        }

        public List<string> GetAvailableSortColumns()
        {
            return new List<string>
        {
            "Id",
            "Message",
            "CreatedAt",
            "UserId"
        };
        }

    }
}
