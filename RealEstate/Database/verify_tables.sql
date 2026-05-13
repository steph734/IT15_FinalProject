-- Verify all tables were created successfully
SELECT 
    t.name AS TableName,
    SCHEMA_NAME(t.schema_id) AS SchemaName,
    p.rows AS TotalRows
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0, 1)
WHERE t.name IN (
    'Properties', 'PropertyImages', 'PropertyDocuments',
    'PropertyPricings', 'CommissionRules',
    'Inquiries',
    'Transactions', 'Payments', 'Invoices',
    'Commissions', 'Payouts',
    'FinancialRecords', 'AuditLogs'
)
ORDER BY t.name;
