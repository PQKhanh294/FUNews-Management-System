# FU News Management System - Assignment 02 Frontend

Solution: `PhanQuocKhanh_SE18D05_A02_FE.sln`

## Scope Theo De Chinh Thuc

- ASP.NET Core MVC Web Application.
- jQuery AJAX client calls the backend Web API.
- CRUD/Search pages use DataTables and Bootstrap modals.
- Delete actions use confirmation modal.
- Frontend does not connect to SQL Server directly.

## Run

1. Start backend first.
2. Update `FUNewsManagementSystem.WebClient/appsettings.json` when backend URL changes:

```json
{
  "ApiBaseUrl": "https://localhost:7101"
}
```

3. Start frontend:

```powershell
dotnet run --project .\FUNewsManagementSystem.WebClient\FUNewsManagementSystem.WebClient.csproj
```

Default launch profile: `https://localhost:7036`.

## Pages

- `/` - public active news.
- `/Home/Login` - email/password login.
- `/Home/News` - Staff news management with popup create/update.
- `/Home/Accounts` - Admin account management with popup create/update.
- `/Home/Dashboard` - Admin report statistics by date range.
- `/Home/History` - Staff news history.
- `/Home/Profile` - Staff/Admin profile update.

