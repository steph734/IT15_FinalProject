-- PropertyVisits Seeder Script
-- Run this script after checking your existing data with CheckExistingData.sql
-- Update the PropertyId, ClientId, and BrokerId values to match your actual data

-- First, run CheckExistingData.sql to get the correct IDs
-- Then replace the placeholder values below with your actual IDs

-- Sample PropertyVisit Records
-- NOTE: Replace PropertyId, ClientId, BrokerId with actual values from your database

-- Record 1: Pending visit with pickup service
SET IDENTITY_INSERT PropertyVisits ON;
INSERT INTO PropertyVisits (
    VisitId, PropertyId, ClientId, BrokerId, ScheduledDate, ScheduledTime,
    PickupService, PickupAddress, PickupLatitude, PickupLongitude,
    Status, CreatedAt, UpdatedAt
) VALUES (
    1, 
    1, -- Replace with actual PropertyId
    1, -- Replace with actual Client UserId
    2, -- Replace with actual Broker UserId
    '2024-05-15', 
    '10:00:00',
    1, 
    '123 Main St, City, State', 
    40.7128, 
    -74.0060,
    0, -- Pending
    '2024-05-14 10:00:00', 
    '2024-05-14 10:00:00'
);

-- Record 2: Confirmed visit without pickup service
INSERT INTO PropertyVisits (
    VisitId, PropertyId, ClientId, BrokerId, ScheduledDate, ScheduledTime,
    PickupService, Status, BrokerAcceptedAt, CreatedAt, UpdatedAt
) VALUES (
    2, 
    2, -- Replace with actual PropertyId
    3, -- Replace with actual Client UserId
    2, -- Replace with actual Broker UserId
    '2024-05-16', 
    '14:30:00',
    0, 
    1, -- Confirmed
    '2024-05-14 12:00:00',
    '2024-05-14 08:00:00', 
    '2024-05-14 12:00:00'
);

-- Record 3: Broker en route with pickup service
INSERT INTO PropertyVisits (
    VisitId, PropertyId, ClientId, BrokerId, ScheduledDate, ScheduledTime,
    PickupService, PickupAddress, PickupLatitude, PickupLongitude,
    Status, BrokerAcceptedAt, TripStartedAt, CreatedAt, UpdatedAt
) VALUES (
    3, 
    3, -- Replace with actual PropertyId
    4, -- Replace with actual Client UserId
    5, -- Replace with actual Broker UserId
    '2024-05-17', 
    '09:00:00',
    1, 
    '456 Oak Ave, Town, State', 
    40.7580, 
    -73.9855,
    2, -- BrokerEnRoute
    '2024-05-14 06:00:00',
    '2024-05-14 13:30:00',
    '2024-05-14 04:00:00', 
    '2024-05-14 13:30:00'
);

-- Record 4: Completed visit with feedback
INSERT INTO PropertyVisits (
    VisitId, PropertyId, ClientId, BrokerId, ScheduledDate, ScheduledTime,
    PickupService, Status, BrokerAcceptedAt, TripStartedAt, CompletedAt,
    PropertyRating, BrokerServiceRating, PickupExperienceRating, ClientFeedback,
    CreatedAt, UpdatedAt
) VALUES (
    4, 
    4, -- Replace with actual PropertyId
    6, -- Replace with actual Client UserId
    5, -- Replace with actual Broker UserId
    '2024-05-13', 
    '11:00:00',
    0, 
    7, -- Completed
    '2024-05-13 09:00:00',
    '2024-05-13 10:00:00',
    '2024-05-13 13:00:00',
    5, 4, 5,
    'Excellent service! The broker was very knowledgeable and professional.',
    '2024-05-12 10:00:00', 
    '2024-05-13 13:00:00'
);

-- Record 5: Another pending visit
INSERT INTO PropertyVisits (
    VisitId, PropertyId, ClientId, BrokerId, ScheduledDate, ScheduledTime,
    PickupService, PickupAddress, PickupLatitude, PickupLongitude,
    Status, CreatedAt, UpdatedAt
) VALUES (
    5, 
    5, -- Replace with actual PropertyId
    7, -- Replace with actual Client UserId
    8, -- Replace with actual Broker UserId
    '2024-05-18', 
    '15:00:00',
    1, 
    '789 Pine Rd, Village, State', 
    40.7489, 
    -73.9680,
    0, -- Pending
    '2024-05-14 11:00:00', 
    '2024-05-14 11:00:00'
);

SET IDENTITY_INSERT PropertyVisits OFF;

-- Verification Query
SELECT 
    v.VisitId,
    v.PropertyId,
    p.Title as PropertyTitle,
    v.ClientId,
    c.FullName as ClientName,
    v.BrokerId,
    b.FullName as BrokerName,
    v.ScheduledDate,
    v.ScheduledTime,
    v.Status,
    v.PickupService
FROM PropertyVisits v
LEFT JOIN Properties p ON v.PropertyId = p.PropertyId
LEFT JOIN Users c ON v.ClientId = c.UserId
LEFT JOIN Users b ON v.BrokerId = b.UserId
ORDER BY v.VisitId;
