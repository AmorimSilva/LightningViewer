# LightningViewer

## Deploy com Docker na VPS

O Compose sobe a aplicação ASP.NET Core e um PostgreSQL 16 persistente. A aplicação
fica disponível na porta `2080`, executa as migrations automaticamente e reinicia
com a VPS.

### 1. Configure as variáveis

Na raiz do projeto:

```bash
cp .env.example .env
nano .env
```

Defina uma senha forte para o PostgreSQL e informe a `Consumer Key` e a
`Consumer Secret` da API EUMETSAT.

Opcionalmente, coloque o CSV das unidades em:

```text
data/unidades.csv
```

O CSV deve usar `;` como separador. Se ele não existir, a aplicação sobe
normalmente com a lista de unidades vazia.

### 2. Suba os containers

```bash
docker compose up -d --build
```

Acesse:

```text
http://IP_DA_VPS:2080
```

Se a VPS usa firewall:

```bash
sudo ufw allow 2080/tcp
```

### Operação

```bash
# Ver status e healthchecks
docker compose ps

# Acompanhar os logs
docker compose logs -f web

# Atualizar após baixar uma nova versão
docker compose up -d --build

# Parar sem apagar o banco
docker compose down
```

O banco fica no volume Docker `lightning-viewer_postgres_data`. Não execute
`docker compose down -v` em produção, pois a opção `-v` remove os dados.

