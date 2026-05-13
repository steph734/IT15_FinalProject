-- Script to check existing Properties and Users for PropertyVisit seed data
-- Run this script in your database to get the actual IDs you need to use

-- Check existing Properties
SELECT 
    PropertyId,
    Title,
    Location,
    Price,
    Status
FROM Properties
ORDER BY PropertyId;

-- Check existing Users with their roles
SELECT 
    UserId,
    FullName,
    Email,
    Role,
    IsActive
FROM Users
WHERE IsActive = 1
ORDER BY Role, UserId;

-- Check Users with Broker role specifically
SELECT 
    UserId,
    FullName,
    Email,
    Role
FROM Users
WHERE Role = 'broker' AND IsActive = 1
ORDER BY UserId;

-- Check Users with Client role specifically
SELECT 
    UserId,
    FullName,
    Email,
    Role
FROM Users
WHERE Role = 'client' AND IsActive = 1
ORDER BY UserId;

-- Check if there are any existing PropertyVisits
SELECT 
    VisitId,
    PropertyId,
    ClientId,
    BrokerId,
    ScheduledDate,
    Status
FROM PropertyVisits
ORDER BY VisitId;
