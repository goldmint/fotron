using System.Threading.Tasks;

namespace Fotron.WebApplication.Services.Email {

	public interface IEmailSender {

		Task<bool> Send(string[] recipients, string subject, string body);
	}
}
