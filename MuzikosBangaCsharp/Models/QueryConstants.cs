using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MuzikosBangaCsharp.Models
{
    public static class QueryConstants
    {
        public static readonly string queryGetUsers = "Select * From users";
        public static readonly string queryGet10Albums = "SELECT* FROM albums ORDER BY RAND() LIMIT 10";
        public static readonly string queryCheckLogin = "SELECT id, username FROM users Where username = @username and password = @password";
        public static readonly string queryUserRegistration = "INSERT INTO users(username, firstName, lastName, email, password, signUpDate, profilePic) VALUES(@username, @firstName, @lastName, @email, @password, @signUpDate, @profilePic)";

    }
}