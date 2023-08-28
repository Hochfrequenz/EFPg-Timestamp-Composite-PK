# EntityFramework Cannot Distinguish PostgreSQL `0001-01-01 00:00:00.000000 +00:00` from `-infinity` in Key Columns

This repository contains demo code for a bug in Entity Framework that occurs when a timestamp column is used as part of a composite primary key.

## What's the problem?

There are multiple _different_ SQL values of (at least) type `timestamp with time zone` which are mapped to the _same_ C# value.

| SQL value                           | C# value                  |
| ----------------------------------- | ------------------------- |
| `-infinity`                         | `DateTimeOffset.MinValue` |
| `0001-01-01 00:00:00.000000 +00:00` | `DateTimeOffset.MinValue` |

Hence, the mapping is not injective and not bi-unique so that the mapping is also not reversible.
This causes problems when the timestamp column is used as part of a primary key.
Entries where the PK column holds the value `0001-01-01 00:00:00.000000 +00:00` can be `SELECT`ed but not `UPDATE`d although from a ORM user perspective the look the same.

The error message says:

> The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded.

## How to reproduce?

This repository does the following:

- There is minimal working example of the scenario described above, an ORM-annotated class [`MyModel`](MySolution/DataModelAndMigration/MyModel.cs) with a composite primary key consisting of a `Guid` and a `DateTimeOffset`.
- There is a (boring) test that shows that we can update models just fine, if we create and INSERT them via C#/the ORM: [`MySolution/IntegrationTests/UpdatModelsCreatedInCSharpTests.cs`](MySolution/IntegrationTests/UpdateModelsCreatedInCSharpTests.cs)
- There is a test to reproduce the issue that uses the same model class but
  - creates the data using [raw SQL](MySolution/IntegrationTests/upsert_example_data.sql) first
  - then shows that from a EF user perspective the data look the same (the values named above are indistinguishable)
  - then tries to update the records and shows that this fails for those that originally did not use `-infinity`.

### Start the Postgres database in docker

```bash
docker-compose up -d
```

Make sure the port 5432 is not allocated.
If it is allocated, change to a free port in [`docker-compose.yml`](docker-compose.yml) and the Application
Context [`UseNpgsql` arg](MySolution/DataModelAndMigration/ApplicationContext.cs#L16).

### Run the integration tests

In either your ide or using

```bash
dotnet test MySolution.sln --filter FullyQualifiedName~MySolution.IntegrationTests
```

See the [`.github/workflows/integrationtests.yml`](.github/workflows/integrationtests.yml) for a full example from build to test.

### How does the DB look like?

The only relevant table is the `MyModels` table which is created by the migration:

```sql
create table public."MyModels"
(
    "GuidPartOfKey" uuid                     not null,
    "DatePartOfKey" timestamp with time zone not null,
    "SomeValue"     text                     not null,
    constraint "PK_MyModels"
        primary key ("GuidPartOfKey", "DatePartOfKey")
)
```
