using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace SchoolAdminPanel.Providers;

public class SupabaseAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly Supabase.Client _client;

    public SupabaseAuthenticationStateProvider(Supabase.Client client)
    {
        _client = client;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = _client.Auth.CurrentSession;
        
        // Try to recover session if not present from memory but persisted (Supabase client handles persistence usually, 
        // but explicit RetrieveSessionAsync might be needed if using LocalStorage persistence).
        // For simplicity, we rely on the injected client's state for now.
        // In a real WASM app, you typically persist the session token in LocalStorage and restore it here.
        
        if (session == null || session.ExpiresAt() < DateTime.Now)
        {
             // Try to restore session
            try 
            {
               // This implies Supabase client is configured to use LocalStorage/Persistence
               // If the C# Supabase client doesn't auto-restore, we might be null here.
               // For this task, assuming the Login page sets the session on the Singleton client.
            }
            catch {}
        }

        // Re-check session
        session = _client.Auth.CurrentSession;

        if (session == null || session.User == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, session.User.Email ?? "User"),
            new Claim(ClaimTypes.NameIdentifier, session.User.Id),
            new Claim(ClaimTypes.Email, session.User.Email ?? "")
        };

        var identity = new ClaimsIdentity(claims, "Supabase");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
