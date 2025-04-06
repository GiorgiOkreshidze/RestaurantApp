# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2025-04-04
### Added

#### 1. Reservation Management for Waiters

- Add POST /api/reservations/waiter endpoint for waiter to manage reservations

#### 2. Food Menu Browsing

- Create GET /dishes and GET /dishes/{id} endpoints
- Create GET /location/{id} endpoint

#### 3. Reports for Restaurant Managers

- Create automated report generation for session statistics

### Changed

- Update GET /reservations endpoint to support waiter-specific use cases
- Include locationId in waiter profile response
- Removed Authorizer from the refresh token process

### Fixed

- Prevent multiple identical reservations for the same date and time by the same user

## [1.0.0] - 2025-03-20
### Added

#### 1. User Profile Creation

- Generate Syndicate Project
- Configure Cognito User Pool and API Gateway for the restaurant backend
- Create project structure for the restaurant BE API handler
- Implement Sign-In (**POST /signin**) and Sign-Up (**POST /signup**) logic
- Add Access-Control-Allow headers

#### 2. User Login and Account Access

- Implement Logout Handling (**POST /signout**)
- Implement Refresh Token Functionality (**POST /auth/refresh**)

#### 3. Automatic Role Assignment

- Expose User Profile API for User Information (**GET /users/profile**)
- Configure DynamoDB for the restaurant backend
- Implement **Customer** and **Visitor** Roles
- Create DB table to store waiter email addresses

#### 4. Restaurant Location and Ratings

- Create **GET /dishes/popular** endpoint
- Create **GET /locations** endpoint
- Create **GET /locations/{id}/feedbacks** endpoint with the following features:
  - **Support for feedback types:** SERVICE_QUALITY, CUISINE_EXPERIENCE
  - **Pagination support:** page number, page size
  - **Sorting support:** sort by property, asc/desc
- Create **GET /locations/{id}/speciality-dishe**s endpoint
- Implement data seeding script to load mock data into DynamoDB

#### 5. Table Viewing and Selection

- Create **GET /locations/select-options** endpoint
- Create **GET /bookings/tables** endpoint with filters:
    - **Location ID**
    - **Date**
    - **Time**
    - **Number of guests**
- Integrate Swagger for API documentation
- Refactor codebase for better structure and readability
- Implement image storage using Amazon S3 with public URL generation
- Create Users table

#### 6. Table Reservations

- Create **POST /reservations/client** endpoint
- Implement validation for **POST /api/reservations/client** to prevent overbooking:
- No double booking for the same table, time, and location
- Limit to 10 guests per reservation
- Create **DELETE /api/reservations/{id}** endpoint
- Create **GET /api/reservations** endpoint

### Changed

- Replace simple string IDs with GUIDs
- Migrate to UTC from local time

### Fixed

- Logout Functionality (Correct authorization logic to use Amazon Cognito pool)
- CORS issues when sending access tokens in custom headers
