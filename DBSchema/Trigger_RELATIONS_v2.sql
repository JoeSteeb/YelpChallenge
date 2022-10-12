CREATE TABLE Users (
	user_id VARCHAR PRIMARY KEY,
	user_name VARCHAR,
	yelping_since TIMESTAMP,
	tip_count INTEGER DEFAULT 0,
	fan_count INTEGER,
	average_stars FLOAT,
	funny INTEGER,
	useful INTEGER,
	cool INTEGER,
	longitude FLOAT DEFAULT 0,
	latitude FLOAT DEFAULT 0,
	likes_count INTEGER DEFAULT 0
);
CREATE TABLE FriendedUser (
	user_id VARCHAR,
	friended_by_id VARCHAR,
	PRIMARY KEY (friended_by_id, user_id),
	FOREIGN KEY(friended_by_id) REFERENCES Users(user_id),
	FOREIGN KEY (user_id) REFERENCES Users(user_id)
);
CREATE TABLE Business (
	business_id VARCHAR PRIMARY KEY,
	business_name VARCHAR,
	business_address VARCHAR,
	business_state VARCHAR,
	city VARCHAR,
	zip INTEGER,
	longitude FLOAT,
	latitude FLOAT,
	stars DOUBLE PRECISION,
	is_open BOOLEAN,
	tip_count INTEGER DEFAULT 0,
	numCheckins INTEGER DEFAULT 0
);
CREATE TABLE Category (
	business_id VARCHAR,
	category VARCHAR,
	PRIMARY KEY (business_id, category),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);
CREATE TABLE Tip (
	user_id VARCHAR,
	business_id VARCHAR,
	date_posted TIMESTAMP,
	body VARCHAR,
	likes INTEGER DEFAULT 0,
	PRIMARY KEY (user_id, business_id, date_posted),
	FOREIGN KEY (user_id) REFERENCES Users(user_id),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);
CREATE TABLE CheckIn (
	business_id VARCHAR,
	checkin_date TIMESTAMP,
	PRIMARY KEY (business_id, checkin_date),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);
CREATE TABLE OperatingHours (
	business_id VARCHAR,
	day_of_the_week VARCHAR,
	open_time TIME,
	close_time TIME,
	PRIMARY KEY (business_id, day_of_the_week),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);
CREATE TABLE Attribute (
	business_id VARCHAR,
	attribute VARCHAR,
	val VARCHAR,
	PRIMARY KEY (business_id, attribute),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);
