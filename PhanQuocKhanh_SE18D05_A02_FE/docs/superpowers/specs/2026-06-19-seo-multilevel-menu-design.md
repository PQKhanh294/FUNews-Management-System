# SEO and Multilevel Category Menu Design

## Goal

Render public news as crawlable MVC HTML and expose an accessible recursive category menu without changing the SQL Server schema or adding another data store.

## Public rendering

The WebClient uses a typed `PublicNewsApiClient` to call the anonymous API endpoints. The home action renders the initial approved article list on the server. The article action renders the complete title, description, author, dates, category, tags, and sanitized rich content before returning HTML. JavaScript remains progressive enhancement only.

## URLs and metadata

The canonical article route is `/news/{id}/{slug}`. Missing or stale slugs redirect permanently to the canonical route. Each article emits title, description, canonical, Open Graph, Twitter Card, and `NewsArticle` JSON-LD metadata. The old `/Home/NewsDetails/{id}` route permanently redirects.

## Discovery

`/sitemap.xml` contains only active, approved public articles and uses their canonical URLs. `/robots.txt` exposes the sitemap and prevents indexing management and authentication pages.

## Category navigation

The existing `/odata/Categories/Tree` endpoint remains the source of truth. The client renders `Children` recursively at arbitrary depth. Desktop uses fly-out submenus; touch and mobile use explicit expand buttons. Links remain normal category-filter URLs so navigation works without pointer hover.

The backend rejects category parent changes that would create direct or indirect cycles. Tree construction also guards against malformed legacy rows.

## Safety and testing

The public API remains responsible for filtering `NewsStatus = true` and `ApprovalStatus = Approved`. Rich HTML is sanitized before Razor renders it. Tests cover SEO metadata, canonical redirects, public HTML, sitemap filtering, recursive menus, and category-cycle validation. Both solutions are built and smoke-tested against the running SQL Server-backed API.
