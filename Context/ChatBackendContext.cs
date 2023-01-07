using System;
using Microsoft.EntityFrameworkCore;
using ChatBackend.Models;

namespace ChatBackend.Context
{
	public class ChatBackendContext : DbContext
	{
		public ChatBackendContext(DbContextOptions<ChatBackendContext> options) : base(options)
		{
		}

        public DbSet<User> User { get; set; }
        public DbSet<ACSThread> ACSThread { get; set; }

    }
}

