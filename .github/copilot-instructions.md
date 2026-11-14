# Project Context & Instructions: Da Horta

## 1. Product Vision & Business Domain
- **App Name:** Da Horta ("Direto do produtor para a sua mesa.")
- **Target Market:** Portugal (All user-facing UI text, labels, placeholders, and error messages MUST be written in European Portuguese **PT-PT**, never PT-BR).
- **Core Objective (MVP):** A mobile-first hyperlocal marketplace allowing small farmers in a pilot region in Portugal to quickly list surplus fresh produce (fruits, vegetables, olive oil, etc.), and allowing nearby consumers to reserve quantities and pick them up directly at the farm.
- **Key Market Constraints:**
  - Rural farms in Portugal often lack street addresses; exact GPS coordinates (latitude/longitude) and short landmark notes (`AddressHint`) are mandatory.
  - WhatsApp (`https://wa.me...`) is the primary communication tool in Portugal; always favor direct WhatsApp deep-links over building complex in-app chat for the MVP.
  - Farmers have limited time and varying digital literacy; the listing creation flow must be ultra-simple (3 steps max) with automatic listing expiration (24h–48h) to prevent outdated stock.
  - Payment in MVP is cash on pickup ("Pagamento no local"), with future support planned for **MB WAY** (via Stripe, Eupago, or Ifthenpay).

## 2. Tech Stack & Architecture
- **Environment:** Linux (WSL on Windows), Docker Compose.
- **Backend (`DaHorta.Api`):**
  - **.NET 10** Web API (C# 14), Modular Monolith architecture.
  - **Entity Framework Core 10** with `Npgsql.EntityFrameworkCore.PostgreSQL` and `NetTopologySuite`.
  - **API Docs:** Built-in OpenAPI + `Scalar.AspNetCore` (no Swashbuckle/Swagger).
- **Database:**
  - **PostgreSQL 16 + PostGIS** running in Docker.
  - Spatial queries MUST use `NetTopologySuite.Geometries.Point` with SRID `4326` mapped to PostgreSQL `geography (point, 4326)` so distances are calculated accurately in meters.
- **Frontend (`da-horta-web`):**
  - **Angular** configured as a **Progressive Web App (PWA)** (`@angular/pwa`), strictly mobile-first.
  - **Maps:** **Leaflet** (OpenStreetMap) for displaying farm pins, plus external deep-links to Google Maps/Waze (`https://google.com`) for turn-by-turn navigation.
  - **Media:** Client-side image compression before uploading to preserve mobile data in rural areas.

## 3. UI/UX & Brand Identity
- **Color Palette:**
  - Primary (Forest/Olive Green): `#2D5A27`
  - Accent / CTA (Terracotta Orange): `#D96B27`
  - Background (Warm Cream / Off-white for outdoor sunlight readability): `#FAF9F6`
- **Design Principles:** High contrast, large touch targets (minimum 48px height), minimal text, icon-driven navigation.

## 4. Coding Standards
- **C# / .NET 10:** Use primary constructors, file-scoped namespaces, `required` properties on models/DTOs, and clean RESTful conventions. Never expose EF Core entities directly from controllers; always use DTOs/Records.
- **Angular:** Use standalone components, Angular Signals for state management, modern control flow (`@if`, `@for`), and strict TypeScript typing.