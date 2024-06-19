using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.Authentication.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string? Email { get; set; }
    public string Password { get; set; } = "";

    public static AppUser FromAppUsersPersistance(Persistence.AppUsers dbModelAppUser) 
    {
        AppUser appUser = new AppUser()
        {
            Id = dbModelAppUser.Id,
            Username = dbModelAppUser.Username,
            Email = dbModelAppUser.Email,
            Password = dbModelAppUser.Password
        };
        return appUser;
    }
}