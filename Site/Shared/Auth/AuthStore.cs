using Site.Shared.Helpers;
using Site.Shared.Storage;

namespace Site.Shared.Auth;

public class AuthStore
{
    private const string CredentialsKey = "credentials";

    private readonly CredentialsHelper _credentialsHelper;
    private readonly LocalStorage _localStorage;

    public AuthStore(CredentialsHelper credentialsHelper, LocalStorage localStorage)
    {
        _credentialsHelper = credentialsHelper;
        _localStorage = localStorage;
    }

    public bool HasCredentials()
    {
        return _localStorage.HasKey(CredentialsKey);
    }

    public Credentials? TryLoadCredentials()
    {
        if (!_localStorage.TryGetString(CredentialsKey, out var credentials) || string.IsNullOrWhiteSpace(credentials))
            return null;

        return _credentialsHelper.Decode(credentials);
    }

    public void ClearCredentials()
    {
        _localStorage.Remove(CredentialsKey);
    }

    public void SaveCredentials(string user, string password)
    {
        var credentials = _credentialsHelper.Encode(user, password);

        _localStorage.SetString(CredentialsKey, credentials);
    }
}
