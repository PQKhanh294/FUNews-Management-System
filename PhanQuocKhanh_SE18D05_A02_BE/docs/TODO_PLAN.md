# FU News Management System TODO Plan

## Backend Layers

- Common: enums, authorization constants, option classes.
- Data: scaffold entities and `FUNewsManagementContext` from SQL Server, add partial mappings for assignment extension columns, implement `IGenericRepository<T>`, entity repositories, and `IUnitOfWork`.
- Business: DTOs, validators, auth service, CRUD services, report service.
- API: JWT/OData setup, SQL Server context, controllers for Auth, Accounts, Categories, NewsArticles, Tags, Report, Profile.

## Frontend Layers

- MVC shell: Razor views for public news, login, dashboard, accounts, news CRUD, history, profile.
- JavaScript: shared AJAX wrapper in `site.js`; one page script per feature; DataTables, Select2, TinyMCE, Chart.js via CDN.
- Security flow: JWT is stored in `sessionStorage` and attached through the shared AJAX wrapper.

## Verification

- Build BE solution.
- Build FE solution.
- Configure SQL Server connection string and Google Client ID before running end to end.

