**BlogPost123 - ASP.NET Core Razor Pages Blogging Platform**

**Live Demo**
http://blogpost123.runasp.net/

Overview

BlogPost123 is a full-stack blogging web application built using ASP.NET Core Razor Pages and SQL Server.
The application allows users to create, manage, and publish blog posts through a clean and responsive interface.

This project demonstrates:

ASP.NET Core Razor Pages
Entity Framework Core
SQL Server Integration
CRUD Operations
Rich Text Editor Integration
Authentication & Authorization
Deployment on cloud hosting
Features
Authentication
User Registration
User Login/Logout
ASP.NET Core Identity Integration
Blog Management
Create Blog Posts
Edit Existing Posts
Delete Posts
View All Posts
Rich Text Content Support
UI Features
Responsive Design
Bootstrap UI
TinyMCE Rich Text Editor
Clean Razor Pages Architecture
Database
SQL Server Database
Entity Framework Core Code First
Migrations Support
Tech Stack
Technology	Usage
ASP.NET Core Razor Pages	Web Framework
C#	Backend Language
Entity Framework Core	ORM
SQL Server	Database
ASP.NET Identity	Authentication
Bootstrap	UI Styling
TinyMCE	Rich Text Editor
Project Structure
BlogPost123/
│
├── Areas/
├── Data/
├── Migrations/
├── Models/
├── Pages/
├── wwwroot/
├── appsettings.json
├── Program.cs
└── BlogPost123.csproj
Setup Instructions
1. Clone Repository
git clone YOUR_REPOSITORY_URL
2. Open Project

Open the solution in:

Visual Studio

3. Configure Database

Update connection string inside:

appsettings.json

Example:

"ConnectionStrings": {
  "DefaultConnection": "YOUR_SQL_SERVER_CONNECTION_STRING"
}
4. Apply Database Migration

Run:

dotnet ef database update
5. Run Project
dotnet run

OR press:

F5

inside Visual Studio.

Deployment

The application is deployed on:

MonsterASP.NET Hosting
SQL Server Cloud Database

Deployment includes:

ASP.NET Core WebDeploy
SQL Server cloud integration
FTP/WebDeploy publishing
TinyMCE Configuration

TinyMCE is self-hosted inside the project to avoid:

API key dependency
domain restrictions
CDN limitations

Location:

wwwroot/lib/tinymce
Learning Outcomes

This project demonstrates practical experience with:

ASP.NET Core Web Development
Razor Pages Architecture
Database Design
Entity Framework Core
Authentication Systems
Cloud Deployment
Hosting Configuration
Debugging Production Issues
Future Improvements
Categories & Tags
Comments System
Search Functionality
Image Upload Support
Pagination
Admin Dashboard
REST API Integration
Dark Mode
Author

Hanshu Kumar
Experience in C# .NET, Game Development, and Web Development
License

This project is for learning and portfolio purposes.
