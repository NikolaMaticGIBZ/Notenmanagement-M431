using Microsoft.JSInterop;

/// <summary>
/// Authentication Helper
/// </summary>
public static class AuthHelper
{
    /// <summary>
    /// Determines whether [is logged in] [the specified js].
    /// </summary>
    /// <param name="js">The js.</param>
    /// <returns>
    ///   <c>true</c> if [is logged in] [the specified js]; otherwise, <c>false</c>.
    /// </returns>
    public static async Task<bool> IsLoggedIn(IJSRuntime js)
    {
        string token = await js.InvokeAsync<string>("localStorage.getItem", "jwt");
        if (string.IsNullOrEmpty(token)) return false;

        // Check expiration
        try
        {
            string[] parts = token.Split('.');
            if (parts.Length != 3) return false;

            string payload = parts[1].Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            string json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(json);
            long exp = doc.RootElement.GetProperty("exp").GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(exp) > DateTimeOffset.UtcNow;
        }
        catch
        {
            return false;
        }
    }
}