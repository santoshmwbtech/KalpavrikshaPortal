using JBNClassLibrary;
using JBNWebAPI.Logger;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace JBNAdminPortal.Controllers
{
    public class LoginController : Controller
    {
        //LoginValidation Dal = new LoginValidation();
        private readonly IUserRepository _userRepository;
        public LoginController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Login login = await _userRepository.UserLogin(model);
            if (login == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            else
            {
                Session["UserID"] = login.UserID;
                Session["FullName"] = login.FullName;
                Session["RoleID"] = login.RoleID;
                DLFormPermission dL = new DLFormPermission();
                List<FormPermissionItem> MenuList = dL.LoadFormPermissionItems(login.RoleID);
                Session["MenuMaster"] = MenuList;
                return RedirectToAction("Index", "Dashboard");
            }
        }
    }
}