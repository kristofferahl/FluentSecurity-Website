using System.ComponentModel.DataAnnotations;

namespace FluentSecurity.Website.Models
{
	public class ContactEditModel
	{
		[Required] public string Name { get; set; }
		[Required] public string Email { get; set; }
		[Required] public string Subject { get; set; }
		[Required] public string Message { get; set; }

		public bool EmailSent { get; set; }
	}
}