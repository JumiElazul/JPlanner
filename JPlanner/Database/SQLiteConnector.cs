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
            int userId = GetUserId(username);

            if (IsValidUserId(userId))
            {
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

        public static void DeleteMealForUser(string username, MealEntry mealEntry)
        {
            using SqliteConnection connection = OpenConnection();
            int userId = GetUserId(username);

            if (IsValidUserId(userId))
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"
                DELETE FROM Meals
                WHERE UserId = @UserId
                AND Entry = @Entry
                AND Calories = @Calories
                AND TimeStamp = @TimeStamp;";
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Entry", mealEntry.Entry);
                command.Parameters.AddWithValue("@Calories", mealEntry.Calories);
                command.Parameters.AddWithValue("@TimeStamp", mealEntry.TimeStamp.ToString());

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new DbEntryNotFoundException(mealEntry, username, userId);
                }
            }
        }

        public static int GetUserId(string username)
        {
            using SqliteConnection connection = OpenConnection();
            SqliteCommand getUserIdCommand = connection.CreateCommand();

            getUserIdCommand.CommandText = "SELECT UserId FROM Users WHERE Username = @Username";
            getUserIdCommand.Parameters.AddWithValue("@Username", username);
            object userIdResult = getUserIdCommand.ExecuteScalar();

            if (userIdResult != null)
            {
                return Convert.ToInt32(userIdResult);
            }
            return -1;
        }

        public static List<MealEntry> GetMealEntriesForUser(string username)
        {
            List<MealEntry> meals = new List<MealEntry>();
            using SqliteConnection connection = OpenConnection();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                @"SELECT m.Entry, m.Calories, m.TimeStamp
                  FROM Meals m
                  JOIN Users u ON m.UserId = u.UserId
                  WHERE u.Username = @Username;";

            command.Parameters.AddWithValue("@Username", username);

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string entry = reader.GetString(0);
                int calories = reader.GetInt32(1);
                string timeStamp = reader.GetString(2);

                meals.Add(new MealEntry(entry, calories, DateTime.Parse(timeStamp)));
            }

            return meals;
        }

        private static bool IsValidUserId(int userId) => userId >= 0;

#if DEBUG
        public static void PrintMealEntriesForUser(string username)
        {
            List<MealEntry> meals = new List<MealEntry>();
            using SqliteConnection connection = OpenConnection();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                @"SELECT m.Entry, m.Calories, m.TimeStamp
                  FROM Meals m
                  JOIN Users u ON m.UserId = u.UserId
                  WHERE u.Username = @Username;";

            command.Parameters.AddWithValue("@Username", username);

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string entry = reader.GetString(0);
                int calories = reader.GetInt32(1);
                string timeStamp = reader.GetString(2);

                meals.Add(new MealEntry(entry, calories, DateTime.Parse(timeStamp)));
            }

            foreach (MealEntry entry in meals)
            {
                Console.WriteLine(entry);
            }
        }
#endif
    }
}
