from fastapi import FastAPI, HTTPException, Depends, status
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from pydantic import BaseModel, Field
from typing import List, Optional
from datetime import datetime, timedelta
from jose import JWTError, jwt
from passlib.context import CryptContext
from dotenv import load_dotenv
import os

load_dotenv()

# Configurações JWT
SECRET_KEY = os.getenv("SECRET_KEY", "minha-chave-secreta-temporaria-mude-em-producao")
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 30

# Configuração de segurança
security = HTTPBearer()
pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")

# Modelos de dados
class Usuario(BaseModel):
    username: str
    password: str

class UsuarioDB(BaseModel):
    username: str
    hashed_password: str

class Token(BaseModel):
    access_token: str
    token_type: str
    expires_in: int

class Tarefa(BaseModel):
    id: Optional[int] = None
    titulo: str = Field(..., min_length=1, max_length=100)
    descricao: str = Field(..., max_length=500)
    concluida: bool = False
    usuario: str

class TarefaCreate(BaseModel):
    titulo: str = Field(..., min_length=1, max_length=100)
    descricao: str = Field(..., max_length=500)

class TarefaUpdate(BaseModel):
    titulo: Optional[str] = Field(None, min_length=1, max_length=100)
    descricao: Optional[str] = Field(None, max_length=500)
    concluida: Optional[bool] = None

# Inicializa o app
app = FastAPI(
    title="API com Autenticação JWT",
    description="API de tarefas com autenticação e testes",
    version="2.0.0",
    docs_url="/docs"
)

# Banco de dados simulado
usuarios_db = {}  # username -> UsuarioDB
tarefas_db = []   # Lista de Tarefas
contador_id = 1

# Funções auxiliares
def verificar_senha(plain_password, hashed_password):
    return pwd_context.verify(plain_password, hashed_password)

def hash_senha(password):
    return pwd_context.hash(password)

def criar_token_acesso(data: dict, expires_delta: Optional[timedelta] = None):
    to_encode = data.copy()
    if expires_delta:
        expire = datetime.utcnow() + expires_delta
    else:
        expire = datetime.utcnow() + timedelta(minutes=15)
    to_encode.update({"exp": expire})
    encoded_jwt = jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)
    return encoded_jwt

async def get_current_user(credentials: HTTPAuthorizationCredentials = Depends(security)):
    token = credentials.credentials
    credentials_exception = HTTPException(
        status_code=status.HTTP_401_UNAUTHORIZED,
        detail="Credenciais inválidas",
        headers={"WWW-Authenticate": "Bearer"},
    )
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        username: str = payload.get("sub")
        if username is None:
            raise credentials_exception
    except JWTError:
        raise credentials_exception
    
    if username not in usuarios_db:
        raise credentials_exception
    
    return username

# ------------------- ENDPOINTS DE AUTENTICAÇÃO -------------------

@app.post("/registrar", status_code=201, tags=["Autenticação"])
def registrar(usuario: Usuario):
    """Registra um novo usuário"""
    if usuario.username in usuarios_db:
        raise HTTPException(status_code=400, detail="Usuário já existe")
    
    hashed = hash_senha(usuario.password)
    usuarios_db[usuario.username] = UsuarioDB(
        username=usuario.username,
        hashed_password=hashed
    )
    return {"mensagem": "Usuário criado com sucesso"}

@app.post("/login", response_model=Token, tags=["Autenticação"])
def login(usuario: Usuario):
    """Faz login e retorna token JWT"""
    if usuario.username not in usuarios_db:
        raise HTTPException(status_code=401, detail="Usuário ou senha incorretos")
    
    user_db = usuarios_db[usuario.username]
    if not verificar_senha(usuario.password, user_db.hashed_password):
        raise HTTPException(status_code=401, detail="Usuário ou senha incorretos")
    
    access_token_expires = timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    access_token = criar_token_acesso(
        data={"sub": usuario.username}, expires_delta=access_token_expires
    )
    
    return {
        "access_token": access_token,
        "token_type": "bearer",
        "expires_in": ACCESS_TOKEN_EXPIRE_MINUTES * 60
    }

# ------------------- ENDPOINTS DE TAREFAS (protegidos) -------------------

@app.post("/tarefas", response_model=Tarefa, status_code=201, tags=["Tarefas"])
def criar_tarefa(tarefa: TarefaCreate, current_user: str = Depends(get_current_user)):
    """Cria uma nova tarefa (requer autenticação)"""
    global contador_id
    nova_tarefa = Tarefa(
        id=contador_id,
        titulo=tarefa.titulo,
        descricao=tarefa.descricao,
        concluida=False,
        usuario=current_user
    )
    tarefas_db.append(nova_tarefa)
    contador_id += 1
    return nova_tarefa

@app.get("/tarefas", response_model=List[Tarefa], tags=["Tarefas"])
def listar_tarefas(
    concluida: Optional[bool] = None,
    current_user: str = Depends(get_current_user)
):
    """Lista tarefas do usuário autenticado"""
    user_tarefas = [t for t in tarefas_db if t.usuario == current_user]
    
    if concluida is not None:
        return [t for t in user_tarefas if t.concluida == concluida]
    return user_tarefas

@app.get("/tarefas/{tarefa_id}", response_model=Tarefa, tags=["Tarefas"])
def buscar_tarefa(tarefa_id: int, current_user: str = Depends(get_current_user)):
    """Busca uma tarefa específica (requer autenticação)"""
    for tarefa in tarefas_db:
        if tarefa.id == tarefa_id and tarefa.usuario == current_user:
            return tarefa
    raise HTTPException(status_code=404, detail="Tarefa não encontrada")

@app.put("/tarefas/{tarefa_id}", response_model=Tarefa, tags=["Tarefas"])
def atualizar_tarefa(
    tarefa_id: int,
    tarefa_update: TarefaUpdate,
    current_user: str = Depends(get_current_user)
):
    """Atualiza uma tarefa (requer autenticação)"""
    for i, tarefa in enumerate(tarefas_db):
        if tarefa.id == tarefa_id and tarefa.usuario == current_user:
            # Atualiza apenas campos fornecidos
            update_data = tarefa_update.dict(exclude_unset=True)
            for field, value in update_data.items():
                setattr(tarefas_db[i], field, value)
            return tarefas_db[i]
    
    raise HTTPException(status_code=404, detail="Tarefa não encontrada")

@app.delete("/tarefas/{tarefa_id}", status_code=204, tags=["Tarefas"])
def deletar_tarefa(tarefa_id: int, current_user: str = Depends(get_current_user)):
    """Remove uma tarefa (requer autenticação)"""
    for i, tarefa in enumerate(tarefas_db):
        if tarefa.id == tarefa_id and tarefa.usuario == current_user:
            tarefas_db.pop(i)
            return
    
    raise HTTPException(status_code=404, detail="Tarefa não encontrada")

# Endpoint público para healthcheck
@app.get("/health", tags=["Root"])
def health_check():
    """Verifica se a API está funcionando"""
    return {"status": "ok", "usuarios_registrados": len(usuarios_db)}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000, reload=True)