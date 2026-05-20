using System.Net;
using System.Net.Http.Json;
using ApiTarefas.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiTarefas.Tests;

public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Registrar_NovoUsuario_RetornaSucesso()
    {
        // Arrange
        var usuario = new Usuario 
        { 
            Username = $"testuser_{Guid.NewGuid()}", 
            Password = "senha123" 
        };

        // Act
        var response = await _client.PostAsJsonAsync("/registrar", usuario);
        var result = await response.Content.ReadFromJsonAsync<RegistroResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result?.Mensagem.Should().Be("Usuário criado com sucesso");
    }

    [Fact]
    public async Task Registrar_UsuarioDuplicado_RetornaErro()
    {
        // Arrange
        var usuario = new Usuario { Username = "duplicado", Password = "senha123" };
        await _client.PostAsJsonAsync("/registrar", usuario);

        // Act
        var response = await _client.PostAsJsonAsync("/registrar", usuario);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_CredenciaisCorretas_RetornaToken()
    {
        // Arrange
        var username = $"logintest_{Guid.NewGuid()}";
        var usuario = new Usuario { Username = username, Password = "senha123" };
        await _client.PostAsJsonAsync("/registrar", usuario);

        // Act
        var response = await _client.PostAsJsonAsync("/login", usuario);
        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        token.Should().NotBeNull();
        token!.AccessToken.Should().NotBeNullOrEmpty();
        token.TokenType.Should().Be("bearer");
        token.ExpiresIn.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Login_CredenciaisIncorretas_RetornaUnauthorized()
    {
        // Arrange
        var usuario = new Usuario { Username = "invalido", Password = "senhaerrada" };

        // Act
        var response = await _client.PostAsJsonAsync("/login", usuario);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HealthCheck_RetornaStatusOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}