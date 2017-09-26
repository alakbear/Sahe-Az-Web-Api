using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SaheAzWebApiAlakbarAlakbarov.Models;
using SaheAzWebApiAlakbarAlakbarov.Utilities;


namespace SaheAzWebApiAlakbarAlakbarov.Controllers
{
    public class AnnouncementsController : ApiController
    {
        private MyDbEntitiesAnnouncement db = new MyDbEntitiesAnnouncement();

        // GET: api/Announcements
        public IQueryable<Announcement> GetAnnouncement()
        {
            return db.Announcement;
        }

        // GET: api/Announcements/5
        [ResponseType(typeof(Announcement))]
        public IHttpActionResult GetAnnouncement(int id)
        {
            Announcement announcement = db.Announcement.Find(id);
            if (announcement == null)
            {
                return NotFound();
            }

            return Ok(announcement);
        }

        // PUT: api/Announcements/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAnnouncement(int id, Announcement announcement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != announcement.id)
            {
                return BadRequest();
            }

            db.Entry(announcement).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnouncementExists(id))
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

        // POST: api/Announcements
        [ResponseType(typeof(Announcement))]
        public IHttpActionResult PostAnnouncement(Announcement announcement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Announcement.Add(announcement);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = announcement.id }, announcement);
        }

        // DELETE: api/Announcements/5
        [ResponseType(typeof(Announcement))]
        public IHttpActionResult DeleteAnnouncement(int id)
        {
            Announcement announcement = db.Announcement.Find(id);
            if (announcement == null)
            {
                string message = "Meqale tapilmadi. Id yanlishdir.";
                return Ok(message);
            }

            db.Announcement.Remove(announcement);
            db.SaveChanges();

            return Ok(announcement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AnnouncementExists(int id)
        {
            return db.Announcement.Count(e => e.id == id) > 0;
        }



        [HttpPost]
        [Route("api/announcement/publishAnnouncement")]
        public IHttpActionResult PublishAnnouncement([FromBody]Announcement announcement)
        {
            User usr = new Models.User();
            Announcement newAnn = new Announcement();
            usr = UserInfos.UserInfo();
            newAnn.user_id = usr.id;
            newAnn.title = announcement.title;
            newAnn.body = announcement.body;
            newAnn.publish_date = announcement.publish_date;
            db.Announcement.Add(newAnn);
            try
            {
                db.SaveChanges();
                string succesResponse = "Meqale derc olundu";
                return Ok(succesResponse);
            }
            catch (DbUpdateConcurrencyException e)
            {
                HttpResponseMessage errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorResponse.ReasonPhrase = e.Message;
                throw new HttpResponseException(errorResponse);
            }
        }





        [HttpPut]
        [Route("api/announcement/edit")]
        public IHttpActionResult EditAnnouncement([FromBody]Announcement announcement)
        {

            User dbUser = new Models.User();
            Announcement ann = new Models.Announcement();

            ann = db.Announcement.Find(announcement.id);
            if (ann == null)
            {
                ModelState.AddModelError("announcementID", "Meqale ID si bazada tapilmadi.");
                return BadRequest(ModelState);
            }
            ann.title = announcement.title;
            ann.body = announcement.body;
            ann.publish_date = announcement.publish_date;

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


        [HttpDelete]
        [Route("api/announcement/delete")]
        public IHttpActionResult RemoveAnnouncement(Announcement announcement)
        {
            Announcement deletedAnnouncement = db.Announcement.FirstOrDefault(e => e.id == announcement.id);
            //var deletedAnnouncement = db.Announcement.Find(id);
            db.Announcement.Remove(deletedAnnouncement);
            db.SaveChangesAsync();
            string succesMessage = "Siz meqaleni ugurla sildiniz.";
            return Ok(succesMessage);
        }


//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        [HttpDelete]
        [Route("api/announcement/testdelete")]
        public IHttpActionResult removeTestAnn(int id)
        {
            Announcement announcement = db.Announcement.Find(id);
            if (announcement == null)
            {
                return NotFound();
            }

            db.Announcement.Remove(announcement);
            db.SaveChanges();

            return Ok(announcement);
        }



        [HttpPost]
        [Route("api/announcement/insertAnnouncement")]
        public IHttpActionResult InsertAnnouncement([FromBody]Announcement announcement)
        {
            //User usr = new Models.User();
            Announcement newAnn = new Announcement();
            //usr = UserInfos.UserInfo();
            //newAnn.user_id = usr.id;
            newAnn.user_id = 3002;
            newAnn.title = announcement.title;
            newAnn.body = announcement.body;
            newAnn.publish_date = DateTime.Now; //announcement.publish_date;
            //db.Entry(newAnn).State = EntityState.Modified;
            db.Announcement.Add(newAnn);

            try
            {
                
                db.SaveChanges();
                string succesResponse = "Meqale derc olundu";
                return Ok(succesResponse);
            }
            catch (DbUpdateConcurrencyException e)
            {
                HttpResponseMessage errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorResponse.ReasonPhrase = e.Message;
                throw new HttpResponseException(errorResponse);
            }
        }




        [HttpPut]
        [Route("api/announcement/testedit")]
        public IHttpActionResult TestEditAnn([FromBody] Announcement announcement)
        {
            Announcement newAnn = new Announcement();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            newAnn = db.Announcement.Find(announcement.id);
            newAnn.title = announcement.title;
            newAnn.body = announcement.body;
            newAnn.publish_date = DateTime.Now;
            

            db.Entry(newAnn).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnouncementExists(announcement.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("api/announcement/getlastten")]
        public List<Announcement> getLastTen()
        {
            using(MyDbEntitiesAnnouncement testDb = new MyDbEntitiesAnnouncement())
            {
                return testDb.Announcement.OrderByDescending(a => a.id).Take(10).ToList();
            }

        }



        //public List<Announcement> searchByFilter([FromBody]int announcement_type, int number_of_rooms, 
        //                                         int plot, int minPrice, int maxPrice, int city, int district, int subway)
        //{ 

        //    using(MyDbEntitiesAnnouncement testDb = new MyDbEntitiesAnnouncement())
        //    {
                //var result = testDb.Announcement.Where(s => s.ann = announcement)
                //return testDb.Announcement.
                //return 
        //    }
        //}
        
    }
} 
 