# PropertyVisit Seeder Documentation

## Overview
This document explains how to use the PropertyVisit seeder that has been implemented for your RealEstate application.

## What's Included

### 1. Automatic Seeder (Recommended)
The PropertyVisit seeder has been integrated into the existing `DataSeeder` service in `Services/DataSeeder.cs`. It will automatically run when the application starts if:
- The PropertyVisits table is empty
- Properties and Users tables contain data
- There are at least 2 properties and 2 users available

### 2. Manual SQL Script
A manual SQL script is available at `Scripts/SeedPropertyVisits.sql` for direct database execution.

### 3. Data Verification Script
Use `Scripts/CheckExistingData.sql` to check your existing database before seeding.

## How to Use

### Option 1: Automatic Seeding (Recommended)
1. Ensure your application has Properties and Users in the database
2. Run your application normally
3. The seeder will automatically add 5 sample PropertyVisit records

### Option 2: Manual SQL Seeding
1. Run `Scripts/CheckExistingData.sql` to get your existing PropertyId and UserId values
2. Update the placeholder values in `Scripts/SeedPropertyVisits.sql`
3. Execute the SQL script in your database

## Sample Data Created

The seeder creates 5 PropertyVisit records with different statuses:
- **Pending**: 2 records (one with pickup service, one without)
- **Confirmed**: 1 record (broker accepted)
- **BrokerEnRoute**: 1 record (trip started)
- **Completed**: 1 record (with ratings and feedback)

## Features

### Smart Foreign Key Handling
- Automatically detects existing Properties and Users
- Uses actual database IDs to avoid foreign key constraint violations
- Graceful fallback if insufficient data exists

### Comprehensive Data
- All visit statuses represented
- Pickup service variations
- GPS coordinates for pickup locations
- Timestamps for all status transitions
- Client feedback and ratings for completed visits

### Logging
- Detailed logging for debugging
- Clear success/failure messages
- Warning messages for missing prerequisites

## Viewing the Data

Once seeded, you can view the PropertyVisit data by:
1. Navigate to `/PropertyVisit/Schedules` in your application
2. Use the Schedules view to see all visits with filtering options
3. Check the statistics cards for visit counts by status

## Troubleshooting

### Common Issues

**Seeder doesn't run:**
- Check if PropertyVisits table already has data
- Verify Properties and Users tables contain data
- Check application logs for seeder messages

**Foreign key errors:**
- Use `CheckExistingData.sql` to verify your data
- Ensure you have at least 2 properties and 2 active users
- The automatic seeder handles this gracefully

**No data in Schedules view:**
- Verify you're logged in as a user who is a client or broker in the seeded visits
- Check that the PropertyVisit records were created successfully
- Run the verification query in `SeedPropertyVisits.sql`

## Customization

To modify the seeded data:
1. Edit the `SeedPropertyVisits()` method in `Services/DataSeeder.cs`
2. Adjust the number of records, properties, or data values
3. Clear the PropertyVisits table to trigger reseeding

## Security Notes

- The seeder only runs if the table is empty to prevent duplicate data
- All seeded data uses realistic but fictional information
- GPS coordinates are for demonstration purposes only
