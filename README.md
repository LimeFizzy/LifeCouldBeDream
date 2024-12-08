# Memory Improvement Web App

## By LifeCouldBeDream

-------------------------------------

### Team name - Life could be dream

#### Team leader
- Leonardas Sinkevičius
#### Team members
- Nikita Motiejaitis
- Aleksej Krasavcev
- Ramunė Riaubaitė

-------------------------------------

## Overview

This project is a web application aimed at improving cognitive skills - specifically memory. The application provides a series of memory improvement games: Long Number Memory game, Shimp Test, and Sequence Memorization. The goal is to offer users fun and interactive ways to challenge and enhance their memory through engaging activities.

-------------------------------------

## Key Features

1. **Memory Improvement Games**
   - **Long Number Memory:** Users are shown a long sequence of numbers and must recall them in the correct order.
   - **Chimp Test:** A visual memory game where users must remember and select numbers or items in the correct sequence as they disappear.
   - **Sequence Memorization:** Users are shown a sequence of items and must recall the exact order.

2. **User Progress Tracking**
   - Registered users can track and compare their progress in leaderboards.

-------------------------------------

## Tech Stack

- **Backend:**
  - **C# .NET Core Web API:** The server-side logic is built using .NET Core to provide APIs that handle game logic, data persistence, and user interactions.
  
- **Frontend:**
  - **React with TypeScript:** The frontend is built with React, a modern JavaScript library, using TypeScript.
  
- **Database:** 
  - Our project uses MySQL as the primary database management system

-------------------------------------

## Installation Instructions

### Prerequisites

For setting up the project you will need the following:

- **.NET SDK** (Version 6 or later) – for the backend.
- **Node.js** (Version 18 or later) – for the frontend.
- **npm** – for managing frontend packages.
- **A code editor** such as Visual Studio Code or Visual Studio.

### Backend (Web API) Setup

1. **Clone the Repository**
2. **Navigate to the Backend Directory** - run `cd API`
3. **Install Dependencies** - run `dotnet restore`
4. **Build the Project** - run `dotnet build`
5. **Run the Project** - run `dotnet run`

### Frontend (React + TypeScript) Setup
1. **Navigate to the Frontend Directory** - run `cd frontend`
2. **Install Dependencies** - run `npm install`
3. **Start the Development Server** - run `npm start`

After this, you should see a link in your console. Click it and it should take you to our home page.

-------------------------------------

## Usage Guide
 - In order to track your progress and compare yourself with other users use `Sign Up` to create your account. 
 - Log in to save your scores. Your scores you can find in `Leaderboard` section. Select game type which scores you want to see.
 - Enjoye the games!

-------------------------------------

## Configuration
 - The database connection is configured in the `appsettings.json` file in the backend.
 - **Backend:** Runs on `http://localhost:5217` by default.
 - **Frontend:** Runs on `http://localhost:5173` by default

-------------------------------------

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
