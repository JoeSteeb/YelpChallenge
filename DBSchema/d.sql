DROP DATABASE yelpdb;
CREATE DATABASE yelpdb;

\c yelpdb;
\i Trigger_RELATIONS_v2.sql;

\i ../SQLScripts/Trigger_UPDATE.sql;
\i ../SQLScripts/Trigger_TRIGGER.sql;
