-- This file is used to populate our database with some "special case" models.
-- The structure is always to first try updating/resetting an existing value, if it already exists.
-- And if it does not exist, then insert it. So it should both work on first executing, and on subsequent executions of the tests.
WITH upd AS (
    UPDATE testdb.public."MyModels"
        SET "SomeValue" = 'foo',
            "DatePartOfKey" = '0001-01-01 00:00:00.000000 +00:00' -- _not_ the same as '-infinity' but also mapped to DateTimeOffset.MinValue on C# side; cannot be updated
        WHERE "GuidPartOfKey" = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'
        RETURNING *)
INSERT
INTO testdb.public."MyModels" ("GuidPartOfKey", "DatePartOfKey", "SomeValue")
SELECT 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '0001-01-01 00:00:00.000000 +00:00', 'foo'
WHERE NOT EXISTS(SELECT 1 FROM upd);

WITH upd AS (
    UPDATE testdb.public."MyModels"
        SET "SomeValue" = 'foo',
            "DatePartOfKey" = '-infinity' -- mapped to DateTimeOffset.MinValue on C# side; Can be updated.
        WHERE "GuidPartOfKey" = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'
        RETURNING *)
INSERT
INTO testdb.public."MyModels" ("GuidPartOfKey", "DatePartOfKey", "SomeValue")
SELECT 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '0001-01-01 00:00:00.000000 +00:00', 'foo'
WHERE NOT EXISTS(SELECT 1 FROM upd);
