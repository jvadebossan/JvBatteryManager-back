namespace BatteryManager.API.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public Dictionary<string, string> Validate()
    {
        var errors = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(Email))
            errors["email"] = "Email é obrigatório";
        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            errors["password"] = "Senha deve ter pelo menos 6 caracteres";
        if (Password != ConfirmPassword)
            errors["confirmPassword"] = "As senhas não coincidem";
        return errors;
    }
}