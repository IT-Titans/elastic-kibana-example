# elastic-kibana-example

A Docker Compose file that starts a single-node Elasticsearch cluster and Kibana on a local machine, with TLS on the Elasticsearch API and passwords on the built-in users. A small .NET console program shows how to index, read and search one document with the official Elasticsearch .NET client. The repository is demonstration material for getting a local stack running. It is not a deployment template.

**Status:** Frozen. The repository was brought to a final state and archived; it receives no further updates, including security updates. Last verified: 2026-11-13 with Docker Compose v5.1.3 (`docker compose config`) and .NET SDK 10.0.404 (`dotnet build`, `dotnet run` without a cluster). The containers were not started for that verification, so the steps from `docker compose up` onwards are documented but were not run on that date.

## Prerequisites

- Docker with the Compose plugin (`docker compose`). Elastic asks for at least 4 GB of memory allocated to Docker Desktop for this kind of setup.
- `curl`, for the check in step 4.
- .NET SDK 10, only for the optional sample client.

The pinned versions are Elasticsearch and Kibana 9.5.6 (`STACK_VERSION` in `.env.example`), `Elastic.Clients.Elasticsearch` 9.5.3 and .NET 10.

## Quick start

The commands work in a POSIX shell and in PowerShell 7. Run them in the repository root.

1. Create the environment file from the demo defaults.

   ```sh
   cp .env.example .env
   ```

2. Start the stack.

   ```sh
   docker compose up -d
   ```

3. Wait until `es01` and `kibana` are reported as `healthy`. The first start pulls two images, creates a certificate authority and sets the Kibana service password, which takes a few minutes.

   ```sh
   docker compose ps
   ```

4. Copy the CA certificate out of the stack and query Elasticsearch over TLS.

   ```sh
   docker compose cp es01:/usr/share/elasticsearch/config/certs/ca/ca.crt ca.crt
   curl --cacert ca.crt -u elastic https://localhost:9200
   ```

   `curl` asks for the password. It is the value of `ELASTIC_PASSWORD` in `.env`, which is the demo default `changeme` unless you changed it. The answer is a JSON document with the cluster name and version.

5. Open Kibana at <http://localhost:5601> and log in as `elastic` with the same password.

6. Stop the stack when you are done.

   ```sh
   docker compose down
   ```

   `docker compose down -v` also removes the volumes, including the indexed data and the certificates.

### Sample client (optional)

With the stack running, set `ELASTIC_PASSWORD` to the value from `.env` and start the console program. With the demo default, in a POSIX shell:

```sh
ELASTIC_PASSWORD=changeme dotnet run --project main
```

In PowerShell:

```powershell
$env:ELASTIC_PASSWORD = "changeme"; dotnet run --project main
```

The program indexes one document into `my-tweet-index`, reads it back by ID and searches for it. It prints a confirmation for the index call, the message text of the document and a line starting with `query:`.

## Configuration

All settings are read from `.env`. The file is ignored by Git; `.env.example` holds the demo defaults.

| Variable | Demo default | Meaning |
|---|---|---|
| `STACK_VERSION` | `9.5.6` | Image tag for Elasticsearch and Kibana |
| `CLUSTER_NAME` | `docker-cluster` | Name of the Elasticsearch cluster |
| `LICENSE` | `basic` | Elastic license type; `trial` starts the 30-day trial |
| `ELASTIC_PASSWORD` | `changeme` | Password of the built-in `elastic` user |
| `ES_PORT` | `127.0.0.1:9200` | Host address and port for the Elasticsearch API |
| `KIBANA_PASSWORD` | `changeme` | Password of the built-in `kibana_system` user that Kibana uses internally |
| `KIBANA_PORT` | `127.0.0.1:5601` | Host address and port for Kibana |
| `ENCRYPTION_KEY` | a fixed demo string | Key Kibana uses to encrypt sessions, saved objects and reports; 32 characters or more |

Elastic requires passwords of at least six alphanumeric characters. The passwords are applied when the volumes are first created. To change them afterwards, remove the volumes with `docker compose down -v` and start again.

The sample client always connects to `https://localhost:9200` as `elastic`. Only the password is configurable, through the `ELASTIC_PASSWORD` environment variable.

## Known limitations

- The cluster has one node and no memory limits. The certificate authority is created on first start and lives in the `certs` volume.
- The sample client searches immediately after indexing, without a refresh. On the first run the `query:` line can therefore be empty; a second run finds the document.
- `main/Dockerfile` builds an image of the sample client. It is not referenced by `docker-compose.yml` and was not built for the last verification.

## Origin of the material

- `docker-compose.yml` is derived from [elkninja/elastic-stack-docker-part-one](https://github.com/elkninja/elastic-stack-docker-part-one) (Apache-2.0), the companion repository of the Elastic blog post [Getting started with the Elastic Stack and Docker Compose: Part 1](https://www.elastic.co/blog/getting-started-with-the-elastic-stack-and-docker-compose). It was reduced to the setup, Elasticsearch and Kibana services.
- `main/Program.cs` follows the CRUD usage examples in the documentation of the [Elasticsearch .NET client](https://github.com/elastic/elasticsearch-net) (Apache-2.0).

## Security

This setup is for local demonstration only.

- The passwords and the encryption key in `.env.example` are public demo values. With them, keep the ports bound to `127.0.0.1` as in the defaults, and set your own values before exposing the stack to any other host.
- The sample client accepts every server certificate (`ServerCertificateValidationCallback` always returns `true`), so its connection is encrypted but the server is not authenticated. Do not copy this into other code. Trust the CA certificate from step 4 instead.
- The repository is frozen. The pinned versions will fall behind and will not be updated here.

To report a security concern, see the [security policy](https://github.com/IT-Titans/elastic-kibana-example/security/policy).
