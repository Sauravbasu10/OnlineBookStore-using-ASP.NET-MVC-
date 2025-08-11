# 📚 Online Book Store (ASP.NET MVC)

An educational **Online Book Store** application built with **ASP.NET MVC** and **Entity Framework**.  
It demonstrates a simple e-commerce workflow where users can browse books, search, add them to the cart, and place orders.  
An admin dashboard allows management of books, genres, and stock levels.

---

## ✨ Features

### User Features
- Browse books by title, genre, or author
- Search books with keywords
- Add books to cart
- Update cart quantities or remove items
- Checkout and place orders
- User registration and login

### Admin Features
- Manage books (Create, Edit, Delete)
- Manage genres/categories
- Manage stock levels
- View and manage orders

---

## 🛠 Tech Stack

- **ASP.NET MVC** (Framework or Core, depending on branch)
- **C#** for backend logic
- **Razor Views** for front-end rendering
- **Entity Framework (Code First)** for ORM
- **SQL Server / LocalDB** for database
- **Bootstrap** for styling
- **JavaScript/jQuery** for interactivity


## 📂 Project Structure

```plaintext
BookShoppingCart/
├── Controllers/        # MVC Controllers (Books, Cart, Orders, Admin, Account)
├── Models/             # Entity Models and ViewModels
├── Data/               # ApplicationDbContext and EF Migrations
├── Repositories/       # Repository pattern implementation for data access
├── Views/              # Razor Views (UI Pages)
├── Migrations/         # Entity Framework Code First Migrations
├── wwwroot/            # Static files (CSS, JS, Bootstrap)
├── BookShoppingCart.sln # Visual Studio Solution
└── README.md           # Documentation
```

---

## 🚀 Getting Started

Follow these steps to set up and run the project locally.

### 1️⃣ Prerequisites
Before running the project, make sure you have:
- Visual Studio 2019 or 2022 (Community Edition works fine)
- .NET SDK compatible with the project version
- SQL Server (LocalDB or Express)

### 2️⃣ Clone Repository
Open a terminal and run:
```bash
git clone https://github.com/Sauravbasu10/OnlineBookStore-using-ASP.NET-MVC-.git
cd OnlineBookStore-using-ASP.NET-MVC-
```

### 3️⃣ Configure Database

Edit `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BookShoppingCart;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
Run:
```powershell
Update-Database
```

### 4️⃣ Run the App
- Open `BookShoppingCart.sln` in Visual Studio
- Set the main web project as **Startup Project**
- Press **F5** to run in IIS Express or Kestrel



## 👨‍💻 Author
**Saurav Basu**  
GitHub: [Sauravbasu10](https://github.com/Sauravbasu10)

