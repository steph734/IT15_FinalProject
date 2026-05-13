BEGIN TRANSACTION;
ALTER TABLE [ViewingAppointments] ADD [BuyerType] nvarchar(50) NULL;

ALTER TABLE [ViewingAppointments] ADD [CreatedAtUtc] datetime2 NOT NULL DEFAULT (GETUTCDATE());

ALTER TABLE [ViewingAppointments] ADD [CustomerEmail] nvarchar(255) NULL;

ALTER TABLE [ViewingAppointments] ADD [FinancingStatus] nvarchar(50) NULL;

ALTER TABLE [ViewingAppointments] ADD [InformationSource] nvarchar(50) NULL;

ALTER TABLE [ViewingAppointments] ADD [Notes] nvarchar(1000) NULL;

ALTER TABLE [ViewingAppointments] ADD [NumberOfVisitors] int NOT NULL DEFAULT 0;

ALTER TABLE [ViewingAppointments] ADD [PreferredTime] nvarchar(20) NULL;

CREATE INDEX [IX_ViewingAppointments_CustomerEmail] ON [ViewingAppointments] ([CustomerEmail]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260425173933_AddViewingAppointmentDetails', N'10.0.5');

COMMIT;
GO

