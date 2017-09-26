using System;
using System.Collections.Generic;
//using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SaheAzWebApiAlakbarAlakbarov.Models;
using SaheAzWebApiAlakbarAlakbarov.Utilities;
using System.Web.Security;
using System.IO;
using System.Drawing;
using System.Web.Hosting;
//using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
//using Microsoft.Owin.Security;
//using Microsoft.Owin;
using System.Xml.Linq;
using System.Data;

namespace SaheAzWebApiAlakbarAlakbarov.Controllers
{
    public class UsersController : ApiController
    {
        private MyDbEntities db = new MyDbEntities();




        Methods methods = new Methods();
        // GET: api/Users
        public IQueryable<User> GetUser()
        {
            return db.User;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.User.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.User.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = user.id }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.User.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.User.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.User.Count(e => e.id == id) > 0;
        }


        [HttpPost]
        [Route("api/user/register")]
        public IHttpActionResult Register([FromBody]User user)
        {

            User usr = new User();

            usr.username = user.username;
            usr.password = methods.EncryptPassword(user.password);
            usr.first_name = user.first_name;
            usr.last_name = user.last_name;
            usr.email = user.email;
            usr.mobile = user.mobile;
            usr.register_date = user.register_date;
            // usr.photo = PostUserImage();


            if (methods.UsernameExistsRegister(user.username)
                || methods.EmailExistsRegister(user.email)
                || methods.MobileExistsRegister(user.mobile)
                || string.IsNullOrWhiteSpace(user.username)
                || string.IsNullOrWhiteSpace(user.email)
                || string.IsNullOrWhiteSpace(user.mobile))
            {
                return BadRequest();
            }
            else
            {

                db.User.Add(usr);
                db.SaveChanges();
                return Ok(true);
            }

        }


        [Route("api/user/userIsAuthenticated")]
        public IHttpActionResult UserIsAuthenticated()
        {
            return Ok(User.Identity.IsAuthenticated);
        }


        [HttpPost]
        [Route("api/user/session")]
        public IHttpActionResult Session(User user)
        {
            var curSess = SessionStateUtility.GetHttpSessionStateFromContext(HttpContext.Current);
            User loggedUser = new Models.User();
            User temporaryHolder = new Models.User();
            string usernameToId;

            if (curSess == null)
            {
                user = UserInfos.UserInfo();
                usernameToId = db.User.FirstOrDefault(x => x.username.Equals(user.username)).id.ToString();
                temporaryHolder.id = Convert.ToInt32(usernameToId);
                loggedUser = db.User.Find(temporaryHolder.id);


                curSess.Add("User", loggedUser);
                curSess.Timeout = 18000;
            }
            return Ok(curSess["User"]);
        }





        //[HttpPost]
        //[Route("api/user/salam")]
        //public IHttpActionResult salam(User user)
        //{
        //    User alakbar = new Models.User();
        //    User loggedUser = new Models.User();
        //    User temporaryHolder = new Models.User();
        //    string usernameToId;
        //    var curSess = HttpContext.Current.Session;
        //    if (methods.UserExistWithUserNamePassword(user.username, user.password) ||
        //            methods.UserExistWithEmailPassword(user.email, user.password))
        //    {
        //        //FormsAuthentication.SetAuthCookie(
        //        //        db.User.FirstOrDefault(x => x.username.Equals(user.username)).id.ToString(), true);
        //        //usernameToId = db.User.FirstOrDefault(x => x.username.Equals(user.username)).id.ToString();
        //        //temporaryHolder.id = Convert.ToInt32(usernameToId);
        //        //loggedUser = db.User.Find(temporaryHolder.id);
        //        alakbar = UserInfos.UserInfo;
        //        curSess.Add("User", alakbar);
        //        //session.Add("User", loggedUser);

        //        return Ok(curSess["User"]);
        //    }

        //    return Ok(alakbar);
        //}



        [HttpPost]
        [Route("api/user/login")]
        public IHttpActionResult Login([FromBody]User user)
        {

            User loggedUser = new Models.User();
            User temporaryHolder = new Models.User();
            string usernameToId;
            var curSess = HttpContext.Current.Session;
            //var session = SessionStateUtility.GetHttpSessionStateFromContext(HttpContext.Current);

            user.password = methods.EncryptPassword(user.password);
                  if (methods.UserExistWithUserNamePassword(user.username, user.password) ||
                    methods.UserExistWithEmailPassword(user.email, user.password))
                {
                    FormsAuthentication.SetAuthCookie(
                            db.User.FirstOrDefault(x => x.username.Equals(user.username)).id.ToString(), true);
                    usernameToId = db.User.FirstOrDefault(x => x.username.Equals(user.username)).id.ToString();
                    temporaryHolder.id = Convert.ToInt32(usernameToId);
                    loggedUser = db.User.Find(temporaryHolder.id);
                    
                    curSess.Add("User", loggedUser);
                    curSess.Timeout = 18000;
                    //session.Add("User", loggedUser);

                    return Ok(curSess["User"]);
                }
            return BadRequest();
        }
        
    
        [HttpPost]
        [Route("api/user/logout")]
        public IHttpActionResult Logout()
        {
            HttpContext.Current.Session.RemoveAll();
            string sessionMessage = "Butun sessiya melumatlari silinmishdir. LogOut";
            return Ok(sessionMessage);
        }


        //[HttpPut]
        //[Route("api/user/profileUpdate")]
        //public IHttpActionResult ProfileUpdate([FromBody]User user)
        //{


        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // if(user.username!=null)
        //    // {

        //    if (methods.UsernameExists(user.username, user.id)
        //    || string.IsNullOrWhiteSpace(user.username))
        //    {

        //        string errorResponse = "Daxil etdiyiniz istifadeci adi artiq movcuddur";
        //        return BadRequest(errorResponse);
        //    }

        //    // }
        //    //if(user.mobile!=null)
        //    //{

        //    if (methods.MobileExists(user.mobile, user.id)
        //    || string.IsNullOrWhiteSpace(user.mobile))
        //    {
        //        string errorResponse = "Daxil etdiyiniz mobil nomre artiq movcuddur";
        //        return BadRequest(errorResponse);
        //    }

        //    // }
        //    // if(user.email!=null)


        //    if (methods.EmailExists(user.email, user.id)
        //    || string.IsNullOrWhiteSpace(user.email))
        //    {
        //        string errorResponse = "Daxil etdiyiniz email artiq movcuddur";
        //        return BadRequest(errorResponse);
        //    }

        //    // }
        //    User dbUser = new Models.User();

        //    if (!UserExists(user.id))
        //    {
        //        ModelState.AddModelError("userID", "Istifadeci movcud deyil");
        //        return BadRequest(ModelState);
        //    }
        //    //dbUser = db.User.Find(user.id);
        //    //dbUser.username = user.username;
        //    //dbUser.password = methods.EncryptPassword(user.password);
        //    //dbUser.first_name = user.first_name;
        //    //dbUser.last_name = user.last_name;
        //    //dbUser.email = user.email;
        //    //dbUser.mobile = user.mobile;

        //    db.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //        string succesResponse = "Profil ugurla yenilendi";
        //        return Ok(succesResponse);
        //    }
        //    catch (DbUpdateConcurrencyException e)
        //    {
        //        HttpResponseMessage errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
        //        errorResponse.ReasonPhrase = e.Message;
        //        throw new HttpResponseException(errorResponse);

        //    }

        //}



        [HttpPut]
        [Route("api/user/profileUpdate")]
        public IHttpActionResult ProfileUpdate([FromBody]User user)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user.username != null && db.User.Any(x => x.id == user.id && x.username == user.username) == false)
            {
                if (methods.UsernameExists(user.username, user.id) || string.IsNullOrWhiteSpace(user.username))
                {

                    string errorResponse = "Daxil etdiyiniz istifadeci adi artiq movcuddur";
                    return BadRequest(errorResponse);
                }
            }
            if (user.mobile != null && db.User.Any(x => x.id == user.id && x.mobile == user.mobile) == false)
            {
                if (methods.MobileExists(user.mobile, user.id) || string.IsNullOrWhiteSpace(user.mobile))
                {
                    string errorResponse = "Daxil etdiyiniz mobil nomre artiq movcuddur";
                    return BadRequest(errorResponse);
                }
            }

            if (user.email != null && db.User.Any(x => x.id == user.id && x.email == user.email) == false)
            {
                if (string.IsNullOrWhiteSpace(user.email)
                   || methods.EmailExists(user.email, user.id))
                {
                    string errorResponse = "Daxil etdiyiniz email artiq movcuddur";
                    return BadRequest(errorResponse);
                }
            }


            User dbUser = db.User.FirstOrDefault(s => s.id == user.id);

            if (!UserExists(user.id))
            {
                ModelState.AddModelError("userID", "Istifadeci movcud deyil");
                return BadRequest(ModelState);
            }
            dbUser = db.User.Where(x => x.id == user.id).FirstOrDefault();
            if (user.username != null)
                dbUser.username = user.username;
            if (user.password != null)
            { dbUser.password = methods.EncryptPassword(user.password); }
            if (user.first_name != null)
                dbUser.first_name = user.first_name;
            if (user.last_name != null)
                dbUser.last_name = user.last_name;
            if (user.email != null)
                dbUser.email = user.email;
            if (user.mobile != null)
                dbUser.mobile = user.mobile;

            db.Entry(dbUser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                string succesResponse = "Profil ugurla yenilendi";
                return Ok(succesResponse);
            }
            catch (DbUpdateConcurrencyException e)
            {
                HttpResponseMessage errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorResponse.ReasonPhrase = e.Message;
                throw new HttpResponseException(errorResponse);

            }

        }



        [HttpPost]
        [Route("api/user/addProfilePhoto")]
        public IHttpActionResult AddProfileImage([FromBody]User user)
        {
            User myUser = new Models.User();
            myUser = db.User.Find(user.id);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            if (Request.Content.IsMimeMultipartContent())
            {
                //For larger files, this might need to be added:
                //Request.Content.LoadIntoBufferAsync().Wait();
                Request.Content.ReadAsMultipartAsync(
                        new MultipartMemoryStreamProvider()).ContinueWith((task) =>
                        {
                            MultipartMemoryStreamProvider provider = task.Result;
                            foreach (HttpContent content in provider.Contents)
                            {
                                Stream stream = content.ReadAsStreamAsync().Result;
                                Image image = Image.FromStream(stream);
                                var testName = content.Headers.ContentDisposition.Name;
                                string filePath = HostingEnvironment.MapPath("~/Images/");
                                //Note that the ID is pushed to the request header,
                                //not the content header:
                                string[] headerValues = (string[])Request.Headers.GetValues("UniqueId");
                                string fileName = headerValues[0] + ".jpg";
                                string fullPath = Path.Combine(filePath, fileName);
                                image.Save(fullPath);
                                user.photo = fullPath;
                                myUser.photo = user.photo;
                                db.SaveChanges();
                            }
                        });
                return Ok(result);
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(
                        HttpStatusCode.NotAcceptable,
                        "This request is not properly formatted"));
            }
        }


        [HttpPost]
        [Route("api/user/postUserImage")]
        public HttpResponseMessage PostUserImage(string username)
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

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png", ".jpeg" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.,.jpeg");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {

                            var filePath = HttpContext.Current.Server.MapPath("~/Userimage/" + postedFile.FileName);

                            string a = filePath;

                            postedFile.SaveAs(filePath);

                            User user = db.User.FirstOrDefault(x => x.username == username);
                            user.photo = postedFile.FileName;
                            db.Entry(user).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    var message1 = string.Format("Image Updated Successfully.");
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception)
            {
                var res = string.Format("some Message");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }
    }
}