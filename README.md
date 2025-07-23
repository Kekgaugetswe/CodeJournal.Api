# CodeJournal – Backend API (.NET 8)

## 📌 Overview

**CodeJournal** is a personal blogging and journaling platform designed for developers. This backend API powers the entire CodeJournal ecosystem — managing posts, authors, comments, categories, and supporting data.

The project is designed to evolve into a **SaaS-ready platform**, enabling multiple creators to run their own journal/blog instances with scoped data, custom branding, and monetization.

> 🔗 Pairs with the frontend: [CodeJournal.Web](https://github.com/kekgaugetswe/CodeJournal.Web)

---

## ✨ Features

- ✅ Create, edit, and delete blog posts with slugs and metadata
- ✅ Comment system with threaded replies
- ✅ Manage authors, categories, and their relationships
- ✅ Track created and updated timestamps per entity
- ✅ DTO mapping to prevent overexposing domain models
- 🔒 Planned: JWT Auth + Role-based access control (Author/Admin)
- 🏷️ Planned: SaaS-ready multi-tenancy & tenant isolation

---

## 🧱 Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core (SQL Server)
- AutoMapper
- Swagger (Swashbuckle)
- FluentValidation (optional)
- Serilog for logging (optional)
- IdentityServer or JWT-based Identity (future)

---

## 📁 Project Structure

