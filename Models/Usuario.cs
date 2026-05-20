namespace ApiTarefas.Models;

public class Usuario
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UsuarioDB
{
    public string Username { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
}

public class RegistroResponse
{
    public string Mensagem { get; set; } = string.Empty;
}