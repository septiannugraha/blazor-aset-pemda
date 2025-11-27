# ClickHouse Analytics Database

Simple ClickHouse setup for read-heavy analytics queries.

## Setup

1. Copy the example environment file:

   ```bash
   cp .env.example .env
   ```

1. Edit `.env` with your credentials

1. Start the container:

   ```bash
   docker-compose up -d
   ```

## Access

| Interface | URL | Credentials |
|-----------|-----|-------------|
| HTTP API | http://localhost:18123 | See .env file |
| Native | localhost:19000 | See .env file |

## Connect via CLI

```bash
docker exec -it clickhouse clickhouse-client --user $CLICKHOUSE_USER --password $CLICKHOUSE_PASSWORD
```

## Test Query

```sql
SELECT version();
SELECT 'Hello ClickHouse!';
```

## Stop

```bash
docker-compose down
```
