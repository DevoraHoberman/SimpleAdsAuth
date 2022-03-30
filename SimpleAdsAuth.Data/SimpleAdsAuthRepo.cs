using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimpleAdsAuth.Data
{
    public class SimpleAdsAuthRepo
    {
        private string _connectionString;

        public SimpleAdsAuthRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Users(Name, Email, PasswordHash)
                            VALUES(@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", user.Name);            
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(password));
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;
        }

        public User GetByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT TOP 1 * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            conn.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new User
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],               
                Email = (string)reader["Email"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }

        public List<Ad> GetAds()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT a.*, u.Name FROM Ad a JOIN Users u ON a.UserId = u.Id";
            conn.Open();
            var ads = new List<Ad>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    Description = (string)reader["Description"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"],
                    UserName = (string)reader["Name"]
                });
            }
            return ads;
        }

        public void DeleteAd(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Ad WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void NewAd(Ad ad, int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Ad(Title, Description, PhoneNumber, Date, UserId)
                                VALUES(@title, @description, @phonenumber, GETDATE(), @userid)";
            cmd.Parameters.AddWithValue("@title", ad.Title);
            cmd.Parameters.AddWithValue("@description", ad.Description);
            cmd.Parameters.AddWithValue("@phonenumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@userid", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Ad> MyAds(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT a.*, u.Name FROM Ad a JOIN Users u ON a.UserId = u.Id WHERE u.id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var ads = new List<Ad>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    Description = (string)reader["Description"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"],
                    UserName = (string)reader["Name"]
                });
            }
            return ads;
        }
    }
}
