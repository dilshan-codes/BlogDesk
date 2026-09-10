# BlogDesk

A blog platform where posts are plain HTML/CSS/JS folders committed straight to git and rendered in sandboxed iframes with zero build step, backed by an ASP.NET Core + PostgreSQL API for views/ratings/comments, a passwordless email-OTP admin console, and (in progress) a RAG-powered AI assistant that answers reader questions from each post's own FAQ file.

> **Status:** Actively in development. Backend schema and passwordless admin authentication are complete and tested against live infrastructure (Supabase Postgres, Resend). Engagement API, frontend, and AI agent are in progress.

---

## What this project is

BlogDesk rethinks the usual "CMS with an admin form" blogging model. Instead of storing post content in a database, each post is a self-contained folder — `index.html`, `style.css`, `script.js`, `meta.json`, `faq.md` — added directly to the git repository and rendered in an isolated `<iframe>` on the live site. Publishing a post is a `git push`, not a form submission.

What the database *does* track is real reader engagement: view counts, 1–5 star ratings, and comments — all tied to an email address that's collected but never displayed. Instead, each commenter gets a stable, automatically generated display handle derived from their email (e.g. `sarah.k-9f2a`), which cannot be edited by anyone, including the site admin.

Site administration has no visible login button anywhere on the public site. An unlisted admin route leads to a passwordless flow: request a one-time code, receive it by real email, verify it, and get a revocable session — with every admin action logged to an audit trail.

The planned AI layer is a read-only LangGraph agent that re-ingests its own knowledge base every time a post's `faq.md` is added, so the chat widget's knowledge always matches exactly what's live in git, with no manual retraining step.

## Architecture

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core 10 (Web API, controller-based) |
| Database | PostgreSQL, hosted on Supabase (free tier, session pooler) |
| ORM | Entity Framework Core + Npgsql |
| Auth | Passwordless email OTP — BCrypt-hashed codes, opaque revocable session tokens (not JWT) |
| Email delivery | Resend API |
| Frontend | React (Vite) — planned |
| Blog rendering | Sandboxed `<iframe>` per post, `postMessage` bridge for view-pinging and auto-resize — planned |
| AI agent | Python, FastAPI, LangChain, LangGraph, Chroma (RAG), Google Gemini — planned |
| Hosting | Vercel (frontend), MonsterASP.NET (API), Render (agent) — all free tiers, no credit card required |

## Progress

- [x] PostgreSQL schema via EF Core migrations (`PostViews`, `PostRatings`, `Comments`, `AdminOtps`, `AdminSessions`, `AdminActions`)
- [x] Passwordless admin authentication: OTP request/verify/logout, tested end-to-end against real Supabase + Resend infrastructure
- [x] Security hardening: hashed OTP codes, rate limiting, generic non-enumerable responses, reuse prevention, automatic retry on transient database failures
- [ ] REST API for view counts, ratings (upsert-by-email), and comments with deterministic anonymous handle generation
- [ ] Post folder convention + manifest scanner (git-as-CMS publishing workflow)
- [ ] React frontend: homepage, iframe post viewer, rating/comment UI, admin panel
- [ ] Privacy Policy page
- [ ] LangGraph RAG agent with per-post FAQ ingestion and read-only tool-calling
- [ ] Deployment across Vercel, MonsterASP.NET, and Render

## Notable engineering decisions

- **Git as the source of truth for content, database only for engagement data.** Post metadata never lives in two places at once, eliminating an entire class of sync bugs.
- **Opaque session tokens instead of JWT for admin auth**, specifically so a single admin session can be instantly revoked server-side — a property stateless JWTs don't offer without extra infrastructure.
- **Constant-response-shape OTP endpoint** — requesting a code for an unauthorized email returns the same generic message as a valid request, preventing account enumeration, mirroring the pattern used by production password-reset flows.
- **Automatic transient-fault retry** (`EnableRetryOnFailure`) added after diagnosing intermittent connection drops against Supabase's pooled connections — a real production-grade resilience fix, not a workaround.

## Skills demonstrated

ASP.NET Core Web API design · Entity Framework Core & PostgreSQL · secure authentication design (OTP, hashing, session revocation, rate limiting) · third-party API integration (Resend, Supabase) · debugging transient distributed-systems failures · Conventional Commits / git discipline · (planned) React, sandboxed iframe architecture, LangChain/LangGraph RAG agent design, multi-service free-tier deployment.

---

## Local development setup

Requires .NET 8+, Node 18+, Python 3.11+, and a free Supabase project.

```bash
cd BlogDesk.Api
dotnet restore
dotnet ef database update
dotnet run
```

Configuration (connection string, Resend API key, admin email) is set in a gitignored `appsettings.Development.json` — see `appsettings.json` for the required shape.

## License

Not yet decided — add before making this repository public if that's the intent.
