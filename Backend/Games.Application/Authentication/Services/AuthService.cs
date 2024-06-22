using Games.Application.Authentication.Models;
using Games.Application.Infrastructure;
using Games.Application.Persistence;
using Games.Application.TicTacToe.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Games.Application.Authentication.Services
{
    public class AuthService(GamesDbContext gamesDbContext, JwtService jwt)
    {
        private readonly GamesDbContext _context = gamesDbContext;
        private readonly JwtService _jwt = jwt;

        public async Task<Result> RegisterUser(RegisterDto dto)
        {
            if (dto == null)
            {
                return Result.Error("No user given");
            }

            AppUser appUser = new()
            {
                Username = dto.Username,
                Password = dto.Password,
            };
            if (await UserExists(appUser))
            {
                return Result.Error("This username or email is already taken");
            }

            await RegisterUserInDatabase(dto);
            return Result.Success("Successfully registered");
        }

        public async Task<Result<string>> Login(LoginDto dto)
        {
            AppUser appUser = new() 
            {
                Username = dto.Username,
                Password = dto.Password,
            };

            if (!await UserExists(appUser))
            {
                return Result<string>.Error("There is no user with this username or email");
            }

            if (!await EnteredPasswordMatchUserPassword(dto))
            {
                return Result<string>.Error("Wrong password");
            }

            return Result<string>.Success("", "Successfully logged in");
        }

        public async Task<int> GetUserIdFromDataBase(LoginDto dto)
        {
            int id = 0;
            var dbAppUsers = await _context.AppUsers.ToListAsync();
            foreach (var dbAppUser in dbAppUsers)
            {
                if (dbAppUser.Username == dto.Username)
                {
                    id = dbAppUser.Id; 
                }
            }

            return id;
        }

        private async Task<(string, byte[])> GetUsersHashedPaswordAndSalt(int userId)
        {
            var dbUser = await _context.AppUsers.FindAsync(userId);

            string password = dbUser!.Password;
            byte[] salt = dbUser!.Salt;

            return (password, salt);
        } 

        private async Task<bool> EnteredPasswordMatchUserPassword(LoginDto dto)
        {
            var (hashedPassword, salt) = await GetUsersHashedPaswordAndSalt(await GetUserIdFromDataBase(dto));
            bool isPasswordCorrect = PasswordHash.VerifyPassword(dto.Password, hashedPassword, salt);
            return isPasswordCorrect;
        }

        private async Task RegisterUserInDatabase(RegisterDto dto)
        {
            var (password, salt) = PasswordHash.HashPasword(dto.Password);

            var appUserDbModel = new Persistence.AppUsers() 
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = password,
                Salt = salt,
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
