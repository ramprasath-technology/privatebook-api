using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrivateBookAPI.Data;
using PrivateBookAPI.Data.DTO;

namespace PrivateBookAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly PrivateBookContext _context;

        public UsersController(PrivateBookContext context)
        {
            _context = context;
        }

        // GET: api/Users
        // Get all users
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return _context.Users;
        }

        // GET: api/Users/5
        // Get user by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        // Update user
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // Create a new user
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                List<User> checkIfExists = _context.Users.Where(x => x.Email == user.Email).ToList();

                if (checkIfExists.Count != 0)
                {
                    return this.Ok("Already exists");
                }

                MD5 md5Hash = MD5.Create();
                string hash = GetMd5Hash(md5Hash, user.Password);
                user.Password = hash;
                
                 _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUser", new { id = user.UserId }, user);
            }
            catch(Exception ex)
            {
                string message = ex.Message;
                throw ex;
            }
            
        }

        // Get user by email
        [HttpGet("EmailVerification/{email}", Name = "GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await this._context.Users.FirstOrDefaultAsync(x => x.Email == email);

            return this.Ok(user);
        }

        // Get security question
        [HttpGet("SecurityQuestion/{id}", Name = "GetSecurityQuestion")]
        public async Task<IActionResult> GetSecurityQuestion([FromRoute] int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await this._context.Users.FirstOrDefaultAsync(x => x.UserId == id);

            return this.Ok(user.SecurityQuestion);
        }

        // Reset password
        [HttpPost("PasswordReset", Name = "ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordReset password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await this._context.Users.FirstOrDefaultAsync(x => x.UserId == password.UserId);
            if(password.SecurityAnswer.Trim().Equals(user.SecurityAnswer))
            {
                MD5 md5Hash = MD5.Create();
                string hash = GetMd5Hash(md5Hash, password.NewPassword);
                user.Password = hash;

                this._context.Update(user);
                await _context.SaveChangesAsync();

                return this.Ok();
            }
            return this.BadRequest();
        }

        // DELETE: api/Users/5
        // Delete a user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // Check if user exists
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        //Log in user
        [HttpPost("Login", Name = "LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] Login login)
        {
         
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == login.Email);
            if(user == null)
            {
                return NotFound();
            }

            string savedPasswordHash = user.Password;
            MD5 md5Hash = MD5.Create();
            bool authentic = VerifyMd5Hash(md5Hash, login.Password, savedPasswordHash);

            if (authentic)
                return this.Ok(user.UserId);
            else
                return this.Ok(-1);
        }

        //Has using MD5 algorithm
        private string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        private bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}