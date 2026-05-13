BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260510082627_SeedPropertiesAndSellers'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'SellerId', N'IdentityVerified', N'Rating', N'SellerName', N'UserId') AND [object_id] = OBJECT_ID(N'[Sellers]'))
        SET IDENTITY_INSERT [Sellers] ON;
    EXEC(N'INSERT INTO [Sellers] ([SellerId], [IdentityVerified], [Rating], [SellerName], [UserId])
    VALUES (1, CAST(1 AS bit), 5, N''System Seller'', 3)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'SellerId', N'IdentityVerified', N'Rating', N'SellerName', N'UserId') AND [object_id] = OBJECT_ID(N'[Sellers]'))
        SET IDENTITY_INSERT [Sellers] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260510082627_SeedPropertiesAndSellers'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PropertyId', N'Accommodation', N'AgentId', N'AmenitiesVerified', N'ApprovedBy', N'ApproverUserId', N'AreaVerified', N'AuditLogJson', N'BasePrice', N'Bathrooms', N'Bedrooms', N'ContactMethod', N'ContactNotes', N'CoverImage', N'CreatedAt', N'DealArrangementDate', N'DealArrangementScheduled', N'DecisionAt', N'Description', N'DocumentNotes', N'DocumentUrlsJson', N'DocumentsReceived', N'DocumentsReceivedAt', N'DocumentsVerified', N'DocumentsVerifiedAt', N'EmployeeId', N'ExpectedCompletionDate', N'ExpectedTimeframeDays', N'FinalPrice', N'FinalReviewNotes', N'InspectionCompleted', N'InspectionCompletedAt', N'InspectionNotes', N'InspectionResult', N'InspectionScheduled', N'InspectionScheduledDate', N'IsApproved', N'IsRejected', N'Latitude', N'ListingType', N'Location', N'LocationVerified', N'Longitude', N'ManagerNotes', N'MarketValue', N'ParkingSlots', N'ProfitabilityRating', N'PropertyDetailsNotes', N'PropertyType', N'PropertyTypeTypeId', N'PropertyVerified', N'ReadyForApproval', N'ReadyForApprovalAt', N'RejectionReason', N'RentCastLastUpdated', N'RentEstimate', N'ReviewDueDate', N'ReviewStatus', N'ReviewTimeframeDays', N'SellerContacted', N'SellerContactedAt', N'SellerId', N'SourceSellerListingId', N'Sqft', N'Status', N'SuggestedPrice', N'Title', N'UpdatedAt', N'VerifiedDocumentTypesJson', N'YieldScore') AND [object_id] = OBJECT_ID(N'[Properties]'))
        SET IDENTITY_INSERT [Properties] ON;
    EXEC(N'INSERT INTO [Properties] ([PropertyId], [Accommodation], [AgentId], [AmenitiesVerified], [ApprovedBy], [ApproverUserId], [AreaVerified], [AuditLogJson], [BasePrice], [Bathrooms], [Bedrooms], [ContactMethod], [ContactNotes], [CoverImage], [CreatedAt], [DealArrangementDate], [DealArrangementScheduled], [DecisionAt], [Description], [DocumentNotes], [DocumentUrlsJson], [DocumentsReceived], [DocumentsReceivedAt], [DocumentsVerified], [DocumentsVerifiedAt], [EmployeeId], [ExpectedCompletionDate], [ExpectedTimeframeDays], [FinalPrice], [FinalReviewNotes], [InspectionCompleted], [InspectionCompletedAt], [InspectionNotes], [InspectionResult], [InspectionScheduled], [InspectionScheduledDate], [IsApproved], [IsRejected], [Latitude], [ListingType], [Location], [LocationVerified], [Longitude], [ManagerNotes], [MarketValue], [ParkingSlots], [ProfitabilityRating], [PropertyDetailsNotes], [PropertyType], [PropertyTypeTypeId], [PropertyVerified], [ReadyForApproval], [ReadyForApprovalAt], [RejectionReason], [RentCastLastUpdated], [RentEstimate], [ReviewDueDate], [ReviewStatus], [ReviewTimeframeDays], [SellerContacted], [SellerContactedAt], [SellerId], [SourceSellerListingId], [Sqft], [Status], [SuggestedPrice], [Title], [UpdatedAt], [VerifiedDocumentTypesJson], [YieldScore])
    VALUES (1, N'''', 0, CAST(0 AS bit), NULL, NULL, CAST(0 AS bit), NULL, 15000000.0, 2, 3, NULL, NULL, NULL, ''2023-01-01T00:00:00.0000000Z'', NULL, CAST(0 AS bit), NULL, N''Beautiful modern house with spacious rooms, located in a prime area of Makati City. Perfect for families looking for comfort and convenience.'', NULL, NULL, CAST(0 AS bit), NULL, CAST(0 AS bit), NULL, NULL, NULL, NULL, NULL, NULL, CAST(0 AS bit), NULL, NULL, NULL, CAST(0 AS bit), NULL, CAST(0 AS bit), CAST(0 AS bit), NULL, 0, N''Makati City, Metro Manila'', CAST(0 AS bit), NULL, NULL, NULL, 2, NULL, NULL, N''House'', NULL, CAST(0 AS bit), CAST(0 AS bit), NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(0 AS bit), NULL, 1, NULL, 150, N''Approved'', NULL, N''Modern 3BR House in Makati'', NULL, NULL, NULL),
    (2, N'''', 0, CAST(0 AS bit), NULL, NULL, CAST(0 AS bit), NULL, 12000000.0, 1, 2, NULL, NULL, NULL, ''2023-01-01T00:00:00.0000000Z'', NULL, CAST(0 AS bit), NULL, N''High-end condominium unit in the heart of Bonifacio Global City. Stunning city views and world-class amenities.'', NULL, NULL, CAST(0 AS bit), NULL, CAST(0 AS bit), NULL, NULL, NULL, NULL, NULL, NULL, CAST(0 AS bit), NULL, NULL, NULL, CAST(0 AS bit), NULL, CAST(0 AS bit), CAST(0 AS bit), NULL, 0, N''Bonifacio Global City, Taguig'', CAST(0 AS bit), NULL, NULL, NULL, 1, NULL, NULL, N''Condo'', NULL, CAST(0 AS bit), CAST(0 AS bit), NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(0 AS bit), NULL, 1, NULL, 80, N''Approved'', NULL, N''Luxury Condo in BGC'', NULL, NULL, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PropertyId', N'Accommodation', N'AgentId', N'AmenitiesVerified', N'ApprovedBy', N'ApproverUserId', N'AreaVerified', N'AuditLogJson', N'BasePrice', N'Bathrooms', N'Bedrooms', N'ContactMethod', N'ContactNotes', N'CoverImage', N'CreatedAt', N'DealArrangementDate', N'DealArrangementScheduled', N'DecisionAt', N'Description', N'DocumentNotes', N'DocumentUrlsJson', N'DocumentsReceived', N'DocumentsReceivedAt', N'DocumentsVerified', N'DocumentsVerifiedAt', N'EmployeeId', N'ExpectedCompletionDate', N'ExpectedTimeframeDays', N'FinalPrice', N'FinalReviewNotes', N'InspectionCompleted', N'InspectionCompletedAt', N'InspectionNotes', N'InspectionResult', N'InspectionScheduled', N'InspectionScheduledDate', N'IsApproved', N'IsRejected', N'Latitude', N'ListingType', N'Location', N'LocationVerified', N'Longitude', N'ManagerNotes', N'MarketValue', N'ParkingSlots', N'ProfitabilityRating', N'PropertyDetailsNotes', N'PropertyType', N'PropertyTypeTypeId', N'PropertyVerified', N'ReadyForApproval', N'ReadyForApprovalAt', N'RejectionReason', N'RentCastLastUpdated', N'RentEstimate', N'ReviewDueDate', N'ReviewStatus', N'ReviewTimeframeDays', N'SellerContacted', N'SellerContactedAt', N'SellerId', N'SourceSellerListingId', N'Sqft', N'Status', N'SuggestedPrice', N'Title', N'UpdatedAt', N'VerifiedDocumentTypesJson', N'YieldScore') AND [object_id] = OBJECT_ID(N'[Properties]'))
        SET IDENTITY_INSERT [Properties] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260510082627_SeedPropertiesAndSellers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260510082627_SeedPropertiesAndSellers', N'10.0.0');
END;

COMMIT;
GO

