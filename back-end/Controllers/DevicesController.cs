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
    public class DevicesController : ApiController
    {
        private EmulCursContext db = new EmulCursContext();

        // GET: api/Devices
        public IQueryable<Device> GetDevices()
        {
            return db.Devices;
        }

        // GET: api/Devices/5
        [ResponseType(typeof(int))]
        public IHttpActionResult GetDevice(int id)
        {
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return NotFound();
            }

            return Ok(device.EmulationKitId);
        }
        [ResponseType(typeof(int))]
        public IHttpActionResult GetDevice(int st,string email,string email1)
        {
            Device device = db.Devices.Find(st);
            if (device == null)
            {
                return NotFound();
            }

            return Ok(device.Status);
        }

        // GET: api/Devices/5
        [ResponseType(typeof(Collection<Device>))]
        public Collection<Device> GetDevices(int idUser, string email)
        {

            Collection<Device> deviceCollect = new Collection<Device>();
            IEnumerator<Device> deviceIE = db.Devices.SqlQuery("SELECT * FROM dbo.Devices WHERE UserId = @p0", idUser).GetEnumerator();
            while (deviceIE.MoveNext())
            {
                deviceCollect.Add(deviceIE.Current);
            }
            return deviceCollect;
   
        }

        // PUT: api/Devices/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDevice(int id, Device device)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != device.DeviceId)
            {
                return BadRequest();
            }
            Device myDevice = db.Devices.Find(id);
            myDevice.Status = device.Status;
            myDevice.EmulationKitId = device.EmulationKitId;
            db.Entry(myDevice).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
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

        // POST: api/Devices
        [ResponseType(typeof(Device))]
        public IHttpActionResult PostDevice(Device device)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Devices.Add(device);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = device.DeviceId }, device);
        }

        // DELETE: api/Devices/5
        [ResponseType(typeof(Device))]
        public IHttpActionResult DeleteDevice(int id)
        {
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return NotFound();
            }

            db.Devices.Remove(device);
            db.SaveChanges();

            return Ok(device);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DeviceExists(int id)
        {
            return db.Devices.Count(e => e.DeviceId == id) > 0;
        }
    }
}