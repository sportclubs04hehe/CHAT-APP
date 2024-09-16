using API.Data;
using API.DTOs.Message;
using API.Helpers;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace API.Repository.Impl
{
    public class MessageRepository(Context context,
        IMapper mapper) : IMessageRepository
    {

        public void AddMessage(Messages messages)
        {
            context.Messages.Add(messages);
        }

        public void DeleteMessage(Messages messages)
        {
            context.Messages.Remove(messages);
        }

        public async Task<Messages?> GetMessagesAsync(Guid id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username 
                && u.RecipientDeleted == false && u.DateRead == null),
            };

            var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

            return await PagedList<MessageDto>
                .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesThreadAsync(string currentUsername, string recipientUsername)
        {
            var query = context.Messages.Where(
                m => m.RecipientUsername == currentUsername && m.RecipientDeleted == false && m.SenderUsername == recipientUsername ||
                m.RecipientUsername == recipientUsername && m.SenderDeleted == false && m.SenderUsername == currentUsername
                ).OrderBy(m => m.MessageSent).AsQueryable();

            // Tin nhắn chưa đọc
            var unreadMessages = query.Where(u => u.DateRead == null &&
            u.RecipientUsername == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
                
                await context.SaveChangesAsync();
            }

            return await query.ProjectTo<MessageDto>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
