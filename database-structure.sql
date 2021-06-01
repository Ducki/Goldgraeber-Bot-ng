

CREATE TABLE Triggers (
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	searchstring VARCHAR
);

CREATE TABLE Answers (
	id integer primary key autoincrement,
	trigger_id integer,
	answer varchar,
	foreign key (trigger_id) REFERENCES Triggers(id)

);
