using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;

namespace CapstoneProject.Pages
{
    public class AccountModel : PageModel
    {
        private readonly UserAccessLayer _userAccess;
        private readonly CharacterAccessLayer _characterAccess;

        public AccountModel(UserAccessLayer userAccess, CharacterAccessLayer characterAccess)
        {
            _userAccess = userAccess;
            _characterAccess = characterAccess;
        }
        public CharacterModel CurrentCharacter { get; set; }

        public UserModel User { get; set; }

        public IActionResult OnGet(int id)
        {
            IActionResult temp = Page();
            User = _userAccess.GetUserByID(id);   

            if (User == null)
            {
                temp = RedirectToPage("/Index");
            }

            CurrentCharacter = _characterAccess.GetCharacterBySlot(id.ToString(), 1);

            return temp; 
        }
    }
}
