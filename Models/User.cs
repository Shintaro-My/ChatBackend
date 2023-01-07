using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ChatBackend.Models
{
	public class User
	{
		[Key]
		public string UserName { get; set; }
		public string UserIdentityId { get; set; }
	}
}

