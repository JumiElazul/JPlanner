using JPlanner.Exceptions;
using JPlanner.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JPlanner.Database
{
    internal static class SQLiteHandler
    {
        private static readonly string _connectionString = "Data Source=database.db;";

        private static SqliteConnection OpenConnection()
        {
            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public static void InitializeDatabase()
        {
            using SqliteConnection connection = OpenConnection();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
                @"CREATE TABLE IF NOT EXISTS Users (
                UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE
                );";
            command.ExecuteNonQuery();

            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS Meals (
                MealId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Entry TEXT NOT NULL,
                Calories INTEGER NOT NULL,
                TimeStamp TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
                );";
            command.ExecuteNonQuery();
        }

        public static void CreateUser(string username)
        {
            using SqliteConnection connection = OpenConnection();
            SqliteCommand command = connection.CreateCommand();

            string commandText = $"INSERT INTO Users (Username) VALUES (@Username);";
            command.CommandText = commandText;
            command.Parameters.AddWithValue("@Username", username);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void CreateMealForUser(string username, MealEntry mealEntry)
        {
            using SqliteConnection connection = OpenConnection();
            using SqliteCommand getUserIdCommand = connection.CreateCommand();
            getUserIdCommand.CommandText = "SELECT UserId FROM Users WHERE Username = @Username";
            getUserIdCommand.Parameters.AddWithValue("@Username", username);
            object userIdResult = getUserIdCommand.ExecuteScalar();

            if (userIdResult != null)
            {
                int userId = Convert.ToInt32(userIdResult);
                using SqliteCommand addMealCommand = connection.CreateCommand();
                addMealCommand.CommandText = $"INSERT INTO Meals (UserId, Entry, Calories, TimeStamp) VALUES (@UserId, @Entry, @Calories, @TimeStamp);";
                addMealCommand.Parameters.AddWithValue("@UserId", userId);
                addMealCommand.Parameters.AddWithValue("@Entry", mealEntry.Entry);
                addMealCommand.Parameters.AddWithValue("@Calories", mealEntry.Calories);
                addMealCommand.Parameters.AddWithValue("@TimeStamp", mealEntry.TimeStamp.ToString());

                addMealCommand.ExecuteNonQuery();
            }
            else
            {
                throw new DbUserNotFoundException(username);
            }
        }
    }
}
