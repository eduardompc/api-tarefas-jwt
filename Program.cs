using System.Security.Claims;
using System.Text;
using ApiTarefas.Models;
using ApiTarefas.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurações
var tokenSettings = builder.Configuration.GetSection("TokenSettings");
builder.Services.Configure<TokenSettings>(tokenSettings);

var secretKey = builder.Configuration["TokenSettings:SecretKey"] ?? "minha-chave-secreta-temporaria-mude-em-producao-32bytes!!";

// Autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();

// Swagger com suporte a JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Tarefas com JWT",
        Description = "API Minimal com autenticação JWT e Swagger",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Suporte",
            Email = "suporte@exemplo.com"
        }
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Banco de dados em memória
var usuariosDB = new Dictionary<string, UsuarioDB>();
var tarefasDB = new List<Tarefa>();
var contadorId = 1;

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Tarefas v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseAuthentication();
app.UseAuthorization();

// Helper para obter usuário atual
string? GetCurrentUser(ClaimsPrincipal user)
{
    return user.Identity?.Name;
}

// ------------------- ENDPOINTS -------------------

// Health Check
app.MapGet("/health", () => Results.Ok(new { status = "ok", usuarios_registrados = usuariosDB.Count }))
    .WithName("HealthCheck")
    .WithTags("Root")
    .WithOpenApi();

// Registrar usuário
app.MapPost("/registrar", async (Usuario usuario, ITokenService tokenService) =>
{
    if (usuariosDB.ContainsKey(usuario.Username))
    {
        return Results.BadRequest(new { mensagem = "Usuário já existe" });
    }
    
    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
    usuariosDB[usuario.Username] = new UsuarioDB
    {
        Username = usuario.Username,
        HashedPassword = hashedPassword
    };
    
    return Results.Created($"/usuarios/{usuario.Username}", new { mensagem = "Usuário criado com sucesso" });
})
.WithName("RegistrarUsuario")
.WithTags("Autenticação")
.WithOpenApi();

// Login
app.MapPost("/login", (Usuario usuario, ITokenService tokenService) =>
{
    if (!usuariosDB.ContainsKey(usuario.Username))
    {
        return Results.Unauthorized();
    }
    
    var userDB = usuariosDB[usuario.Username];
    if (!BCrypt.Net.BCrypt.Verify(usuario.Password, userDB.HashedPassword))
    {
        return Results.Unauthorized();
    }
    
    var token = tokenService.GenerateToken(usuario.Username);
    var tokenSettingsConfig = builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>();
    
    return Results.Ok(new TokenResponse
    {
        AccessToken = token,
        TokenType = "bearer",
        ExpiresIn = tokenSettingsConfig?.AccessTokenExpirationMinutes * 60 ?? 1800
    });
})
.WithName("Login")
.WithTags("Autenticação")
.WithOpenApi();

// Criar tarefa (protegido)
app.MapPost("/tarefas", async (TarefaCreate tarefaCreate, ClaimsPrincipal user) =>
{
    var currentUser = GetCurrentUser(user);
    if (string.IsNullOrEmpty(currentUser))
    {
        return Results.Unauthorized();
    }
    
    var novaTarefa = new Tarefa
    {
        Id = contadorId++,
        Titulo = tarefaCreate.Titulo,
        Descricao = tarefaCreate.Descricao,
        Concluida = false,
        Usuario = currentUser
    };
    
    tarefasDB.Add(novaTarefa);
    return Results.Created($"/tarefas/{novaTarefa.Id}", novaTarefa);
})
.RequireAuthorization()
.WithName("CriarTarefa")
.WithTags("Tarefas")
.WithOpenApi();

// Listar tarefas (protegido)
app.MapGet("/tarefas", (bool? concluida, ClaimsPrincipal user) =>
{
    var currentUser = GetCurrentUser(user);
    if (string.IsNullOrEmpty(currentUser))
    {
        return Results.Unauthorized();
    }
    
    var userTarefas = tarefasDB.Where(t => t.Usuario == currentUser);
    
    if (concluida.HasValue)
    {
        userTarefas = userTarefas.Where(t => t.Concluida == concluida.Value);
    }
    
    return Results.Ok(userTarefas.ToList());
})
.RequireAuthorization()
.WithName("ListarTarefas")
.WithTags("Tarefas")
.WithOpenApi();

// Buscar tarefa por ID (protegido)
app.MapGet("/tarefas/{id:int}", (int id, ClaimsPrincipal user) =>
{
    var currentUser = GetCurrentUser(user);
    if (string.IsNullOrEmpty(currentUser))
    {
        return Results.Unauthorized();
    }
    
    var tarefa = tarefasDB.FirstOrDefault(t => t.Id == id && t.Usuario == currentUser);
    if (tarefa == null)
    {
        return Results.NotFound(new { mensagem = "Tarefa não encontrada" });
    }
    
    return Results.Ok(tarefa);
})
.RequireAuthorization()
.WithName("BuscarTarefa")
.WithTags("Tarefas")
.WithOpenApi();

// Atualizar tarefa (protegido)
app.MapPut("/tarefas/{id:int}", (int id, TarefaUpdate tarefaUpdate, ClaimsPrincipal user) =>
{
    var currentUser = GetCurrentUser(user);
    if (string.IsNullOrEmpty(currentUser))
    {
        return Results.Unauthorized();
    }
    
    var tarefa = tarefasDB.FirstOrDefault(t => t.Id == id && t.Usuario == currentUser);
    if (tarefa == null)
    {
        return Results.NotFound(new { mensagem = "Tarefa não encontrada" });
    }
    
    if (!string.IsNullOrEmpty(tarefaUpdate.Titulo))
        tarefa.Titulo = tarefaUpdate.Titulo;
    
    if (!string.IsNullOrEmpty(tarefaUpdate.Descricao))
        tarefa.Descricao = tarefaUpdate.Descricao;
    
    if (tarefaUpdate.Concluida.HasValue)
        tarefa.Concluida = tarefaUpdate.Concluida.Value;
    
    return Results.Ok(tarefa);
})
.RequireAuthorization()
.WithName("AtualizarTarefa")
.WithTags("Tarefas")
.WithOpenApi();

// Deletar tarefa (protegido)
app.MapDelete("/tarefas/{id:int}", (int id, ClaimsPrincipal user) =>
{
    var currentUser = GetCurrentUser(user);
    if (string.IsNullOrEmpty(currentUser))
    {
        return Results.Unauthorized();
    }
    
    var tarefa = tarefasDB.FirstOrDefault(t => t.Id == id && t.Usuario == currentUser);
    if (tarefa == null)
    {
        return Results.NotFound(new { mensagem = "Tarefa não encontrada" });
    }
    
    tarefasDB.Remove(tarefa);
    return Results.NoContent();
})
.RequireAuthorization()
.WithName("DeletarTarefa")
.WithTags("Tarefas")
.WithOpenApi();

app.Run();