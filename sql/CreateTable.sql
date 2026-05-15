IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U')
BEGIN
    CREATE TABLE Products (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        ProductName NVARCHAR(255)    NOT NULL,
        Description NVARCHAR(MAX)    NOT NULL,
        Price       DECIMAL(18, 2)   NOT NULL,
        ConstPrice  DECIMAL(18, 2)   NOT NULL,
        Discount    DECIMAL(18, 2)   NOT NULL,
        Quantity    BIGINT           NOT NULL,
        IsAvailable BIT              NOT NULL,
        CreatedAt   DATETIME         NOT NULL,
        UpdatedAt   DATETIME         NOT NULL
    );
END
