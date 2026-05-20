using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ApiTarefas.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiTarefas.Tests;

public class TarefasTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private string _authToken = string.Empty;

    public TarefasTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        SetupAuth().Wait();
    }

    private async Task SetupAuth()
    {
        var username = $"tarefasuser_{Guid.NewGuid()}";
        var usuario = new Usuario { Username = username, Password = "tarefas123" };
        
        await _client.PostAsJsonAsync("/registrar", usuario);
        
        var loginResponse = await _client.PostAsJsonAsync("/login", usuario);
        var token = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
        
        _authToken = token?.AccessToken ?? string.Empty;
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _authToken);
    }

    [Fact]
    public async Task CriarTarefa_ComTokenValido_RetornaSucesso()
    {
        // Arrange
        var novaTarefa = new TarefaCreate
        {
            Titulo = "Estudar ASP.NET Core",
            Descricao = "Aprender Minimal APIs"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/tarefas", novaTarefa);
        var tarefa = await response.Content.ReadFromJsonAsync<Tarefa>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        tarefa.Should().NotBeNull();
        tarefa!.Titulo.Should().Be("Estudar ASP.NET Core");
        tarefa.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ListarTarefas_ComTokenValido_RetornaLista()
    {
        // Act
        var response = await _client.GetAsync("/tarefas");
        var tarefas = await response.Content.ReadFromJsonAsync<List<Tarefa>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        tarefas.Should().NotBeNull();
    }

    [Fact]
    public async Task AtualizarTarefa_ComDadosValidos_AtualizaCorretamente()
    {
        // Arrange
        // Primeiro cria uma tarefa
        var criarResponse = await _client.PostAsJsonAsync("/tarefas", 
            new TarefaCreate { Titulo = "Original", Descricao = "Desc original" });
        var tarefaCriada = await criarResponse.Content.ReadFromJsonAsync<Tarefa>();
        
        var atualizacao = new TarefaUpdate 
        { 
            Titulo = "Atualizado", 
            Concluida = true 
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/tarefas/{tarefaCriada!.Id}", atualizacao);
        var tarefaAtualizada = await response.Content.ReadFromJsonAsync<Tarefa>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        tarefaAtualizada!.Titulo.Should().Be("Atualizado");
        tarefaAtualizada.Concluida.Should().BeTrue();
    }

    [Fact]
    public async Task DeletarTarefa_RemoveTarefaCorretamente()
    {
        // Arrange
        var criarResponse = await _client.PostAsJsonAsync("/tarefas", 
            new TarefaCreate { Titulo = "Para Deletar", Descricao = "Será removida" });
        var tarefa = await criarResponse.Content.ReadFromJsonAsync<Tarefa>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/tarefas/{tarefa!.Id}");
        
        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verifica que foi deletada
        var getResponse = await _client.GetAsync($"/tarefas/{tarefa.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AcessarEndpointSemToken_RetornaUnauthorized()
    {
        // Arrange
        var clientSemToken = new HttpClient();
        
        // Act
        var response = await clientSemToken.GetAsync("/tarefas");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task TokenInvalido_RetornaUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", "token_invalido_123");
        
        // Act
        var response = await _client.GetAsync("/tarefas");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}