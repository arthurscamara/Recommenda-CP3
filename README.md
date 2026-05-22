# Recommenda — CP3

API REST para descoberta musical. Evolucao do CP2 com documentacao Swagger, repositorio generico e tratamento global de erros.

---

## Integrantes

Nome: Arthur Câmara RM:562310

---

## Dominio

Plataforma de descoberta musical que permite cadastrar artistas, albuns, faixas e generos, alem de registrar avaliacoes de usuarios sobre albuns.

---

## SGBD

MySQL via Pomelo.EntityFrameworkCore.MySql.
A connection string e configurada em `appsettings.json` na chave `ConnectionStrings:RecommendaMySQL`.

---

## Como executar

### Pre-requisitos

- .NET 10 SDK
- MySQL em execucao local ou remoto
- Connection string configurada em `Recommenda.API/appsettings.json`

### Passos

```bash
# 1. Clone o repositorio
git clone <url-do-repositorio>
cd Recommenda

# 2. Configure a connection string
# Edite Recommenda.API/appsettings.json e preencha RecommendaMySQL

# 3. Execute a API (as migrations sao aplicadas automaticamente na inicializacao)
cd Recommenda.API
dotnet run
```

### Acessar o Swagger

Apos iniciar a API em modo Development, acesse:

```
http://localhost:5290/swagger
```

---

## Recursos expostos via HTTP

| Recurso | Metodos disponíveis | Repositorio |
|---------|--------------------|----|
| Artist | GET /api/artist, GET /api/artist/{id}, POST, DELETE | IArtistRepository (especifico) |
| Album | GET /api/album, GET /api/album/{id}, GET /api/album/artist/{id}, POST, DELETE | IAlbumRepository (especifico) |
| Track | GET /api/track/album/{id}, GET /api/track/{id}, POST, DELETE | ITrackRepository (especifico) |
| Genre | GET /api/genre, GET /api/genre/{id}, POST, DELETE | IRepository<Genre> (generico) |
| AlbumRating | GET /api/albumrating/album/{id}, GET /api/albumrating/user/{id}, POST | IAlbumRatingRepository (especifico) |

---

## Repositorio generico

`IRepository<T>` (Recommenda.Application/Repositories/IRepository.cs) define o contrato CRUD minimo para qualquer entidade que herde de `BaseEntity`:

- `GetAll()` — retorna todas as entidades ordenadas por data de criacao
- `GetById(Guid id)` — busca por identificador
- `Add(T entity)` — persiste a entidade
- `Delete(Guid id)` — remove; retorna false se nao encontrada
- `ExistsById(Guid id)` — verifica existencia sem trazer o objeto

`Repository<T>` (Recommenda.Infrastructure/Persistence/Repositories/Repository.cs) implementa essa interface com EF Core usando AsNoTracking em leituras.

Registro na DI:

```csharp
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

O GenreController demonstra o uso direto de IRepository<Genre> sem repositorio especifico.

---

## Mapeamento de excecoes para HTTP

| Excecao | Status HTTP | Titulo |
|---------|-------------|--------|
| ArgumentNullException | 400 | Requisicao invalida |
| ArgumentException | 400 | Requisicao invalida |
| DomainException (generica) | 400 | Nao foi possivel concluir a operacao |
| InvalidOperationException | 400 | Nao foi possivel concluir a operacao |
| ResourceNotFoundException | 404 | Recurso nao encontrado |
| KeyNotFoundException | 404 | Recurso nao encontrado |
| ConflictException | 409 | Conflito |
| UnauthorizedAccessException | 401 | Nao autorizado |
| Demais excecoes | 500 | Erro interno do servidor |

Todas as respostas de erro seguem RFC 7807 (application/problem+json).
Em producao, detalhes internos e stack trace nao sao expostos.

---

## Exemplos de chamada

### Criar artista

```bash
curl -X POST http://localhost:5290/api/artist \
  -H "Content-Type: application/json" \
  -d '{"name":"Criolo","bio":"Rapper brasileiro.","country":"Brasil"}'
```

### Listar albuns de um artista

```bash
curl http://localhost:5290/api/album/artist/{artistId}
```

### Criar genero (repositorio generico)

```bash
curl -X POST http://localhost:5290/api/genre \
  -H "Content-Type: application/json" \
  -d '{"name":"MPB","description":"Musica Popular Brasileira."}'
```

### Erro 404

```json
{
  "type": "about:blank",
  "title": "Recurso nao encontrado",
  "status": 404,
  "detail": "Artista com id '00000000-...' nao foi encontrado.",
  "instance": "/api/artist/00000000-..."
}
```

### Erro 409 — avaliacao duplicada

```json
{
  "type": "about:blank",
  "title": "Conflito",
  "status": 409,
  "detail": "Usuario ja avaliou este album.",
  "instance": "/api/albumrating"
}
```
