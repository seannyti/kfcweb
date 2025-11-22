-- Update all existing users to be verified (so they aren't locked out)
UPDATE Users
SET EmailVerified = 1
WHERE EmailVerified = 0 OR EmailVerified IS NULL;

SELECT 
    COUNT(*) AS TotalUsers,
    SUM(CASE WHEN EmailVerified = 1 THEN 1 ELSE 0 END) AS VerifiedUsers,
    SUM(CASE WHEN EmailVerified = 0 THEN 1 ELSE 0 END) AS UnverifiedUsers
FROM Users;
