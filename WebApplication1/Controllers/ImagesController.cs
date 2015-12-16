using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ImagesController : ODataController
    {
        private ImageContext db = new ImageContext();

        // GET: odata/Images
        [EnableQuery]
        public IQueryable<Image> GetImages()
        {
            return db.Images;
        }

        // GET: odata/Images(5)
        [EnableQuery]
        public SingleResult<Image> GetImage([FromODataUri] int key)
        {
            return SingleResult.Create(db.Images.Where(image => image.Id == key));
        }

        // PUT: odata/Images(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Image> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Image image = await db.Images.FindAsync(key);
            if (image == null)
            {
                return NotFound();
            }

            patch.Put(image);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(image);
        }

        // POST: odata/Images
        public async Task<IHttpActionResult> Post(Image image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Images.Add(image);
            await db.SaveChangesAsync();

            return Created(image);
        }

        // PATCH: odata/Images(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Image> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Image image = await db.Images.FindAsync(key);
            if (image == null)
            {
                return NotFound();
            }

            patch.Patch(image);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(image);
        }

        // DELETE: odata/Images(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Image image = await db.Images.FindAsync(key);
            if (image == null)
            {
                return NotFound();
            }

            db.Images.Remove(image);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ImageExists(int key)
        {
            return db.Images.Count(e => e.Id == key) > 0;
        }
    }
}
