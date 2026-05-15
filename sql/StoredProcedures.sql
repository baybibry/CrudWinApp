-- =============================================
-- Stored Procedures for Products
-- Database: ProductDb
-- =============================================

-- =============================================
-- sp_AddProduct
-- Matches: ProductService.AddProduct()
-- =============================================
CREATE OR ALTER PROCEDURE sp_AddProduct
    @ProductName    NVARCHAR(255),
    @Description    NVARCHAR(MAX),
    @Price          DECIMAL(18, 2),
    @ConstPrice     DECIMAL(18, 2),
    @Discount       DECIMAL(18, 2),
    @Quantity       BIGINT,
    @IsAvailable    BIT,
    @CreatedAt      DATETIME,
    @UpdatedAt      DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Products (ProductName, Description, Price, ConstPrice, Discount, Quantity, IsAvailable, CreatedAt, UpdatedAt)
    VALUES (@ProductName, @Description, @Price, @ConstPrice, @Discount, @Quantity, @IsAvailable, @CreatedAt, @UpdatedAt);

    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO

-- =============================================
-- sp_GetAllProducts
-- Matches: ProductService.GetAllProducts()
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetAllProducts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ProductName, Description, Price, ConstPrice, Discount, Quantity, IsAvailable, CreatedAt, UpdatedAt
    FROM Products
    ORDER BY Id ASC;
END;
GO

-- =============================================
-- sp_GetProductById
-- Matches: ProductService.GetProductById()
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetProductById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ProductName, Description, Price, ConstPrice, Discount, Quantity, IsAvailable, CreatedAt, UpdatedAt
    FROM Products
    WHERE Id = @Id;
END;
GO

-- =============================================
-- sp_UpdateProduct
-- Matches: ProductService.UpdateProduct()
-- =============================================
CREATE OR ALTER PROCEDURE sp_UpdateProduct
    @Id             INT,
    @ProductName    NVARCHAR(255),
    @Description    NVARCHAR(MAX),
    @Price          DECIMAL(18, 2),
    @ConstPrice     DECIMAL(18, 2),
    @Discount       DECIMAL(18, 2),
    @Quantity       BIGINT,
    @IsAvailable    BIT,
    @UpdatedAt      DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = @Id)
    BEGIN
        RAISERROR('Product not found.', 16, 1);
        RETURN;
    END

    UPDATE Products
    SET ProductName = @ProductName,
        Description = @Description,
        Price       = @Price,
        ConstPrice  = @ConstPrice,
        Discount    = @Discount,
        Quantity    = @Quantity,
        IsAvailable = @IsAvailable,
        UpdatedAt   = @UpdatedAt
    WHERE Id = @Id;
END;
GO

-- =============================================
-- sp_DeleteProduct
-- Matches: ProductService.DeleteProduct()
-- =============================================
CREATE OR ALTER PROCEDURE sp_DeleteProduct
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = @Id)
    BEGIN
        RAISERROR('Product not found.', 16, 1);
        RETURN;
    END

    DELETE FROM Products WHERE Id = @Id;
END;
GO
