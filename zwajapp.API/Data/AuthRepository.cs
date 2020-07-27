using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using zwajapp.API.Models;

namespace zwajapp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            this._context = context;
        }
        async Task<User> IAuthRepository.Login(string username, string password)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null )
            {
                return null;

            }

            if (!VerifyPasswordHash(password, user.PasswordSalt, user.PasswordHash)) {

                return null; 
            }

            return user;


        }

        private bool VerifyPasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using (var Hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedhash = Hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedhash.Length; i++)
                {
                    if (computedhash[i] != passwordHash[i])
                        return false;

                }
                return true;

            }
        }

        async Task<User> IAuthRepository.Register(User user, string password)
        {
            byte[] PassordSalt, PasswordHash;
            createpasswordhash(password ,out PasswordHash, out PassordSalt);
            user.PasswordHash = PasswordHash;
            user.PasswordSalt = PassordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user; 
        }

        private void createpasswordhash(string password, out byte[] passwordHash, out byte[] passordSalt)
        {
           
                using (var Hmac = new System.Security.Cryptography.HMACSHA512()) {
                passordSalt = Hmac.Key;
                passwordHash = Hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            
            }
               
        }

       async  Task<bool> IAuthRepository.UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u =>u.Username == username))
            {

                return true;
            }
            return false;
        }

     
    }
}