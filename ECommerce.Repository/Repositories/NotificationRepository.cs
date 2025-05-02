using ECommerce.Core.Models;
using ECommerce.Core.Repository.Contract;
using ECommerce.DashBoard.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ECommerceDbContext _context;

        public NotificationRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.AppUserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var unread = _context.Notifications.Where(n => n.AppUserId == userId && !n.IsRead);
            foreach (var n in unread)
                n.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

}
