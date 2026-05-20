namespace ApiTarefas.Models;

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "bearer";
    public int ExpiresIn { get; set; }
}