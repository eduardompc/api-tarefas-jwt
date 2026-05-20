import pytest
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)

def test_registrar_usuario():
    """Testa registro de novo usuário"""
    response = client.post("/registrar", json={
        "username": "testuser",
        "password": "test123"
    })
    assert response.status_code == 201
    assert response.json()["mensagem"] == "Usuário criado com sucesso"

def test_registrar_usuario_duplicado():
    """Testa registro com usuário já existente"""
    # Primeiro registro
    client.post("/registrar", json={
        "username": "duplicado",
        "password": "senha123"
    })
    
    # Tentativa de registro duplicado
    response = client.post("/registrar", json={
        "username": "duplicado",
        "password": "outrasenha"
    })
    assert response.status_code == 400
    assert "Usuário já existe" in response.json()["detail"]

def test_login_sucesso():
    """Testa login com credenciais corretas"""
    # Registra usuário primeiro
    client.post("/registrar", json={
        "username": "logintest",
        "password": "senha123"
    })
    
    # Tenta login
    response = client.post("/login", json={
        "username": "logintest",
        "password": "senha123"
    })
    assert response.status_code == 200
    assert "access_token" in response.json()
    assert response.json()["token_type"] == "bearer"

def test_login_falha():
    """Testa login com credenciais incorretas"""
    response = client.post("/login", json={
        "username": "usuario_inexistente",
        "password": "senhaerrada"
    })
    assert response.status_code == 401

def test_health_check():
    """Testa endpoint de health check"""
    response = client.get("/health")
    assert response.status_code == 200
    assert response.json()["status"] == "ok"