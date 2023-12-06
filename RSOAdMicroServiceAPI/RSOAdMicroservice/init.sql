--DROP TABLE IF EXISTS advertisers;

--CREATE TABLE advertisers (
--  user_id SERIAL PRIMARY KEY
--);

--INSERT INTO advertisers (user_id) VALUES (1);
--INSERT INTO advertisers (user_id) VALUES (2);

--DROP TABLE IF EXISTS ads;

--CREATE TABLE ads (
--  "ID" SERIAL PRIMARY KEY,
--  user_id INT NOT NULL,
--  "Thing" VARCHAR NOT NULL,
--  "Price" INTEGER,
--  "Category" VARCHAR,
--  "PostTime" TIMESTAMP,
--  FOREIGN KEY (user_id) REFERENCES advertisers (user_id)
--);

--INSERT INTO ads (user_id, "Thing", "Price", "Category", "PostTime") VALUES (1, 'iPhone 6', 100, 'Electronics', '2016-01-01 00:00:00');

CREATE TABLE Ads (
  "ID" SERIAL PRIMARY KEY,
  "UserId" INT NOT NULL,
  "Thing" VARCHAR NOT NULL,
  "Price" INTEGER,
  "Category" VARCHAR,
  "PostTime" TIMESTAMP
);

INSERT INTO Ads ("UserId", "Thing", "Price", "Category", "PostTime") VALUES (1, 'iPhone 6', 100, 'Electronics', '2016-01-01 00:00:00');