using JBNClassLibrary;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace JBNWebAPI.Controllers
{
    [Authorize]
    public class tblCategoryProductWithCustsController : ApiController
    {
        private mwbtDealerEntities db = new mwbtDealerEntities();

        // GET: api/tblCategoryProductWithCusts
        public IQueryable<tblCategoryProductWithCust> GettblCategoryProductWithCusts()
        {
            return db.tblCategoryProductWithCusts;
        }

        // GET: api/tblCategoryProductWithCusts/5
        [ResponseType(typeof(tblCategoryProductWithCust))]
        public IHttpActionResult GettblCategoryProductWithCust(int id)
        {
            tblCategoryProductWithCust tblCategoryProductWithCust = db.tblCategoryProductWithCusts.Find(id);
            if (tblCategoryProductWithCust == null)
            {
                return NotFound();
            }

            return Ok(tblCategoryProductWithCust);
        }

        // PUT: api/tblCategoryProductWithCusts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PuttblCategoryProductWithCust(int id, [FromBody] tblCategoryProductWithCust tblCategoryProductWithCust)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tblCategoryProductWithCust.ID)
            {
                return BadRequest();
            }

            db.Entry(tblCategoryProductWithCust).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblCategoryProductWithCustExists(id))
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

        // POST: api/tblCategoryProductWithCusts
        [ResponseType(typeof(tblCategoryProductWithCust))]
        public IHttpActionResult PosttblCategoryProductWithCust(tblCategoryProductWithCust tblCategoryProductWithCust)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tblCategoryProductWithCusts.Add(tblCategoryProductWithCust);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tblCategoryProductWithCust.ID }, tblCategoryProductWithCust);
        }

        // DELETE: api/tblCategoryProductWithCusts/5
        [ResponseType(typeof(tblCategoryProductWithCust))]
        public IHttpActionResult DeletetblCategoryProductWithCust(int id)
        {
            tblCategoryProductWithCust tblCategoryProductWithCust = db.tblCategoryProductWithCusts.Find(id);
            if (tblCategoryProductWithCust == null)
            {
                return NotFound();
            }

            db.tblCategoryProductWithCusts.Remove(tblCategoryProductWithCust);
            db.SaveChanges();

            return Ok(tblCategoryProductWithCust);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblCategoryProductWithCustExists(int id)
        {
            return db.tblCategoryProductWithCusts.Count(e => e.ID == id) > 0;
        }
    }
}