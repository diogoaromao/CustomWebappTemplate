# Database Schema Organization

This template uses a project-named schema to organize database objects, providing better isolation and organization.

## Schema Structure

When you create a project using this template with:
```bash
dotnet new webappvue -n MyAwesomeProject
```

The database will be organized as follows:

### Schema Name
- **Schema**: `MyAwesomeProject` (matches your project name)
- **Template Value**: `MyTemplate` (replaced during template instantiation)

### Tables Created

```sql
-- Products table
MyAwesomeProject.Products
├── Id (int, primary key, auto-increment)
├── Name (varchar(100), required)
├── Description (varchar(500))
├── Price (decimal(18,2), required)
├── CreatedAt (timestamp, required)
└── UpdatedAt (timestamp, required)

-- Carts table  
MyAwesomeProject.Carts
├── UserId (varchar(50), primary key)
├── CreatedAt (timestamp, required)
└── UpdatedAt (timestamp, required)

-- Cart Items table
MyAwesomeProject.CartItems
├── UserId (varchar(50), primary key, foreign key)
├── ProductId (int, primary key)
├── ProductName (varchar(100), required)
├── UnitPrice (decimal(18,2), required)
└── Quantity (int, required)
```

## Benefits

### ✅ **Organization**
- All application tables are grouped under one schema
- Easy to identify which objects belong to your application
- Clear separation from system tables and other applications

### ✅ **Multi-Tenant Support**
- Multiple applications can share the same database server
- No naming conflicts between different projects
- Each project has its own namespace

### ✅ **Security**
- Database permissions can be granted at the schema level
- Application user only needs access to its own schema
- Better security isolation

### ✅ **Maintenance**
- Easier database backups (schema-specific)
- Simpler migrations and deployments
- Clear ownership of database objects

## Connection Examples

### Entity Framework Connection String
```
Host=localhost;Database=MyCompanyDB;Username=myapp_user;Password=secure123;SearchPath=MyAwesomeProject
```

### SQL Queries
```sql
-- Explicit schema reference
SELECT * FROM MyAwesomeProject.Products;

-- With search path set, schema is optional
SELECT * FROM Products;
```

## Configuration

The schema is configured in `ApplicationDbContext.cs`:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Set default schema to project name (will be replaced by template engine)
    modelBuilder.HasDefaultSchema("MyTemplate");
    
    // ... rest of configuration
}
```

When you instantiate the template:
- `MyTemplate` → `MyAwesomeProject` (or your chosen name)
- All generated migrations will use the correct schema
- Entity Framework will automatically use the schema for all operations

## Migration Example

The generated migration will create the schema and tables:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.EnsureSchema(name: "MyAwesomeProject");

    migrationBuilder.CreateTable(
        name: "Products",
        schema: "MyAwesomeProject",
        columns: table => new
        {
            // ... column definitions
        });
}
```

## Best Practices

### ✅ **DO:**
- Use the project name as the schema name
- Grant permissions at the schema level
- Include schema in backup/restore scripts
- Document schema usage for team members

### ❌ **DON'T:**
- Mix application tables with system tables
- Use generic schema names like "app" or "data"
- Grant excessive permissions to the entire database
- Hardcode schema names in queries (use EF configuration)