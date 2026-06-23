# FU News Management System - Assignment 02 Backend

Solution: `PhanQuocKhanh_SE18D05_A02_BE.sln`

## Scope Theo De Chinh Thuc

- ASP.NET Core 8 Web API + OData query support + JWT.
- Entity Framework Core scaffold tu SQL Server database `FUNewsManagement`.
- 3-layer architecture: API / Business Service / Repository-DAL.
- Repository pattern + UnitOfWork; API Controller khong ket noi truc tiep `DbContext`.
- Singleton pattern: `IJwtTokenGenerator`, `IPasswordHasher`, `ISeoMetadataBuilder`.
- Admin account lay tu `appsettings.json`: `admin@FUNewsManagementSystem.org` / `@@abc123@@`.
- Staff account lay tu bang `SystemAccount` voi `AccountRole = 1`.

## Run

1. Confirm SQL Server has database `FUNewsManagement`.
2. Update `FUNewsManagementSystem.API/appsettings.json` if SQL Server is not `Server=localhost`.
3. Start API:

```powershell
dotnet run --project .\FUNewsManagementSystem.API\FUNewsManagementSystem.API.csproj
```

Default launch profile: `https://localhost:7101`.

The API uses only the configured SQL Server database for runtime data.

## Main Endpoints

- `POST /api/auth/login` - anonymous, email/password JWT login.
- `GET /odata/NewsArticles` - anonymous can view active news; authenticated users can query news with OData.
- `GET /odata/NewsArticles/{id}` - anonymous can view active news detail.
- `POST|PUT|DELETE /odata/NewsArticles` - Staff.
- `GET /odata/NewsArticles/MyHistory` - Staff.
- `GET|POST|PUT|DELETE /odata/Accounts` - Admin.
- `GET /odata/Categories/Tree` - anonymous/authenticated.
- `GET /odata/Categories` - OData query.
- `POST|PUT|DELETE /odata/Categories` - Staff.
- `GET|POST|PUT|DELETE /odata/Tags` - Staff for writes.
- `GET /api/report/news-statistics?startDate=&endDate=` - Admin.
- `GET|PUT /api/profile` - Admin or Staff.

## Delete Rules

- Account cannot be deleted if it has created any news article.
- Category cannot be deleted if it is used by any news article.
- News delete removes related tag links first.

