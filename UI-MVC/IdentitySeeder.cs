using Microsoft.AspNetCore.Identity;

namespace EM.UI.MVC
{
    public class IdentitySeeder
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentitySeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            // Zorg ervoor dat de rollen bestaan
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Maak vijf gebruikers aan, zoals gevraagd in de opdracht
            await CreateUserWithRoleAsync("user1@example.com", "Password123!", "Admin");
            await CreateUserWithRoleAsync("user2@example.com", "Password123!", "User");
            await CreateUserWithRoleAsync("user3@example.com", "Password123!", "User");
            await CreateUserWithRoleAsync("user4@example.com", "Password123!", "User");
            await CreateUserWithRoleAsync("user5@example.com", "Password123!", "Admin");
        }

        private async Task CreateUserWithRoleAsync(string email, string password, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    // Log eventuele fouten bij het aanmaken van de gebruiker
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating user {email}: {error.Description}");
                    }
                }
            }
            else
            {
                // Reset het wachtwoord om ervoor te zorgen dat het overeenkomt met 'password'
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, password);
                if (!resetResult.Succeeded)
                {
                    foreach (var error in resetResult.Errors)
                    {
                        Console.WriteLine($"Error resetting password for {email}: {error.Description}");
                    }
                }

                // Verwijder bestaande rollen om dubbele rollen te voorkomen
                var existingRoles = await _userManager.GetRolesAsync(user);
                if (existingRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, existingRoles);
                }
                // Voeg de gewenste rol toe
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}