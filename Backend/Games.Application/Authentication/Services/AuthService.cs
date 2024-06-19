using Games.Application.Authentication.Models;
using Games.Application.Infrastructure;
using Games.Application.Persistence;
using Games.Application.TicTacToe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.Authentication.Services
{
    public class AuthService(AuthDbContext authDbContext, Encryptor encryptor)
    {
        private readonly AuthDbContext _context = authDbContext;
        private readonly Encryptor _encryptor = encryptor;

        private readonly string _encryptorKey = "kdhst5s9f72agfa9s7g30a5s7h5j7w20";

        public async Task<Result> RegisterUser(AppUser user)
        {
            if (user == null)
            {
                return Result.Error("No user given");
            }

            if (await UserExists(user))
            {
                return Result.Error("This username or email is already taken");
            }

            await AddAppUserToDatabase(user);
            return Result.Success("Successfully registered");
        }

        public async Task<Result> Login(LoginUser user)
        {
            AppUser appUser = new() 
            {
                Username = user.Username,
                Password = user.Password,
            };

            if (!await UserExists(appUser))
            {
                return Result.Error("There is no user with this username or email");
            }

            if (!EnteredPasswordMatchUserPassword(user.Password, await GetUserEncryptedPassword(user)))
            {
                return Result.Error("Wrong password");
            }

            return Result.Success("Successfully logged in");
        }

        public async Task<int?> GetUserIdFromDataBase(LoginUser user)
        {
            int? id = null;
            var dbAppUsers = await _context.AppUsers.ToListAsync();
            foreach (var dbAppUser in dbAppUsers)
            {
                if (dbAppUser.Username == user.Username)
                {
                    id = dbAppUser.Id; 
                }
            }

            return id;
        }

        public async Task<string> GetUserEncryptedPassword(LoginUser user)
        {
            var dbUser = await _context.AppUsers.FindAsync(await GetUserIdFromDataBase(user));
            return dbUser!.Password;
        }

        private bool EnteredPasswordMatchUserPassword(string enteredPassword, string userPassword)
        {
            string password = _encryptor.EncryptString(_encryptorKey, enteredPassword);
            return password == userPassword;
        }

        private async Task AddAppUserToDatabase(AppUser user)
        {
            var appUserDbModel = new Persistence.AppUsers() 
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Password = _encryptor.EncryptString(_encryptorKey, user.Password)
            };

            var appUsersDb = await _context.AppUsers.AddAsync(appUserDbModel);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> UserExists(AppUser user)
        {
            var appUsers = await GetAppUsers();

            return appUsers.Any(appUser =>
                        appUser.Username == user.Username
                   ) ||
                   appUsers.Any(appUser =>
                       appUser.Email == user.Email)
                   && user.Email != null;
        }

        private async Task<List<AppUser>> GetAppUsers()
        {
            var appUsers = ConvertDbModelListToAppUserList(
                await _context.AppUsers.ToListAsync()
            );

            return appUsers;
        }

        private List<AppUser> ConvertDbModelListToAppUserList(List<AppUsers> dbModelAppUsers)
        {
            List<AppUser> appUsers = new();
            
            foreach (AppUsers dbModelUser in dbModelAppUsers)
            {
                appUsers.Add(AppUser.FromAppUsersPersistance(dbModelUser));
            }

            return appUsers;
        }
    }
}
