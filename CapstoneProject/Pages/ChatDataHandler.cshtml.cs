using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class ChatDataHandlerModel : PageModel
    {
        private readonly UserAccessLayer _userDAL;
        private readonly CommunityDataAccessLayer _communityDAL;

        public ChatDataHandlerModel(UserAccessLayer userDAL, IConfiguration configuration)
        {
            _userDAL = userDAL;
            _communityDAL = new CommunityDataAccessLayer(configuration);
        }

        // GET /ChatDataHandler?handler=Friends
        public IActionResult OnGetFriends()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return new JsonResult(new List<object>());

            int? userID = _userDAL.GetUserID(username);
            if (!userID.HasValue)
                return new JsonResult(new List<object>());

            var friends = _userDAL.GetFriends(userID.Value)
                .Select(f => new { id = f.User_ID, name = f.Username });

            return new JsonResult(friends);
        }

        // GET /ChatDataHandler?handler=Communities
        public IActionResult OnGetCommunities()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return new JsonResult(new List<object>());

            int? userID = _userDAL.GetUserID(username);
            if (!userID.HasValue)
                return new JsonResult(new List<object>());

            var communities = _communityDAL.GetUserCommunities(userID.Value).ToList();

            return new JsonResult(communities.Select(c => new { id = c.CommunityID, name = c.Name }));
        }
        public IActionResult OnGetMessages(string roomId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return new JsonResult(new List<object>());

            int? currentUserId = _userDAL.GetUserID(username);
            if (!currentUserId.HasValue)
                return new JsonResult(new List<object>());

            // roomId is "smallerID_largerID" e.g. "3_7"
            var parts = roomId.Split('_');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int id1) || !int.TryParse(parts[1], out int id2))
                return new JsonResult(new List<object>());

            var messages = _userDAL.GetMessages(id1, id2);
            return new JsonResult(messages.Select(m => new
            {
                sender = m.SenderUsername,
                text = m.Content,
                sentAt = m.Timestamp
            }));
        }
    }
}
