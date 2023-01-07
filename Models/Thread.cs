using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ChatBackend.Models
{
	public class ACSThread
	{
		[Key]
		public string ThreadId { get; set; }
		public string ModeratorIdentityId { get; set; }
	}
}

