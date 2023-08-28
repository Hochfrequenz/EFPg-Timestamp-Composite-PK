# EntityFramework-DateTime.MinValue-In-Composite-PK-Bug
Demonstrates a bug in Entity Framework that occurs when a Datetime column is used as part of a composite primary key and entries with a value near 0 cannot be updated

## How to run?
Start the database in docker: 
```bash
docker-compose up -d
```
Make sure the port 5432 is not allocated.
If it is allocated, change to a free port in [`docker-compose.yml`](docker-compose.yml) and the Application Context [`UseNpgsql` arg](MySolution/DataModelAndMigration/ApplicationContext.cs#L16).
