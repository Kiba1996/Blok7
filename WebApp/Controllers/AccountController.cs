using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebApp.Models;
using WebApp.Providers;
using WebApp.Results;
using WebApp.Persistence.UnitOfWork;
using System.Linq;
using WebApp.Persistence;
using System.Data.Entity;
using System.Net;
using System.IO;
using System.Drawing;
using System.Text;
using System.Net.Mail;
using System.Data.Entity.Infrastructure;
using WebApp.Models.HelpModels;

namespace WebApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        public static string path = "";
        //private readonly DbContext dbContext;
        private readonly IUnitOfWork unitOfWork;


        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat, IUnitOfWork uw)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
            unitOfWork = uw;

        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

      
        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            var user = new ApplicationUser();
            user.UserName = model.Email;
            user.Email = model.Email;
            user.Activated = model.Activated;
            user.Birthday = DateTime.Parse(model.Birthday);
            user.Address = model.Address;
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Role = model.Role;
            user.Image = path;
            if (path != "")
            {
                user.Activated = "Pending";
            }
            path = "";
            if (user.Role == "AppUser")
            {
                if (model.PassengerType == "Regular")
                {
                    user.Activated = "Activated";//////////////////////////////////////////
                }
                var p = unitOfWork.PassengerTypes.GetAll();
                foreach (var k in p)
                {
                    if (k.Name == model.PassengerType)
                    {
                        user.PassengerTypeId = k.Id;
                        break;
                    }
                }
            }
            
            IdentityResult result = await UserManager.CreateAsync(user, model.Password);
           
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            UserManager.AddToRole(user.Id, user.Role);
            return Ok("Register succeeded");
        }

        [AllowAnonymous]
        [Route("Edit")]
        public async Task<IHttpActionResult> Edit(EditBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = UserManager.FindById(model.Id);
            user.UserName = model.Email;
            user.Email = model.Email;
            user.Birthday = DateTime.Parse(model.Birthday);
            user.Address = model.Address;
            user.Name = model.Name;
            user.Surname = model.Surname;
            if(user.Image == "")
            {
                if (path != "")
                {
                    user.Activated = "Pending";
                }
                user.Image = path;
            }
            path = "";
            
            IdentityResult result = await UserManager.UpdateAsync(user);
            //UserManager.AddToRole(user.Id, user.Role);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }



        [Route("GetUser")]
        public ApplicationUser GetUser(string email)
        {
            ApplicationUser user = UserManager.FindByEmail(email);
            
           
          //  ApplicationUser user = unitOfWork.ApplicationUsers.Find(u => u.Email == email).FirstOrDefault(); /*unitOfWork.ApplicationUsers.Find(u => u.Email == email).FirstOrDefault();*/
            return user;
        }
        [Route("GetPassengerTypeForUser")]
        public PassengerType GetPassengerTypeForUser(string email)
        {
            ApplicationUser user = UserManager.FindByEmail(email);
            PassengerType pas = unitOfWork.PassengerTypes.Get((int)user.PassengerTypeId);
            return pas;

        }


        [AllowAnonymous]
        [Route("PostImage")]
        public async Task<HttpResponseMessage> PostImage()
        {
          
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png", ".img", ".jpeg" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png,.img,.jpeg.");

                            return Request.CreateResponse(HttpStatusCode.BadRequest, message);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {
                            var message = string.Format("Please Upload a file upto 1 mb.");

                            return Request.CreateResponse(HttpStatusCode.BadRequest, message);
                        }
                        else
                        {
                            if (!Directory.Exists(HttpContext.Current.Server.MapPath("/Content/Images")))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("/Content/Images"));

                            var filePath = HttpContext.Current.Server.MapPath("/Content/Images/" + postedFile.FileName);
                            //postedFile as .SaveAs(filePath);

                            Bitmap bmp = new Bitmap(postedFile.InputStream);
                            Image img = (Image)bmp;
                            byte[] imagebytes = ImageToByteArray(img);
                            byte[] cryptedBytes = EncryptBytes(imagebytes, "password", "asdasd");
                            string newFN = "";
                            
                            path = "/Content/Images/" + postedFile.FileName;
                            var message = string.Format("/Content/Images/" + postedFile.FileName);
                            if (File.Exists(filePath)){
                                newFN = postedFile.FileName.Split('.')[0] + "1." + postedFile.FileName.Split('.')[1];
                                filePath = HttpContext.Current.Server.MapPath("/Content/Images/" + newFN);
                                path = "/Content/Images/" + newFN;
                                message = string.Format("/Content/Images/" + newFN);
                            }

                            File.WriteAllBytes(filePath, cryptedBytes);

                        }
                    }

                    var message1 = string.Format("Image Updated Successfully.");
                    //return Request.CreateErrorResponse(HttpStatusCode.Created, message1);
                }

                var res = string.Format("Please Upload a image.");
                //return Request.CreateResponse(HttpStatusCode.NotFound, res);
            }
            catch (Exception)
            {
                var res = string.Format("some Message");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }

          
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public static byte[] EncryptBytes(byte[] inputBytes, string passPhrase, string saltValue)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            RijndaelCipher.Mode = CipherMode.CBC;
            byte[] salt = Encoding.ASCII.GetBytes(saltValue);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, salt, "SHA1", 2);

            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(password.GetBytes(32), password.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] CipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return CipherBytes;
        }

        [AllowAnonymous]
        [Route("GetUserImages")]
        public async Task<List<byte[]>> PostUserImages(List<ApplicationUser> list)
        {
            List<byte[]> returnList = new List<byte[]>();
            foreach (var uid in list)
            {
                var filePath = HttpContext.Current.Server.MapPath(uid.Image /*+ postedFile.FileName.Split('.').LastOrDefault()*/);

                if (File.Exists(filePath))
                {
                    byte[] bytes = File.ReadAllBytes(filePath);
                    byte[] decryptedBytes = DecryptBytes(bytes, "password", "asdasd");
                    returnList.Add(decryptedBytes);
                }
            }

            return returnList;
        }

        public static byte[] DecryptBytes(byte[] encryptedBytes, string passPhrase, string saltValue)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            RijndaelCipher.Mode = CipherMode.CBC;
            byte[] salt = Encoding.ASCII.GetBytes(saltValue);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, salt, "SHA1", 2);

            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(password.GetBytes(32), password.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream(encryptedBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
            byte[] plainBytes = new byte[encryptedBytes.Length];

            int DecryptedCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

            memoryStream.Close();
            cryptoStream.Close();

            return plainBytes;
        }

        [Authorize(Roles = "Admin")]
        [Route("GetAwaitingAdmins")]
        public List<ApplicationUser> GetAwaitingAdmins()
        {
            List<ApplicationUser> u = UserManager.Users.Where(x => ((x.Activated == "Deactivated" || x.Activated == "Pending") && x.Role == "Admin")).ToList();
            return u;
        }

        [Authorize(Roles = "Admin")]
        [Route("GetAwaitingAControllers")]
        public List<ApplicationUser> GetAwaitingControllers()
        {
            List<ApplicationUser> u =  UserManager.Users.Where(x => ((x.Activated == "Deactivated" || x.Activated == "Pending") && x.Role == "Controller")).ToList();
            return u;
        }

        [Authorize(Roles = ("Controller"))]
        [Route("GetAwaitingClients")]
        public List<ApplicationUser> GetAwaitingClients()
        {
            List<ApplicationUser> u = UserManager.Users.Where(x => (x.Activated == "Pending" && x.Role == "AppUser" && x.Image != "")).ToList();
            return u;
        }

        [Authorize(Roles = "Admin")]
        [Route("AuthorizeAdmin")]
        public string AuthorizeAdmin([FromBody]ModelHelperAuthorization Id)
        {
           
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState).ToString();
                }
                //Get user data, and update activated to true
                ApplicationUser current = UserManager.FindById(Id.Id);
                current.Activated = "Activated";

                try
                {
                IdentityResult result =  UserManager.Update(current);

                try
                    {
                        string subject = "Admin approved";
                        string desc = $"Dear {current.Name}, You have been approved as admin.";
                        var adminEmail = current.Email;
                       NotifyViaEmail(adminEmail, subject, desc);
                    }
                    catch { }
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest().ToString();
                }

                return "Ok";
            
        }

        [Authorize(Roles = "Admin")]
        [Route("AuthorizeControll")]
        public string AuthorizeControll([FromBody]ModelHelperAuthorization Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState).ToString();
            }
            //Get user data, and update activated to true
            ApplicationUser current = UserManager.FindById(Id.Id);
            current.Activated = "Activated";

            try
            {
                IdentityResult result =  UserManager.Update(current);

                try
                {
                    string subject = "Controller approved";
                    string desc = $"Dear {current.Name}, You have been approved as controller.";
                    var controllerEmail = current.Email;
                    NotifyViaEmail(controllerEmail, subject, desc);
                }
                catch { }
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest().ToString();
            }

            return "Ok";

        }
        
        [Authorize(Roles = "Controller")]
        [Route("AuthorizeUser")]
        public string AuthorizeUser([FromBody]ModelHelperAuthorization Id)
        {
            
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState).ToString();
                }
            //Get user data, and update activated to true
            ApplicationUser current = UserManager.FindById(Id.Id);
            current.Activated = "Activated";

            try
                {
                IdentityResult result = UserManager.Update(current);
                try
                {
                   
                    
                    
                    string subject = "Account approval";
                    string desc = $"Dear {current.Name}, Your account has been approved.";
                    var controllerEmail = current.Email;
                    NotifyViaEmail(controllerEmail, subject, desc);
                }
                catch { }
            }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest().ToString();
                }

                return "Ok";
            
        }

        [Authorize(Roles = "Admin")]
        [Route("DeclineAdmin")]
        public string DeclineAdmin([FromBody]ModelHelperAuthorization Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState).ToString();
            }
            //Get user data, and update activated to true
            ApplicationUser current = UserManager.FindById(Id.Id);
            current.Activated = "Declined";

            try
            {
                IdentityResult result = UserManager.Update(current);

                try
                {
                    string subject = "Admin declined";
                    string desc = $"Dear {current.Name}, You have been declined as admin. You can resend request on you profile!";
                    var adminEmail = current.Email;
                    NotifyViaEmail(adminEmail, subject, desc);
                }
                catch { }
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest().ToString();
            }
            return "Ok";

        }

        [Authorize(Roles = "Admin")]
        [Route("DeclineController")]
        public string DeclineController([FromBody]ModelHelperAuthorization Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState).ToString();
            }
            //Get user data, and update activated to true
            ApplicationUser current = UserManager.FindById(Id.Id);
            current.Activated = "Declined";

            try
            {
                IdentityResult result = UserManager.Update(current);

                try
                {
                    string subject = "Controller declined";
                    string desc = $"Dear {current.Name}, You have been declined as controller. You can resend request on you profile!";
                    var adminEmail = current.Email;
                    NotifyViaEmail(adminEmail, subject, desc);
                }
                catch { }
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest().ToString();
            }

            return "Ok";

        }

        [Authorize(Roles = "Controller")]
        [Route("DeclineUser")]
        public string DeclineUser([FromBody]ModelHelperAuthorization Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState).ToString();
            }
            //Get user data, and update activated to true
            ApplicationUser current = UserManager.FindById(Id.Id);
            current.Activated = "Declined";
            current.Image = "";

            try
            {
                IdentityResult result = UserManager.Update(current);

                try
                {
                    string subject = "User declined";
                    string desc = $"Dear {current.Name}, You have been declined as authorized user. You can resend request by uploading picture of document!";
                    var adminEmail = current.Email;
                    NotifyViaEmail(adminEmail, subject, desc);
                }
                catch { }
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest().ToString();
            }

            return "Ok";

        }


        public bool NotifyViaEmail(string targetEmail, string subject, string body)
        {
            string mailTo = targetEmail;
            string mailFrom = "pusgs2019app@gmail.com";
            string pass = "12345Aa.";

            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 10000;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(mailFrom, pass);

                MailMessage mm = new MailMessage(mailFrom, mailTo);
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.Subject = subject;
                mm.Body = body;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(mm);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error, mail did not send");
                return false;
            }
        }

        [AllowAnonymous]
        [Route("ResendRequest")]
        public string ResendRequest([FromBody]ModelHelperAuthorization Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState).ToString();
            }
            

            ApplicationUser user = UserManager.FindByEmail(Id.Id);
            user.Activated = "Pending";
            try
            {
                IdentityResult result = UserManager.Update(user);


            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest().ToString();
            }

            return "Ok";
        }


        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
