# SEO and Multilevel Category Menu Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make public news crawlable and add an accessible recursive category menu while preserving the existing API, OData, JWT, and SQL Server behavior.

**Architecture:** The MVC client will call anonymous API endpoints through a typed HttpClient and render public pages server-side. SEO metadata and discovery endpoints live in the WebClient because it owns the public URLs. The existing recursive Category Tree DTO powers the menu, with backend cycle validation protecting the hierarchy.

**Tech Stack:** .NET 8, ASP.NET Core MVC, ASP.NET Core Web API/OData, Razor, xUnit, Bootstrap-compatible CSS, vanilla JavaScript/jQuery.

---

### Task 1: Tests and typed public models

- [ ] Add test projects to both solutions.
- [ ] Add failing tests for slug generation, public API mapping, canonical redirects, sitemap filtering, recursive menu rendering, and category cycles.
- [ ] Run the focused tests and confirm failures are caused by missing behavior.

### Task 2: Server-render public pages

- [ ] Add public news/category response models and `IPublicNewsApiClient`.
- [ ] Register the typed HttpClient with `ApiBaseUrl`.
- [ ] Convert home and article actions to async server rendering with 404/error handling.
- [ ] Replace client-generated initial cards and article body with Razor output.

### Task 3: Canonical SEO

- [ ] Add `/news/{id}/{slug?}` and permanent redirect rules.
- [ ] Add per-page title, description, canonical, robots, Open Graph, and Twitter metadata.
- [ ] Add safe `NewsArticle` JSON-LD.

### Task 4: Sitemap and robots

- [ ] Add dynamic `/sitemap.xml` from approved public articles.
- [ ] Add `/robots.txt` with sitemap location and management-route exclusions.

### Task 5: Recursive category menu

- [ ] Render all Category Tree depths recursively.
- [ ] Add desktop fly-out and mobile/touch accordion interactions.
- [ ] Add keyboard and ARIA behavior.
- [ ] Reject indirect category cycles and guard tree construction.

### Task 6: Verification

- [ ] Run all automated tests and build both solutions.
- [ ] Start API and WebClient, then smoke-test public pages, canonical redirects, robots, and sitemap.
- [ ] Verify desktop/mobile menu behavior and confirm page source contains article SEO without JavaScript.
