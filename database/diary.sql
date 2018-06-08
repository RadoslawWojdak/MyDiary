DROP SCHEMA diary;
CREATE SCHEMA diary;

DROP TABLE users;
CREATE TABLE users
(
	id INT UNSIGNED NOT NULL UNIQUE PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(24) NOT NULL UNIQUE,
    password VARCHAR(24) NOT NULL,
    email VARCHAR(45) NOT NULL UNIQUE
);

DROP TABLE diaries;
CREATE TABLE diaries
(
	id INT UNSIGNED NOT NULL UNIQUE PRIMARY KEY AUTO_INCREMENT,
    users_id INT UNSIGNED NOT NULL,
    name VARCHAR(45) NOT NULL,
    description TINYTEXT,
    
    FOREIGN KEY (users_id) REFERENCES users(id)
);

DROP TABLE notes;
CREATE TABLE notes
(
	id INT UNSIGNED NOT NULL UNIQUE PRIMARY KEY AUTO_INCREMENT,
    diaries_id INT UNSIGNED NOT NULL,
    title VARCHAR(45),
    text LONGTEXT,
    creation_date DATETIME NOT NULL,
    modification_date DATETIME NOT NULL,
    
    FOREIGN KEY (diaries_id) REFERENCES diaries(id)
);

DROP TABLE photos;
CREATE TABLE photos
(
	id INT UNSIGNED NOT NULL UNIQUE PRIMARY KEY AUTO_INCREMENT,
    diaries_id INT UNSIGNED NOT NULL,
    image LONGBLOB NOT NULL,
    description TINYTEXT,
    insertion_date DATETIME NOT NULL,
    event_date DATETIME,
    
    FOREIGN KEY (diaries_id) REFERENCES diaries(id)
);

DROP TABLE tags;
CREATE TABLE tags
(
	id INT UNSIGNED NOT NULL UNIQUE PRIMARY KEY AUTO_INCREMENT,
    text VARCHAR(35) NOT NULL UNIQUE
);

#MANY-TO-MANY RELATIONSHIPS
DROP TABLE diaries_tags;
CREATE TABLE diaries_tags
(
	diaries_id INT UNSIGNED NOT NULL,
    tags_id INT UNSIGNED NOT NULL,
    
    FOREIGN KEY (diaries_id) REFERENCES diaries(id),
    FOREIGN KEY (tags_id) REFERENCES tags(id)
);

DROP TABLE notes_tags;
CREATE TABLE notes_tags
(
	notes_id INT UNSIGNED NOT NULL,
    tags_id INT UNSIGNED NOT NULL,
    
    FOREIGN KEY (notes_id) REFERENCES notes(id),
    FOREIGN KEY (tags_id) REFERENCES tags(id)
);

DROP TABLE photos_tags;
CREATE TABLE photos_tags
(
	photos_id INT UNSIGNED NOT NULL,
    tags_id INT UNSIGNED NOT NULL,
    
    FOREIGN KEY (photos_id) REFERENCES photos(id),
    FOREIGN KEY (tags_id) REFERENCES tags(id)
);