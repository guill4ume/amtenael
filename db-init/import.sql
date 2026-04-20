LOAD DATA INFILE '/docker-entrypoint-initdb.d/serverproperty.csv'
REPLACE INTO TABLE serverproperty
FIELDS TERMINATED BY '\t'
LINES TERMINATED BY '\r\n'
IGNORE 2 LINES
(Category, `Key`, Description, DefaultValue, Value, LastTimeRowUpdated, ServerProperty_ID);
