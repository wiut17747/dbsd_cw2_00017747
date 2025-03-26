using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dbsd_cw2_00017747.Models;

namespace dbsd_cw2_00017747.Controllers {
    public class EntityBooksController : Controller {
        private EFBookEntities db = new EFBookEntities();
        // GET: EntityBooks
        public ActionResult Index() {
            return View(db.EFBooks.ToList());
        }

        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            EFBook book = db.EFBooks.Find(id);

            if (book == null) {
                return new HttpNotFoundResult();
            }
            return View(book);
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EFBook book, HttpPostedFileBase coverImageFile) {
            if (ModelState.IsValid) {
                if (coverImageFile != null && coverImageFile.ContentLength > 0) {
                    if (!coverImageFile.ContentType.StartsWith("image/")) {
                        ModelState.AddModelError("cover_image", "Please upload a valid image file (e.g., JPEG, PNG).");
                        return View(book);
                    }

                    try {
                        using (var ms = new MemoryStream()) {
                            coverImageFile.InputStream.CopyTo(ms);
                            book.cover_image = ms.ToArray();
                        }
                    } catch (Exception ex) {
                        ModelState.AddModelError("coverImageFile", $"Error processing image: {ex.Message}");
                        return View(book);
                    }
                }

                if (!string.IsNullOrEmpty(book.isbn) && db.EFBooks.Any(b => b.isbn == book.isbn && b.id != book.id)) {
                    ModelState.AddModelError("isbn", "This ISBN already exists. Please use a unique ISBN.");
                    return View(book);
                }

                db.EFBooks.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }

        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            EFBook book = db.EFBooks.Find(id);
            if (book == null) {
                return HttpNotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EFBook book, HttpPostedFileBase coverImageFile) {
            if (ModelState.IsValid) {
                var existingBook = db.EFBooks.Find(book.id);
                if (existingBook == null) {
                    return HttpNotFound();
                }

                if (coverImageFile != null && coverImageFile.ContentLength > 0) {
                    if (!coverImageFile.ContentType.StartsWith("image/")) {
                        ModelState.AddModelError("coverImageFile", "Please upload a valid image file (e.g., JPEG, PNG).");
                        return View(book);
                    }

                    try {
                        using (var ms = new MemoryStream()) {
                            coverImageFile.InputStream.CopyTo(ms);
                            book.cover_image = ms.ToArray();
                        }
                    } catch (Exception ex) {
                        ModelState.AddModelError("coverImageFile", $"Error processing image: {ex.Message}");
                        return View(book);
                    }
                } else {
                    book.cover_image = existingBook.cover_image;
                }

                if (!string.IsNullOrEmpty(book.isbn) && db.EFBooks.Any(b => b.isbn == book.isbn && b.id != book.id)) {
                    ModelState.AddModelError("isbn", "This ISBN already exists. Please use a unique ISBN.");
                    return View(book);
                }

                db.Entry(existingBook).CurrentValues.SetValues(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            EFBook book = db.EFBooks.Find(id);
            if (book == null) {
                return HttpNotFound();
            }
            return View(book);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            EFBook book = db.EFBooks.Find(id);
            if (book == null) {
                return HttpNotFound();
            }
            db.EFBooks.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}