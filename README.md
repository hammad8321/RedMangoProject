# 🍽️ MangoFusion API

**MangoFusion API** is a RESTful backend built with **ASP.NET Core** that powers a restaurant-style application.  
It manages menu items, user authentication, and customer orders with JWT-based security.

---

## 🚀 Features

- ✅ User authentication & registration using **ASP.NET Identity**
- 🔐 JWT-based login and role management
- 🍔 CRUD operations for **Menu Items**
- 🛒 Order processing and tracking (OrderHeader / OrderDetail)
- 📦 Integrated image upload for menu items
- 📡 Standardized API response model (`ApiResponse.cs`)
- 🧠 Entity Framework Core integration for SQL Server

---

## 🧩 Project Structure

| File | Description |
|------|--------------|
| **ApiResponse.cs** | Standardized response wrapper for all API endpoints |
| **ApplicationUser.cs** | Extends IdentityUser with custom fields (like Name) |
| **MenuItem.cs** | Represents f
