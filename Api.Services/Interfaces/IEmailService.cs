namespace Api.Services.Interfaces;

/// <summary>
/// Interface for Email Service
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a email with smtp client asynchronous.
    /// </summary>
    /// <param name="to">To.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="body">The body.</param>
    /// <returns>Returns a asynchronous method => sending email</returns>
    Task SendAsync(string to, string subject, string body);
}