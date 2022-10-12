-- Triggers

CREATE OR REPLACE FUNCTION updateTipCount() RETURNS trigger AS'
	BEGIN
	UPDATE Users 
	SET tip_count = tip_count+1 
	WHERE user_id = NEW.user_id;
	UPDATE Business
	SET tip_count = tip_count+1 
	WHERE business_id = NEW.business_id;
	RETURN NEW;
	END
' LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION updateCheckInCount() RETURNS trigger AS'
	BEGIN
	UPDATE Business 
	SET numCheckins = numCheckins+1 
	WHERE business_id = NEW.business_id;
	RETURN NEW;
	END
' LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION updateLikeCount() RETURNS trigger AS'
	BEGIN
	UPDATE Users 
	SET likes_count = likes_count + (NEW.likes - OLD.likes)
	WHERE user_id = NEW.user_id;
	RETURN NEW;
	END
' LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION insertLikeCount() RETURNS trigger AS'
	BEGIN
	UPDATE Users 
	SET likes_count = likes_count+NEW.likes
	WHERE user_id = NEW.user_id;
	RETURN NEW;
	END
' LANGUAGE plpgsql;

-- CREATE OR REPLACE FUNCTION checkUser(userid IN VARCHAR, name IN VARCHAR) RETURNS VARCHAR AS'
-- 	BEGIN
-- 	IF EXISTS (SELECT user_id, user_name FROM Users WHERE userid = user_id AND user_name = name)
-- 	THEN
-- 		RETURN ''true'';
-- 	else
-- 		RETURN ''false'';
-- 	end if;
-- 	END
-- ' LANGUAGE plpgsql;

-- CREATE OR REPLACE FUNCTION checkUser(userid IN VARCHAR, name IN VARCHAR) RETURNS VARCHAR AS'
-- 	BEGIN
-- 	IF EXISTS (SELECT user_id, user_name FROM Users WHERE userid = user_id AND user_name = name)
-- 	THEN
-- 		UPDATE Users
-- 		SET user_id = userid
-- 		WHERE user_id = userid;
		
-- 		RETURN ''true'';
-- 	else
-- 		RETURN ''false'';
-- 	end if;
-- 	END
-- ' LANGUAGE plpgsql;


