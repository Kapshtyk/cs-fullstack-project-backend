# E-commerce App - Backend for Crazy Shop
## Overview
Crazy Shop is a modern web application designed to manage e-commerce operations. It includes functionalities for creating, reading, updating and deleting categories, products, users and other entities necessary for an online shop.

[Live link](http://98.71.25.4/swagger/index.html)

[Frontend link](http://crazy-shop.zapto.org)

[Frontend repo link](https://github.com/Kapshtyk/fs18_CSharp_FullStack_Frontend/tree/main)

Admin credentials: 
   - email: test@user.com
   - password: sygxy2-cuxteb-maJcaq

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies](#technologies)
- [Project Structure](#project-structure)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [Testing](#testing)

## Features
- Authentication and authorisation: Secure user authentication using JWT access and update tokens
- Category Management: CRUD operations on product categories.
- Product Management: CRUD operations for products.
- User Management: CRUD operations on users.
- Review Management: CRUD operations for product reviews.
- Order Management: Manage customer orders.
- File Management: Manage file uploads and storage.
- Swagger Documentation: Automatically generated API documentation.
- Unit and integration testing using xUnit and Moq.
- CI/CD: Continuous integration and deployment using GitHub actions and a virtual machine.

## Technologies
- ASP.NET Core: Web framework for building the application.
- Entity Framework Core: ORM for database interactions.
- PostgreSQL: Relational database management system.
- Fluent Validation: Data validation library.
- Swagger: API documentation tool.
- xUnit: Testing framework.
- Moq: Library for creating mock objects in tests.
- Redis: Caches the data associated with get requests, stores the encrypted refresh tokens and blacklisted tokens (after logout).

## Project structure

```
.github/
    pull_request_template.md
    workflows/
        check.yml
        deploy.tml
.gitignore
.vscode/
    settings.json
cicd/
   nginx/
      nginx.conf
   docker-compose.local.yml
   docker-compose.yml    
Ecommerce/
    Ecommerce.Controllers/
        AppController.cs
        AuthController.cs
        CartItemController.cs
        CategoryController.cs
        FrontPageController.cs
        OrderController.cs
        ProductController.cs
        ReviewController.cs
        UserController.cs
        CustomAuthorization/
        CustomMiddleware/
        CustomExceptions/
        ...
    Ecommerce.Domain/
    Ecommerce.Infrastructure/
    Ecommerce.Services/
    Ecommerce.sln
    Ecommerce.Tests/
README.md
```

### Layers and entities
#### Controllers

Controllers process HTTP requests and return responses. They act as an interface between the client and the server-side logic.

- AuthController: Manages user authentication, authorisation and token updates.
- CategoryController: Handles CRUD operations for product categories.
- ProductController: Handles CRUD operations for products.
- UserController: Handles CRUD operations for users.
- CartItemController: Handles user shopping carts.
- OrderController: Handles customer orders.
- ReviewController: Manages product reviews.
- FrontPageController: Manages the front page of the application

There is also exception handling middleware that provides additional functionality, such as specific ways of handling SQL exceptions and generating JSON-friendly output.

#### Services
Services contain the application's business logic. They interact with repositories to perform operations on the data.

- AuthService: Handles user login, logout and token updates.
- CategoryService: Manages category business logic.
- ProductService: Manages the business logic for products.
- UserService: Manages the business logic for users.
- OrderService: Manages the business logic for orders.
- ReviewService: Manages the business logic for reviews.
- CartItemService: Manages the business logic for shopping cart items.
- FrontPageService: Manages front pages.

#### Repositories
Repositories encapsulate the logic required to access data sources. They provide an abstraction layer over the database.

- CategoryRepository: Handles data operations for categories.
- ProductRepository: Handles data operations for products.
- UserRepository: Handles data operations for users.
- OrderRepository: Handles data operations for orders.
- ReviewRepository: Handles data operations for reviews.

#### Domain
The domain layer contains the core entities and interfaces of the application.

- Entities: Represent the data models, such as Category, Product, User, Order, and Review.
- Interfaces: Define the contracts for repositories and services.

ERD diagram describing the database structure:

![ERD](https://github.com/user-attachments/assets/08b1fa43-78e8-4bfa-b163-feb9006f2ee6)



#### Infrastructure
The infrastructure layer contains the implementation of repositories, the database context, and additional services.

- EcommerceContext: The Entity Framework Core database context.
- Migrations: Database migrations for schema changes.
- Database Functions: Database migrations for SQL functions.

In the repository, there are many implemented SQL functions, for example, a function for getting categories with pagination and recursive filtering based on the parent category, a function for converting cart items into order items while checking the available quantity of the product and updating it, and a function for deleting an order while restoring the number of products.

There are also a number of infrastructure layer services, such as Hashing Service (for hashing passwords and tokens), File Service (for handling images) and Token Service (for issuing and validating access and update tokens).

#### Tests
The tests layer contains unit tests to ensure the correctness of the application.

## Installation

1. Clone the repository:
```
git clone git@github.com:Kapshtyk/fs18_CSharp_Teamwork.git
```
2. Navigate to the project directory:
```
cd fs18_CSharp_Teamwork/Ecommerce/
```
3. Restore dependencies:
```
dotnet restore
```
4. Navigate to the cicd (optional, if you have your own Postgres database, you can use it).
```
cd ../cicd
```
5. Run the docker container for the database (optional)
```
docker-compose -f docker-compose.local.yml up -d
```
6. Add appsettings.Development.json to the Ecommerce/Ecommerce.Infrastructure folder and specify your connection string (if you used our docker setup, you can use the example file as is).
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5435;Database=postgres;Username=postgres;Password=postgres"
  },
  "Redis": {
    "ConnectionString": "localhost:6380"
  },
  "AuthSettings": {
    "PrivateKey": "MIICWwIBAAKBgHZO8IQouqjDyY47ZDGdw9jPDVHadgfT1kP3igz5xamdVaYPHaN24UZMeSXjW9sWZzwFVbhOAGrjR0MM6APrlvv5mpy67S/K4q4D7Dvf6QySKFzwMZ99Qk10fK8tLoUlHG3qfk9+85LhL/Rnmd9FD7nz8+cYXFmz5LIaLEQATdyNAgMBAAECgYA9ng2Md34IKbiPGIWthcKb5/LC/+nbV8xPp9xBt9Dn7ybNjy/blC3uJCQwxIJxz/BChXDIxe9XvDnARTeN2yTOKrV6mUfI+VmON5gTD5hMGtWmxEsmTfu3JL0LjDe8Rfdu46w5qjX5jyDwU0ygJPqXJPRmHOQW0WN8oLIaDBxIQQJBAN66qMS2GtcgTqECjnZuuP+qrTKL4JzG+yLLNoyWJbMlF0/HatsmrFq/CkYwA806OTmCkUSm9x6mpX1wHKi4jbECQQCH+yVb67gdghmoNhc5vLgnm/efNnhUh7u07OCL3tE9EBbxZFRs17HftfEcfmtOtoyTBpf9jrOvaGjYxmxXWSedAkByZrHVCCxVHxUEAoomLsz7FTGM6ufd3x6TSomkQGLw1zZYFfe+xOh2W/XtAzCQsz09WuE+v/viVHpgKbuutcyhAkB8o8hXnBVz/rdTxti9FG1b6QstBXmASbXVHbaonkD+DoxpEMSNy5t/6b4qlvn2+T6a2VVhlXbAFhzcbewKmG7FAkEAs8z4Y1uI0Bf6ge4foXZ/2B9/pJpODnp2cbQjHomnXM861B/C+jPW3TJJN2cfbAxhCQT2NhzewaqoYzy7dpYsIQ=="
  },
  "AllowedOrigin": "http://localhost:3000"
}
```
   
## Running the Application
1. Build the project (from the Ecommerce/Ecommerce.Infrastructure folder):
```
dotnet build
```
2. Apply the migrations (make sure the database is up and running and you can use the default `public' schema)
```
dotnet ef database update
```
3. Run the project:
```
dotnet run 
```
4. Open your browser and navigate to `http://localhost:5169`.

## Testing
1. Run tests:
```
dotnet test Ecommerce.Tests/
```

## API Documentation Screenshot

<img width="1507" alt="Screenshot 2024-09-20 at 13 42 32" src="https://github.com/user-attachments/assets/0ffbd0d5-069f-4909-aa17-1b9ee4cae0ae">

## Contributors:

- [Mahmood Sharifizemeidani](https://github.com/mahmood-sharifi)
- [Roman Kuzero](https://github.com/rokuzzz)
- [Arseniiy Kapshtyk](https://github.com/kapshtyk)

---
