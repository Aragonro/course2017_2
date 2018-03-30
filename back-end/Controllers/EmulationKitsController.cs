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
using EmulCurs.Models;
using System.Collections.ObjectModel;

namespace EmulCurs.Controllers
{
    public class EmulationKitsController : ApiController
    {
        private EmulCursContext db = new EmulCursContext();

        // GET: api/EmulationKits
        public IQueryable<EmulationKit> GetEmulationKits()
        {
            return db.EmulationKits;
        }

        // GET: api/EmulationKit/email
        [ResponseType(typeof(Collection<EmulationKit>))]
        public Collection<EmulationKit> GetEmulationKit(string email)
        {

            Collection<EmulationKit> emulkitCollect= new Collection<EmulationKit>();
            IEnumerator<EmulationKit> emulkitIE = db.EmulationKits.SqlQuery("SELECT * FROM dbo.EmulationKits WHERE UserId = (Select UserId FROM dbo.Users WHERE @p0=Email)", email).GetEnumerator();
            while (emulkitIE.MoveNext())
            {
                emulkitCollect.Add(emulkitIE.Current);
            }
            return emulkitCollect;
        }

        
        [ResponseType(typeof(void))]
        public IHttpActionResult GetEmulationKit(string login,int idUser, int idEmulation)
        {

            IEnumerator<EmulationKit> emulkitIE = db.EmulationKits.SqlQuery("SELECT * FROM dbo.EmulationKits WHERE EmulationKitId = @p0 AND UserId = @p1", idEmulation, idUser).GetEnumerator();
            if (!emulkitIE.MoveNext())
            {
                return BadRequest();
            }
            IEnumerator<User> userIE = db.Users.SqlQuery("SELECT * FROM dbo.Users WHERE UserId = @p0 AND Login=@p1", idUser, login).GetEnumerator();
            if (!userIE.MoveNext())
            {
                return BadRequest();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: api/EmulationKits/5
        [ResponseType(typeof(EmulationKit))]
        public IHttpActionResult GetEmulationKit(int id)
        {
            EmulationKit emulationKit = db.EmulationKits.Find(id);
            if (emulationKit == null)
            {
                return NotFound();
            }

            return Ok(emulationKit);
        }

        [ResponseType(typeof(String))]
        public IHttpActionResult GetEmulationKit(int id, string device)
        {
            EmulationKit emulationKit = db.EmulationKits.Find(id);
            String res = "temperature:";
            if (emulationKit.Temperature == -1000)
            {
                res += "-";
            }
            else
            {
                res += emulationKit.Temperature;
            }
            res += ";pressure:";
            if (emulationKit.Pressure == -1000)
            {
                res += "-";
            }
            else
            {
                res += emulationKit.Pressure;
            }
            res += ";humidity:";
            if (emulationKit.Humidity == -1000)
            {
                res += "-";
            }
            else
            {
                res += emulationKit.Humidity;
            }
            res += ".";

            return Ok(res);
        }

        // PUT: api/EmulationKits/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmulationKit(int id, EmulationKit emulationKit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
        [ResponseType(typeof(EmulationKit))]
        public IHttpActionResult PostEmulationKit(EmulationKit emulationKit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EmulationKits.Add(emulationKit);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = emulationKit.EmulationKitId }, emulationKit);
        }

        // DELETE: api/EmulationKits/5
        [ResponseType(typeof(EmulationKit))]
        public IHttpActionResult DeleteEmulationKit(int id)
        {
            EmulationKit emulationKit = db.EmulationKits.Find(id);
            if (emulationKit == null)
            {
                return NotFound();
            }

            db.EmulationKits.Remove(emulationKit);
            db.SaveChanges();

            return Ok(emulationKit);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmulationKitExists(int id)
        {
            return db.EmulationKits.Count(e => e.EmulationKitId == id) > 0;
        }
    }
}