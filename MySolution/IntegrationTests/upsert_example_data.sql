-- This file is used to populate our database with some "special case" models.
-- The structure is always to first try updating/resetting an existing value, if it already exists.
-- And if it does not exist, then insert it. So it should both work on first executing, and on subsequent executions of the tests.
WITH upd AS (
    UPDATE testdb.public."MyModels"
        SET "SomeValue" = 'foo',
            "DatePartOfKey" = '0001-01-01 00:00:00.000000 +00:00' -- _not_ the same as '-infinity' but also mapped to DateTimeOffset.MinValue on C# side
        WHERE "GuidPartOfKey" = '5ed0b229-4ab6-45fc-94c1-f7e6c69f9857'
        RETURNING *)
INSERT
INTO testdb.public."MyModels" ("GuidPartOfKey", "DatePartOfKey", "SomeValue")
SELECT '5ed0b229-4ab6-45fc-94c1-f7e6c69f9857', '0001-01-01 00:00:00.000000 +00:00', 'foo'
WHERE NOT EXISTS(SELECT 1 FROM upd);
