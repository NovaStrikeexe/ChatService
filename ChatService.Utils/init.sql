CREATE TABLE IF NOT EXISTS messages (
    id int PRIMARY KEY,
    content VARCHAR(128) NOT NULL,
    date TIMESTAMPTZ NOT NULL
);