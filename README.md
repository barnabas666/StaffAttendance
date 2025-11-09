StaffAttendance

===

\# Staff Attendance System



A modern, secure, and flexible staff management and attendance tracking solution built with .NET 8.0.  

Supports web, desktop, and API access, with multi-database backend support (PostgreSQL, SQL Server, SQLite).



---



\## Features



\- \*\*Web App\*\*: Staff registration, profile management, and administration via a user-friendly web interface.

\- \*\*Desktop App\*\*: Portable WPF application for recording check-ins and check-outs, suitable for on-premises use.

\- \*\*REST API\*\*: Central backend for all business logic, data access, and authentication.

\- \*\*Multi-Database Support\*\*: Easily switch between PostgreSQL (recommended), SQL Server, or SQLite for the main staff database.

\- \*\*Secure Authentication\*\*: Uses ASP.NET Core Identity and JWT tokens for secure access.

\- \*\*Role Management\*\*: Supports roles like Administrator and Member.

\- \*\*Downloadable Desktop App\*\*: Available from the web app for end users.



---



\## Solution Structure



\- \*\*StaffAtt.Web\*\*: ASP.NET Core Razor Pages web application (UI for staff and admins).

\- \*\*StaffAtt.Desktop\*\*: WPF desktop application (portable, connects via API).

\- \*\*StaffAttApi\*\*: ASP.NET Core Web API (all business logic and data access).

\- \*\*StaffAttLibrary\*\*: Core business logic and data access (used only by API).

\- \*\*StaffAtt.Identity\*\*: Identity system for authentication (used by API and web).

\- \*\*StaffAttShared\*\*: Shared DTOs and enums for communication between clients and API.



---



\## Database Support



\- \*\*Main Staff Database\*\*:  

&nbsp; - PostgreSQL (default, recommended for deployment)  

&nbsp; - SQL Server (with stored procedures, scripts in `/StaffAttDB/`)  

&nbsp; - SQLite (for local/testing, queries in `/StaffAttLibrary/Queries/SQLite/` and `/StaffAttLibrary/Queries/PostgreSQL/`)

\- \*\*Identity Database\*\*:  

&nbsp; - SQLite (for authentication, configured in `StaffAtt.Web/appsettings.json`)



A database diagram is available at the solution root: `StaffAttDbDiagram.png`.



---



\## How It Works



\- Both the web and desktop apps communicate \*\*only with the API\*\* (never directly with the database or business logic).

\- The API handles all authentication, authorization, and business rules.

\- All data exchanged between clients and API uses DTOs from `StaffAttShared`.



---



\## Getting Started



1\. \*\*Clone the repository\*\*

2\. \*\*Configure your database\*\* (PostgreSQL recommended; see `appsettings.json` in each app)

3\. \*\*Run the API\*\* (`StaffAttApi`)

4\. \*\*Run the Web App\*\* (`StaffAtt.Web`) or \*\*Desktop App\*\* (`StaffAtt.Desktop`)

5\. \*\*Access the web app\*\* in your browser, or download and run the desktop app from the web interface



---



\## Security



\- All sensitive operations require authentication via JWT tokens.

\- The desktop app is portable and does not store secrets; all secure operations go through the API.



---



\## Contributing



Contributions are welcome! Please open issues or submit pull requests.



---



\## License



\[MIT](LICENSE)



---



\*\*For more details, see the full documentation in `documentation.txt` or the database diagram.\*\*

