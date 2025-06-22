# 🌿Agri-Energy Connect Application  

This application guide will walk you through the process of setting up the Agri Energy connect application on your machine, using .NET 9 in Visual Studio Code with an SQL database. It will also include instructions for creating the database, as well as adding the tables to the application should it be needed  

---

`application walkthrough` [YOUTUBE VIDEO](https://youtu.be/wDp07HFADWY)

## ‼️Prerequisites  
**N.B.** References for the code can be found at the bottom of the `Program.cs` file  

Before starting, ensure you have the following items installed on your machine:  

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)  
- [Visual Studio Code](https://code.visualstudio.com/)  
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/ssms/download-sql-server-management-studio-ssms)  

Once these are downloaded, ensure that you install these software's, and restart your machine to ensure changes to your device are made.  

In terms of SQL Server, ensure you have a running SQL Server instance. To this, open SQL Server Management Studio and connect to a database. A guide for setting up SSMS to work on your local machine can be found [here](https://learn.microsoft.com/en-us/ssms/tutorials/ssms-configuration)  

---

## Step 1: Clone the Repository  
Navigate to the repository found [here](https://github.com/st10385722/AgriEnergyConnectProject). Steps to clone a repository to Visual Studio Code can be found [here](https://www.jcchouinard.com/git-clone-github-repository-vscode/)  

1. Clone the project repository or download the project files from GitHub.  
2. Open the project folder in Visual Studio Code.  

---

## Step 2: Set Up the Database to work with the application  
1. Open SQL Server Management Studio (SSMS).  
2. Connect to your SQL Server instance.  
3. Run the provided database script (`st10385722-agri-energy-connect-db.sql`) to create the database and tables.  

---

## Step 3: Scaffold the models into the project folder (If Needed)  
If the models in the Models folder do not exist or need to be updated, you can scaffold them from the newly created database. To do this, do the following steps:  

1. Open the terminal in VS Code. This is done by going to the Nav bar at the top > terminal > new terminal  
2. You should be in the project folder already, but if not, do the following:  
   ```bash
   cd .<Your_Project_Folder>
   cd .\Agri-EnergyConnect

Now, you should be in the project folder. you will know that you are if the terminal looks something like this:
`C:\Users\<Your Name Space>\source\vsc\Agri-EnergyConnect>`

Run the following command to scaffold the database tables into the Models folder:
    N.B. Replace `<Your-Server-Name-Here>` with the name of your SQL Server instance. You can get this copying the name of the database engine you connect to when you open SSMS

    dotnet ef dbcontext scaffold "Server=<Your-Server-Name-Here>;Database=st10385722-agri-energy-connect-db;Trusted_Connection=True;TrustServerCertificate=True;" --force```

`The --force flag overwrites existing models if they already exist.`

## Step 4: Configure the Application

Open the `appsettings.json` file in the project folder in Visual Studio Code

Update the DefaultConnection string under ConnectionStrings with your SQL Server name:
Replace `<Your-Server-Name-Here>` with your SQL Server name, the same one that you used for the command above

```bash
"AllowedHosts": "*",
    "ConnectionStrings": {
    "DefaultConnection": "Server=<Your-Server-Name-Here>;Database=st10385722-agri-energy-connect-db;Trusted_Connection=True;TrustServerCertificate=True;"
  }
```

## Step 5: Run the Application

Open the terminal in Visual Studio Code using the steps before

Navigate to the project folder:

Run the following command to build and run the application:
```bash
dotnet run
```

Once the application starts, it will display a message like:
    Open your browser and navigate to the URL `(e.g., http://localhost:5000)` to access the application.

## 📖 Additional Notes

`Authentication`: The application uses role-based authentication (farmer, employee, admin). Ensure you have seeded roles and users in the database by running the application

`Database Seeding`: The application automatically create data for the application in order to test its functionality, if they do not exist. Check out the Program.cs file for details.


## Default Login Details 
**Admin**

    email - admin@test.com	
    password - Adm!n1234

**Employee**

    email - employee@test.com
    password - Emp!oyee1234

**Farmer**	

    email - farmer@test.com
    password - F@rmer1234

File Uploads: When adding a new product, images are stored in the database itself, and are pulled automatically.


## 🛠️ Troubleshooting

`Database Connection Issues`: Ensure your SQL Server is running and the connection string in appsettings.json matches the SQL Server name of your computer.

`Missing Models`: Run the scaffold command in Step 3 to regenerate the models.

`Port Conflicts`: If the default port (5000) is in use, the application will use a different port. Check the terminal output for the correct URL.


## 📃Agri Energy connect application features

This prototype has the following features:

`Role based authorization`, separating views depending on the role of the user

`User account authentication`, where users have to be logged into the application in order to use it

`Product hosting`, where farmers can upload and view their products to contribute to the Green Energy push

`Filtering`, with the ability to sort by the following metrics:

- Product type

- A range of dates to show a list of products in that range for when it was uploaded

- A range of date to filter farmers based on their harvesting date

`Error handling`, which gracefully handles invalid inputs and unauthorized access to pages

## Roles

This application has 3 roles, which separate the functions of the application. These roles are:

**Admin**

- Admins have the ability to create employees, an essential user needed for managing the platform

**Employee**

- Employees have the ability to view all farmer information, as well as their products. They also possess the ability to filter various farmer information and products to show the best products

**Farmer**

- Farmers possess the ability to create a new product, view their own products, and to fill in their farm details. This includes its name, their types of crops and livestock as well as their harvesting date


🚀 **With this guide, you can now explore the Argi Energy Connect app and witness its future potential**

## References
Make a ReadMe. n.d. Make a readme. [Online]. Available at: https://www.makeareadme.com/ [Accessed 12 May 2025]


Bootswatch. n.d. Free themes for bootstrap. [Online]. Available at: https://bootswatch.com/ [Accessed 12 May 2025]


W3School. n.d. HTML Styles - CSS. [Online]. Available at: https://www.w3schools.com/html/html_css.asp [Accessed 12 May 2025]

## Changes made
1. Updated the product type sorting function for employees to use a `drop down` rather than `typing out` the category, allowing for a more seamless flow of logic

2. Updated the `homepage` view to have a semi- transparent background to allow for `more readable` text 


G, Courtney J. 2023. Essential farm records and data you need to be tracking, FarmBite.com. 27 September 2023. [Online]. Available at: //https://www.farmbrite.com/post/essential-farm-records-and-data-you-need-to-be-tracking [Accessed 12 May 2025]


LearnSQL.com Team. 2023. SQL Server cheat sheet, LearnSQL.com. 4 September 2023. [Online]. Available at: https://learnsql.com/blog/sql-server-cheat-sheet/#:~:text=This%20cheat%20sheet%20is%20a%20comprehensive%20guide%20to,commands%20such%20as%20SELECT%2C%20INSERT%2C%20UPDATE%2C%20and%20DELETE. [Accessed 12 May 2025]


AppliedK. 2024. ASP NET Core How to seed data at design time & run time by DbContext AppliedK. [video online]. Available at: https://www.youtube.com/watch?v=k6Il3YYputM [Accessed 13 May 2025]


GeeksForGeeks.2024. Repository design pattern, 1 November 2024. [Online]. Available at: https://www.geeksforgeeks.org/repository-design-pattern/ [Accessed 12 May 2025]


