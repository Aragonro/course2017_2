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
using EmulCurs.Models;
using EmulCurs.Providers;
using EmulCurs.Results;
using System.Collections.ObjectModel;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;


namespace EmulCurs.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
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
        private EmulCursContext db = new EmulCursContext();

        // GET: api/Users/email
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("User")]
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(string email)
        {
            string e = User.Identity.GetUserName();
            if (e != email)
            {
                return BadRequest();
            }
            User user;

            IEnumerator<User> userIE = db.Users.SqlQuery("SELECT * FROM dbo.Users WHERE @p0=email", email).GetEnumerator();
            userIE.MoveNext();
            user = userIE.Current;

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // GET: api/EmulationKit/email
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("EmulationKit")]
        public IHttpActionResult GetEmulationKit(string email)
        {
            string e = User.Identity.GetUserName();
            if (e != email)
            {
                return BadRequest(); 
            }
            Collection<EmulationKit> emulkitCollect = new Collection<EmulationKit>();
            IEnumerator<EmulationKit> emulkitIE = db.EmulationKits.SqlQuery("SELECT * FROM dbo.EmulationKits WHERE UserId = (Select UserId FROM dbo.Users WHERE @p0=Email)", email).GetEnumerator();
            while (emulkitIE.MoveNext())
            {
                emulkitCollect.Add(emulkitIE.Current);
            }
            return Ok(emulkitCollect);
        }

        // GET: api/EmulationKits/5
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("EmulationKit")]
        [ResponseType(typeof(EmulationKit))]
        public IHttpActionResult GetEmulationKit(int id)
        {
            EmulationKit emulationKit = db.EmulationKits.Find(id);
            if (emulationKit == null)
            {
                return NotFound();
            }

            string email = User.Identity.GetUserName();
            IEnumerator<EmulationKit> emulkitIE = db.EmulationKits.SqlQuery("SELECT * FROM dbo.EmulationKits WHERE EmulationKitid=@p0 AND UserId = (Select UserId FROM dbo.Users WHERE @p1=Email)", id, email).GetEnumerator();
            if (!emulkitIE.MoveNext())
            {
                return BadRequest();
            }


            return Ok(emulationKit);
        }
        private bool EmulationKitExists(int id)
        {
            return db.EmulationKits.Count(e => e.EmulationKitId == id) > 0;
        }
        // PUT: api/EmulationKits/5
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("EmulationKit")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmulationKit(int id, EmulationKit emulationKit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string email= User.Identity.GetUserName();
            IEnumerator<EmulationKit> emulkitIE = db.EmulationKits.SqlQuery("SELECT * FROM dbo.EmulationKits WHERE EmulationKitid=@p0 AND UserId = (Select UserId FROM dbo.Users WHERE @p1=Email)", id,email).GetEnumerator();
            if (!emulkitIE.MoveNext())
            {
                return BadRequest();
            }
                if (id != emulationKit.EmulationKitId)
            {
                return BadRequest();
            }

            db.Entry(emulationKit).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmulationKitExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/EmulationKits
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("EmulationKit")]
        [ResponseType(typeof(EmulationKit))]
        public IHttpActionResult PostEmulationKit(EmulationKit emulationKit)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string email= User.Identity.GetUserName();
            IEnumerator<User> userIE = db.Users.SqlQuery("SELECT * FROM dbo.Users WHERE UserId=@p0 AND Email = @p1)", emulationKit.UserId, email).GetEnumerator();
            if (!userIE.MoveNext())
            {
                return BadRequest();
            }
            db.EmulationKits.Add(emulationKit);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = emulationKit.EmulationKitId }, emulationKit);
        }

        // DELETE: api/EmulationKits/5
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("EmulationKit")]
        [ResponseType(typeof(EmulationKit))]
        public IHttpActionResult DeleteEmulationKit(int id)
        {
            EmulationKit emulationKit = db.EmulationKits.Find(id);
            if (emulationKit == null)
            {
                return NotFound();
            }

            string email = User.Identity.GetUserName();
            IEnumerator<User> userIE = db.Users.SqlQuery("SELECT * FROM dbo.Users WHERE UserId=@p0 AND Email = @p1", emulationKit.UserId, email).GetEnumerator();
            if (!userIE.MoveNext())
            {
                return BadRequest();
            }

            db.EmulationKits.Remove(emulationKit);
            db.SaveChanges();

            return Ok(emulationKit);
        }

        // GET: api/Videos/5
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Video")]
        [ResponseType(typeof(Video))]
        public IHttpActionResult GetVideo(int id, string email)
        {
            if (email != User.Identity.GetUserName())
            {
                return BadRequest();
            }
            Video video = db.Videos.Find(id);
            if (video == null)
            {
                return NotFound();
            }

            return Ok(video);
        }

        // POST: api/Videos
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Video")]
        [ResponseType(typeof(Video))]
        public IHttpActionResult PostVideo(string email, Video video)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (email != User.Identity.GetUserName())
            {
                return BadRequest();
            }

            db.Videos.Add(video);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = video.VideoId }, video);
        }

        private bool VideoExists(int id)
        {
            return db.Videos.Count(e => e.VideoId == id) > 0;
        }

        // PUT: api/Videos/5
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Video")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVideo(int id, Video video)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != video.VideoId)
            {
                return BadRequest();
            }

            string email = User.Identity.GetUserName();

            IEnumerator<User> userIE = db.Users.SqlQuery("SELECT * FROM dbo.Users WHERE Email = @p1 AND UserId=(Select UserId From dbo.EmulationKits Where VideoId=@p0)", id, email).GetEnumerator();
            if (!userIE.MoveNext())
            {
                return BadRequest();
            }

            db.Entry(video).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Videos/5
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Video")]
        [ResponseType(typeof(Video))]
        public IHttpActionResult DeleteVideo(int id)
        {
            Video video = db.Videos.Find(id);
            if (video == null)
            {
                return NotFound();
            }
            IEnumerator<EmulationKit> emulkitIE = db.EmulationKits.SqlQuery("SELECT * FROM dbo.EmulationKits WHERE VideoId=@p0", id).GetEnumerator();
            if (emulkitIE.MoveNext())
            {
                return BadRequest();
            }
            db.Videos.Remove(video);
            db.SaveChanges();

            return Ok(video);
        }

        // GET: api/EmulationKitUpdate/idemul
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("EmulationKitUpdate")]
        [ResponseType(typeof(Collection<EmulationKitUpdate>))]
        public Collection<EmulationKitUpdate> GetEmulationKitUpdate(int idemul)
        {
            string email = User.Identity.GetUserName();
            IEnumerator<EmulationKit> emulkitIE = db.EmulationKits.SqlQuery("SELECT * FROM dbo.EmulationKits WHERE EmulationKitId=@p0 AND UserID=(Select UserId FROM dbo.Users Where Email=@p1)", idemul, email).GetEnumerator();
            if (!emulkitIE.MoveNext())
            {
                return null;
            }

            Collection<EmulationKitUpdate> emulkitupCollect = new Collection<EmulationKitUpdate>();
            IEnumerator<EmulationKitUpdate> emulkitupIE = db.EmulationKitUpdates.SqlQuery("SELECT * FROM dbo.EmulationKitUpdates WHERE EmulationKitId = @p0", idemul).GetEnumerator();
            while (emulkitupIE.MoveNext())
            {
                emulkitupCollect.Add(emulkitupIE.Current);
            }
            return emulkitupCollect;
        }

        // DELETE: api/EmulationKitUpdates/5
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("EmulationKitUpdate")]
        [ResponseType(typeof(EmulationKitUpdate))]
        public IHttpActionResult DeleteEmulationKitUpdate(int id)
        {
            EmulationKitUpdate emulationKitUpdate = db.EmulationKitUpdates.Find(id);
            if (emulationKitUpdate == null)
            {
                return NotFound();
            }

            string email = User.Identity.GetUserName();
            IEnumerator<EmulationKit> emulkitIE = db.EmulationKits.SqlQuery("SELECT * FROM dbo.EmulationKits WHERE EmulationKitId=@p0 AND UserID=(Select UserId FROM dbo.Users Where Email=@p1)", emulationKitUpdate.EmulationKitId,email).GetEnumerator();
            if (!emulkitIE.MoveNext())
            {
                return BadRequest();
            }
            EmulationKitUpdatesController em = new EmulationKitUpdatesController();
            return Ok(em.DeleteEmulationKitUpdate(id));
            //db.EmulationKitUpdates.Remove(emulationKitUpdate);
            //db.SaveChanges();

            //return Ok(emulationKitUpdate);
        }

        // DELETE: api/EmulationKitUpdates/5
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("EmulationKitUpdate")]
        [ResponseType(typeof(EmulationKitUpdate))]
        public IHttpActionResult DeleteEmulationKitUpdate(int idemul, string email)
        {
            string e = User.Identity.GetUserName();
            IEnumerator<EmulationKit> emulkitIE = db.EmulationKits.SqlQuery("SELECT * FROM dbo.EmulationKits WHERE EmulationKitId=@p0 AND UserID=(Select UserId FROM dbo.Users Where Email=@p1)", idemul, e).GetEnumerator();
            if (!emulkitIE.MoveNext())
            {
                return BadRequest();
            }
            IEnumerator<EmulationKitUpdate> emulkitupdateIE = db.EmulationKitUpdates.SqlQuery("SELECT * FROM dbo.EmulationKitUpdates WHERE EmulationKitId=@p0", idemul).GetEnumerator();
            EmulationKitUpdate emulationKitUpdate=null;
            while (emulkitupdateIE.MoveNext())
            {
                emulationKitUpdate = db.EmulationKitUpdates.Find(emulkitupdateIE.Current.EmulationKitUpdateId);
                if (emulationKitUpdate == null)
                {
                    return NotFound();
                }
                db.EmulationKitUpdates.Remove(emulationKitUpdate);
                db.SaveChanges();
            }
            if (emulationKitUpdate == null)
            {
                return NotFound();
            }

            return Ok(emulationKitUpdate);
        }
        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
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

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                db.Dispose();
                _userManager.Dispose();
                _userManager = null;
            }
            if (disposing)
            {
                db.Dispose();
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
