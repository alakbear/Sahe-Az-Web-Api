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
    public class FavouritesController : ApiController
    {
        private MyDbEntitiesAnnouncement db = new MyDbEntitiesAnnouncement();

        // GET: api/Favourites
        public IQueryable<Favourite> GetFavourites()
        {
            return db.Favourites;
        }

        // GET: api/Favourites/5
        [ResponseType(typeof(Favourite))]
        public IHttpActionResult GetFavourite(int id)
        {
            Favourite favourite = db.Favourites.Find(id);
            if (favourite == null)
            {
                return NotFound();
            }

            return Ok(favourite);
        }

        // PUT: api/Favourites/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutFavourite(int id, Favourite favourite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != favourite.id)
            {
                return BadRequest();
            }

            db.Entry(favourite).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FavouriteExists(id))
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

        // POST: api/Favourites
        [ResponseType(typeof(Favourite))]
        public IHttpActionResult PostFavourite(Favourite favourite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Favourites.Add(favourite);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = favourite.id }, favourite);
        }

        // DELETE: api/Favourites/5
        [ResponseType(typeof(Favourite))]
        public IHttpActionResult DeleteFavourite(int id)
        {
            Favourite favourite = db.Favourites.Find(id);
            if (favourite == null)
            {
                return NotFound();
            }

            db.Favourites.Remove(favourite);
            db.SaveChanges();

            return Ok(favourite);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FavouriteExists(int id)
        {
            return db.Favourites.Count(e => e.id == id) > 0;
        }

//-------------------------------------------------------------------------------------------------------------------



        [HttpPost]
        [Route("api/announcement/addfavourite")]
        public IHttpActionResult AddToFavourites(int _id, Favourite favourite)
        {
            //User usr = new Models.User();
            Announcement newAnn = new Announcement();
            //usr = UserInfos.UserInfo();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            newAnn.user_id = _id;
            favourite.announcement_id = favourite.id;
            db.Favourites.Add(favourite);
            db.SaveChanges();

            return Ok();
        }
    }
}