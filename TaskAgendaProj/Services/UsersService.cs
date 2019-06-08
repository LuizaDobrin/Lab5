using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskAgendaProj.Models;
using TaskAgendaProj.ViewModels;

namespace TaskAgendaProj.Services
{
    public interface IUsersService
    {
        LoginGetModel Authenticate(string username, string password);
        LoginGetModel Register(RegisterPostModel registerinfo);
        User GetCurentUser(HttpContext httpContext);

        IEnumerable<UserGetModel> GetAll();
        UserGetModel GetById(int id);
        UserGetModel Create(UserPostModel userModel);
        UserGetModel Upsert(int id, UserPostModel userPostModel);
        UserGetModel Delete(int id);
    }

    public class UsersService : IUsersService
    {
        private TasksDbContext context;

        private readonly AppSettings appSettings;

        public UsersService(TasksDbContext context, IOptions<AppSettings> appSettings)
        {
            this.context = context;
            this.appSettings = appSettings.Value;
        }


        public LoginGetModel Authenticate(string username, string password)
        {
            var user = context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == ComputeSha256Hash(password));

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.UserRole.ToString()),        //rolul vine ca string
                    new Claim(ClaimTypes.UserData, user.DataRegistered.ToString())        //DataRegistered vine ca string
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var result = new LoginGetModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Token = tokenHandler.WriteToken(token)
            };

            return result;
        }


        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            //TODO: Also use salt

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        public LoginGetModel Register(RegisterPostModel registerinfo)
        {
            User existing = context.Users.FirstOrDefault(u => u.Username == registerinfo.Username);

            if (existing != null)
            {
                return null;
            }
            context.Users.Add(new User
            {
                Email = registerinfo.Email,
                LastName = registerinfo.LastName,
                FirstName = registerinfo.FirstName,
                Password = ComputeSha256Hash(registerinfo.Password),
                Username = registerinfo.Username,
                UserRole = UserRole.Regular,
                DataRegistered = DateTime.Now

            });
            context.SaveChanges();
            return Authenticate(registerinfo.Username, registerinfo.Password);

        }


        public User GetCurentUser(HttpContext httpContext)
        {
            string username = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            //string accountType = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod).Value;
            //return _context.Users.FirstOrDefault(u => u.Username == username && u.AccountType.ToString() == accountType);

            return context.Users.FirstOrDefault(u => u.Username == username);
        }



        //IMPLEMENTARE CRUD PENTRU USER

        public IEnumerable<UserGetModel> GetAll()
        {
            return context.Users.Select(user => UserGetModel.FromUser(user));
        }

        public UserGetModel GetById(int id)
        {
            User user = context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == id);

            return UserGetModel.FromUser(user);
        }

        public UserGetModel Create(UserPostModel userModel)
        {
            User toAdd = UserPostModel.ToUser(userModel);

            context.Users.Add(toAdd);
            context.SaveChanges();
            return UserGetModel.FromUser(toAdd);

        }

        //public UserGetModel Upsert(int id, UserPostModel userPostModel)
        //{
        //    var existing = context
        //        .Users
        //        .AsNoTracking()
        //        .FirstOrDefault(u => u.Id == id);
        //    if (existing == null)
        //    {

        //       userPostModel.Password = ComputeSha256Hash(userPostModel.Password);
        //        User toAdd = UserPostModel.ToUser(userPostModel);
        //        context.Users.Add(toAdd);
        //        context.SaveChanges();
        //        return UserGetModel.FromUser(toAdd);
        //    }

        //   userPostModel.Password = ComputeSha256Hash(userPostModel.Password);

        //    User toUpdate = UserPostModel.ToUser(userPostModel);
        //    toUpdate.Id = id;
        //    context.Users.Update(toUpdate);
        //    context.SaveChanges();
        //    return UserGetModel.FromUser(toUpdate);
        //}

        public UserGetModel Upsert(int id, UserPostModel userPostModel)
        {
            var existing = context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (existing == null)
            {
                //User UserAdd = User.ToUser(user);
                userPostModel.Password = ComputeSha256Hash(userPostModel.Password);
                User toAdd = UserPostModel.ToUser(userPostModel);
                context.Users.Add(toAdd);
                context.SaveChanges();
                return UserGetModel.FromUser(toAdd);
            }

            //User UserUp = User.ToUser(user);


            userPostModel.Password = ComputeSha256Hash(userPostModel.Password);
            User toUpdate = UserPostModel.ToUser(userPostModel);
            toUpdate.Id = id;
            context.Users.Update(toUpdate);
            context.SaveChanges();
            return UserGetModel.FromUser(toUpdate);
        }


        public UserGetModel Delete(int id)
        {
            var existing = context.Users.Include(x => x.Tasks).FirstOrDefault(u => u.Id == id);
            if (existing == null)
            {
                return null;
            }

            context.Users.Remove(existing);
            context.SaveChanges();

            return UserGetModel.FromUser(existing);
        }




    }
}

