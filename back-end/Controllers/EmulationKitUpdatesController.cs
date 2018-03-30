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
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace EmulCurs.Controllers
{
    public class EmulationKitUpdatesController : ApiController
    {
        private EmulCursContext db = new EmulCursContext();

        // GET: api/EmulationKitUpdates
        public IQueryable<EmulationKitUpdate> GetEmulationKitUpdates()
        {
            return db.EmulationKitUpdates;
        }

        // GET: api/EmulationKitUpdates/5
        [ResponseType(typeof(EmulationKitUpdate))]
        public IHttpActionResult GetEmulationKitUpdate(int id)
        {
            EmulationKitUpdate emulationKitUpdate = db.EmulationKitUpdates.Find(id);
            if (emulationKitUpdate == null)
            {
                return NotFound();
            }

            return Ok(emulationKitUpdate);
        }

        // GET: api/EmulationKit/idemul
        [ResponseType(typeof(Collection<EmulationKitUpdate>))]
        public Collection<EmulationKitUpdate> GetEmulationKitUpdate(int idemul, string email)
        {

            Collection<EmulationKitUpdate > emulkitupCollect = new Collection<EmulationKitUpdate>();
            IEnumerator<EmulationKitUpdate> emulkitupIE = db.EmulationKitUpdates.SqlQuery("SELECT * FROM dbo.EmulationKitUpdates WHERE EmulationKitId = @p0", idemul).GetEnumerator();
            while (emulkitupIE.MoveNext())
            {
                emulkitupCollect.Add(emulkitupIE.Current);
            }
            return emulkitupCollect;
        }

        // PUT: api/EmulationKitUpdates/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmulationKitUpdate(int id, EmulationKitUpdate emulationKitUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != emulationKitUpdate.EmulationKitUpdateId)
            {
                return BadRequest();
            }

            db.Entry(emulationKitUpdate).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmulationKitUpdateExists(id))
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

        // POST: api/EmulationKitUpdates
        [ResponseType(typeof(EmulationKitUpdate))]
        public IHttpActionResult PostEmulationKitUpdate(EmulationKitUpdate emulationKitUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EmulationKitUpdates.Add(emulationKitUpdate);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = emulationKitUpdate.EmulationKitUpdateId }, emulationKitUpdate);
        }

        // POST: api/EmulationKitUpdates
        [ResponseType(typeof(EmulationKitUpdate))]
        public IHttpActionResult PostEmulationKitUpdate(int emulationKitId, int temperature,int pressure,int humidity)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            EmulationKitUpdate emulationKitUpdate = null;
            emulationKitUpdate.EmulationKitUpdateId = 45;
            emulationKitUpdate.EmulationKitId = emulationKitId;
            emulationKitUpdate.TemperatureUpdate = temperature;
            emulationKitUpdate.PressureUpdate = pressure;
            emulationKitUpdate.HumidityUpdate = humidity;
            db.EmulationKitUpdates.Add(emulationKitUpdate);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = emulationKitUpdate.EmulationKitUpdateId }, emulationKitUpdate);
        }

        // DELETE: api/EmulationKitUpdates/5
        [ResponseType(typeof(EmulationKitUpdate))]
        public IHttpActionResult DeleteEmulationKitUpdate(int id)
        {
            EmulationKitUpdate emulationKitUpdate = db.EmulationKitUpdates.Find(id);
            if (emulationKitUpdate == null)
            {
                return NotFound();
            }

            db.EmulationKitUpdates.Remove(emulationKitUpdate);
            db.SaveChanges();

            return Ok(emulationKitUpdate);
        }

        // DELETE: api/EmulationKitUpdates/5
        [ResponseType(typeof(EmulationKitUpdate))]
        public IHttpActionResult DeleteEmulationKitUpdate(int idemul, string email)
        {
            IEnumerator<EmulationKitUpdate> emulkitupdateIE = db.EmulationKitUpdates.SqlQuery("SELECT * FROM dbo.EmulationKitUpdates WHERE EmulationKitId=@p0", idemul).GetEnumerator();
            EmulationKitUpdate emulationKitUpdate = null;
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

        // DELETE: api/EmulationKitUpdates/5
        //[ResponseType(typeof(EmulationKitUpdate))]
        //public IHttpActionResult DeleteEmulationKitUpdate(int id, string token)
        //{

        //    EmulationKitUpdate emulationKitUpdate = db.EmulationKitUpdates.Find(id);
        //    if (emulationKitUpdate == null)
        //    {
        //        return NotFound();
        //    }

        //    string email =  GetProductAsync(token).Result; ;
        //    IEnumerator<EmulationKit> emulkitIE = db.EmulationKits.SqlQuery("SELECT * FROM dbo.EmulationKits WHERE EmulationKitId=@p0 AND UserID=(Select UserId FROM dbo.Users Where Email=@p1)", emulationKitUpdate.EmulationKitId, email).GetEnumerator();
        //    if (!emulkitIE.MoveNext())
        //    {
        //        return BadRequest();
        //    }

        //    db.EmulationKitUpdates.Remove(emulationKitUpdate);
        //    db.SaveChanges();

        //    return Ok(emulationKitUpdate);
        //}


        static async Task<string> GetProductAsync(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            UserInfoViewModel userInfoViewModel = null;
            HttpResponseMessage response =await client.GetAsync("http://localhost:10327/api/Account/UserInfo");
            if (response.IsSuccessStatusCode)
            {
                userInfoViewModel = await response.Content.ReadAsAsync<UserInfoViewModel>();
            }
            return userInfoViewModel.Email;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmulationKitUpdateExists(int id)
        {
            return db.EmulationKitUpdates.Count(e => e.EmulationKitUpdateId == id) > 0;
        }
    }
}