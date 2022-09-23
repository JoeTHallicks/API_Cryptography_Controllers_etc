using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DistSysAcw.Models
{
    // Every database table must have a key so that the database can identify each record as a unique data item.

    #region 

    public class User
    {
        public enum Roles
        {
            Admin,
            User
        }
        [Key] // Primary Key.
        public string ApiKey { get; set; }
        [Required] // Makes the column not nullable in sql.
        public string UserName { get; set; }
        [Required] public Roles Role { get; set; }
        public ICollection<Log> Logs { get; set; } // Collection of Logs.
                                                   // TODO: Create a User Class for use with Entity Framework
                                                   // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key
    }
    #endregion

    #region 

    public class Log
    {
        public Log() { }
        public Log(string logString, DateTime logDateTime)
        {
            LogString = logString;
            LogDateTime = logDateTime;
        }
        [Key] public int LogId { get; set; } // Auto incremented by DB
        [Required] public string LogString { get; set; } // Describes what happened.
        [Required] public DateTime LogDateTime { get; set; }
    }
    public class LogArchive
    {
        public LogArchive(int logId, string apiKey, string logString, DateTime logDateTime)
        {
            LogId = logId;
            ApiKey = apiKey;
            LogString = logString;
            LogDateTime = logDateTime;
        }
        [Key] public int LogArchiveId { get; set; } // Auto incremented by DB.
        [Required] public string ApiKey { get; set; } // Link log to user.
        [Required] public int LogId { get; set; } // Link log to original log.
        [Required] public DateTime LogDateTime { get; set; }
        [Required] public string LogString { get; set; }

        
    }

    #endregion

    // Methods which database read/write.
    public static class UserDatabaseAccess
    {
        #region 
        public static User PostUser(UserContext dbContext, string userName) // Create a new user in the database
        {
            var role = User.Roles.User; 
            if (!dbContext.Users.Any()) role = User.Roles.Admin;//set role to Admin.
            var newUser = new User
            {
                ApiKey = Guid.NewGuid().ToString(),
                Role = role,
                UserName = userName,
                Logs = new List<Log>()
            };
            dbContext.Users.Add(newUser);
            LogAction(dbContext, newUser,// dbContext is saved when logging so wont happen again.
                $"PostUser: New " +
                $"{role} {userName} added to the system.");
            return newUser;
        }
        public static bool UserApiKeyExists(UserContext dbContext, string apiKey)
        {
            return dbContext.Users.FirstOrDefault(u => u.ApiKey == apiKey) != null;
        }
        public static bool UserNameExists(UserContext dbContext, string userName)
        {
            return dbContext.Users.FirstOrDefault(u => u.UserName == userName) != null;
        }
        public static bool UserExists(UserContext dbContext, string apiKey, string userName)
        {
            return dbContext.Users.FirstOrDefault(u => u.ApiKey == apiKey && u.UserName == userName) != null;
        }
        public static User GetUser(UserContext dbContext, string apiKey, string username) // Return a user and their logs with a given ApiKey.
        {
            User user = null;
            if (apiKey != null)
                user = dbContext.Users .Include(u => u.Logs)                  
                .FirstOrDefault(u => u.ApiKey == apiKey);
            if (username != null)
                user = dbContext.Users.Include(u => u.Logs)
                    .FirstOrDefault(u => u.UserName == username);// Doesn't match case for some reason when doing change role.
            return user;
        }
        #endregion

        #region 
    
        public static bool DeleteUser(UserContext dbContext, string apiKey, string username) // Remove a user.
        {
            var user = GetUser(dbContext, apiKey, null);       
            if (user == null || user?.UserName != username) return false; // If user does not exist supplied username.          
            if (!ArchiveUserLogs(dbContext, user.ApiKey)) // Archive user logs, if unsuccessful return false.
            {
                LogAction(dbContext, user,
                    $"DeleteUser: Failed to archive logs and remove user: {user.UserName}");
                return false;
            }         
            dbContext.Users.Remove(user); // Remove user.
            dbContext.SaveChanges();
            return true;
        }
        #endregion

        #region 

        public static bool ChangeUserRole(UserContext dbContext, ChangeRole jsonChangeRole) // Change a users role.
        {
            var user = GetUser(dbContext, null, jsonChangeRole.Username);
            if (user == null) return false;
            user.Role = Enum.Parse<User.Roles>(jsonChangeRole.Role);
            LogAction(dbContext, user,
                $"ChangeUserRole: {jsonChangeRole.Username} " +
                $"role has been changed to {jsonChangeRole.Role}.");
            return true;
        }
        #endregion

        #region 
        
        public static void LogAction(UserContext dbContext, User user, string logString)// Create log of user activity.
        {
            var log = new Log
            {
                LogString = logString,
                LogDateTime = DateTime.Now
            };
            user.Logs.Add(log);
            dbContext.SaveChanges();
        }     
        public static bool ArchiveUserLogs(UserContext dbContext, string apiKey)
        {
            var user = GetUser(dbContext, apiKey, null);
            if (user?.Logs == null) return false;         
            foreach (var log in user.Logs)
                dbContext.LogArchives.Add(new LogArchive(log.LogId, apiKey, log.LogString, log.LogDateTime));
            dbContext.SaveChanges();
            return true;
        }
        #endregion
    }
    public class ChangeRole
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }
}