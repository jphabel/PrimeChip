INSERT INTO Users (Email, Password)
VALUES ('admin@primechip.com', '1234');

UPDATE Users 
SET Password = '$2a$11$lY4rVjFuZQSYr./lOFZpp.5U2b1KR8lpQ85XpNmwlr2Sm9unCoVNC'
WHERE Email = 'admin@primechip.com'