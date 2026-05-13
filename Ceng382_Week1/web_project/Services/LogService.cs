using System;
using tastemam.Data;
using tastemam.Models;

namespace tastemam.Services
{
    public class LogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string eventType, string message, string userEmail = "", string level = "Info")
        {
            var log = new SystemLog
            {
                EventType = eventType,
                Message = message,
                UserEmail = userEmail,
                Date = DateTime.Now,
                Level = level
            };

            _context.SystemLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}