using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dbsd_cw2_00017747.Models;

namespace dbsd_cw2_00017747.Controllers {
    public class BooksController : Controller {
        // GET: Books
        private string connection_string = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public ActionResult Index() {
            List<Book> books = new List<Book>();

            try {


                using (SqlConnection conn = new SqlConnection(connection_string)) {
                    SqlCommand cmd = new SqlCommand("sp_Get_All_Books", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        books.Add(new Book {
                            id = (int)reader["id"],
                            title = reader["title"].ToString(),
                            physical_location = reader["physical_location"].ToString(),
                            is_available = (bool)reader["is_available"],
                            isbn = reader["isbn"].ToString(),
                            publisher_id = (int)reader["publisher_id"],
                            cover_image = reader["cover_image"] as byte[],
                            language = reader["language"].ToString(),
                            publication_date = reader["publication_date"] as DateTime?
                        });
                        System.Diagnostics.Debug.WriteLine($"Book ID: {books.Last().id}, Cover Image: {(books.Last().cover_image != null ? $"Length: {books.Last().cover_image.Length}" : "null")}");

                    }
                }
            } catch (Exception ex) {
                ViewBag.Error = $"Message: {ex.Message} | Inner Exception: {(ex.InnerException?.Message ?? "None")} | Stack Trace: {ex.StackTrace}";
                return View("Error");
            }
            return View(books);
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "cover_image")] Book book, HttpPostedFileBase cover_image) {
            System.Diagnostics.Debug.WriteLine("Entered Create POST action");

            if (ModelState.IsValid) {
                System.Diagnostics.Debug.WriteLine($"cover_image: {cover_image?.FileName}, ContentLength: {cover_image?.ContentLength}, ContentType: {cover_image?.ContentType}");

                if (cover_image != null && cover_image.ContentLength > 0) {
                    System.Diagnostics.Debug.WriteLine("File detected, processing...");
                    if (!cover_image.ContentType.StartsWith("image/")) {
                        ModelState.AddModelError("cover_image", "Please upload a valid image file (e.g., JPEG, PNG).");
                        return View(book);
                    }

                    try {
                        using (var ms = new MemoryStream()) {
                            cover_image.InputStream.CopyTo(ms);
                            book.cover_image = ms.ToArray();
                            System.Diagnostics.Debug.WriteLine($"cover_image byte array length: {book.cover_image.Length}, First few bytes: {BitConverter.ToString(book.cover_image.Take(10).ToArray())}");
                        }
                    } catch (Exception ex) {
                        ModelState.AddModelError("cover_image", $"Error processing image: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Image processing error: {ex.Message}");
                        return View(book);
                    }
                } else {
                    System.Diagnostics.Debug.WriteLine("No cover image uploaded or file is empty.");
                    book.cover_image = null;
                }

                try {
                    using (SqlConnection conn = new SqlConnection(connection_string)) {
                        SqlCommand cmd = new SqlCommand("sp_Create_Book", conn);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@title", (object)book.title ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@physical_location", (object)book.physical_location ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@is_available", (object)book.is_available);
                        cmd.Parameters.AddWithValue("@isbn", (object)book.isbn ?? DBNull.Value);
                        cmd.Parameters.Add("@cover_image", SqlDbType.VarBinary).Value = (object)book.cover_image ?? DBNull.Value;
                        cmd.Parameters.AddWithValue("@language", (object)book.language ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@publication_date", (object)book.publication_date ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@publisher_id", (object)book.publisher_id ?? DBNull.Value);

                        System.Diagnostics.Debug.WriteLine($"Saving to database: @cover_image is {(book.cover_image != null ? $"not null, length: {book.cover_image.Length}" : "null")}");

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    return RedirectToAction("Index");
                } catch (Exception ex) {
                    ModelState.AddModelError("", $"error saving book: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"database error: {ex.Message}");
                    return View(book);
                }
            } else {
                System.Diagnostics.Debug.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors)) {
                    System.Diagnostics.Debug.WriteLine($"Error: {error.ErrorMessage}");
                }
            }

            return View(book);
        }

        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Book book = null;
            try {
                using (SqlConnection conn = new SqlConnection(connection_string)) {
                    SqlCommand cmd = new SqlCommand("sp_Get_Book_By_Id", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id.Value;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) {
                        book = new Book {
                            id = (int)reader["id"],
                            title = reader["title"].ToString(),
                            physical_location = reader["physical_location"].ToString(),
                            is_available = (bool)reader["is_available"],
                            isbn = reader["isbn"].ToString(),
                            publisher_id = (int)reader["publisher_id"],
                            cover_image = reader["cover_image"] as byte[],
                            language = reader["language"].ToString(),
                            publication_date = reader["publication_date"] as DateTime?
                        };
                    }
                }
            } catch (Exception ex) {
                ViewBag.Error = $"Error retrieving book: {ex.Message}";
                return View("Error");
            }

            if (book == null) {
                return HttpNotFound();
            }

            return View(book);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            try {
                using (SqlConnection conn = new SqlConnection(connection_string)) {
                    SqlCommand cmd = new SqlCommand("sp_Delete_Book", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            } catch (Exception ex) {
                ViewBag.Error = $"error deleting book: {ex.Message}";
                return View("Error");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Book book = null;
            try {
                using (SqlConnection conn = new SqlConnection(connection_string)) {
                    SqlCommand cmd = new SqlCommand("sp_Get_Book_By_Id", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id.Value;

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) {
                        book = new Book {
                            id = (int)reader["id"],
                            title = reader["title"].ToString(),
                            physical_location = reader["physical_location"].ToString(),
                            is_available = (bool)reader["is_available"],
                            isbn = reader["isbn"].ToString(),
                            publisher_id = (int)reader["publisher_id"],
                            cover_image = reader["cover_image"] as byte[],
                            language = reader["language"].ToString(),
                            publication_date = reader["publication_date"] as DateTime?
                        };
                    }
                }
            } catch (Exception ex) {
                ViewBag.Error = $"Error retrieving book: {ex.Message}";
                return View("Error");
            }

            if (book == null) {
                return HttpNotFound();
            }

            return View(book);
        }

        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Book book = null;
            try {
                using (SqlConnection conn = new SqlConnection(connection_string)) {
                    SqlCommand cmd = new SqlCommand("sp_Get_Book_By_Id", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id.Value;

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) {
                        book = new Book {
                            id = (int)reader["id"],
                            title = reader["title"].ToString(),
                            physical_location = reader["physical_location"].ToString(),
                            is_available = (bool)reader["is_available"],
                            isbn = reader["isbn"].ToString(),
                            publisher_id = (int)reader["publisher_id"],
                            cover_image = reader["cover_image"] as byte[],
                            language = reader["language"].ToString(),
                            publication_date = reader["publication_date"] as DateTime?
                        };
                    }
                }
            } catch (Exception ex) {
                ViewBag.Error = $"Error retrieving book: {ex.Message}";
                return View("Error");
            }

            if (book == null) {
                return HttpNotFound();
            }

            return View(book);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "cover_image")] Book book, HttpPostedFileBase cover_image) {
            System.Diagnostics.Debug.WriteLine("Entered Edit POST action");

            if (ModelState.IsValid) {
                System.Diagnostics.Debug.WriteLine($"cover_image: {cover_image?.FileName}, ContentLength: {cover_image?.ContentLength}, ContentType: {cover_image?.ContentType}");

                byte[] coverImageBytes = null;
                // If a new image is uploaded, process it
                if (cover_image != null && cover_image.ContentLength > 0) {
                    System.Diagnostics.Debug.WriteLine("File detected, processing...");
                    if (!cover_image.ContentType.StartsWith("image/")) {
                        ModelState.AddModelError("cover_image", "Please upload a valid image file (e.g., JPEG, PNG).");
                        return View(book);
                    }

                    try {
                        using (var ms = new MemoryStream()) {
                            cover_image.InputStream.CopyTo(ms);
                            coverImageBytes = ms.ToArray();
                            System.Diagnostics.Debug.WriteLine($"cover_image byte array length: {coverImageBytes.Length}, First few bytes: {BitConverter.ToString(coverImageBytes.Take(10).ToArray())}");
                        }
                    } catch (Exception ex) {
                        ModelState.AddModelError("cover_image", $"Error processing image: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Image processing error: {ex.Message}");
                        return View(book);
                    }
                } else {
                    // If no new image is uploaded, fetch the existing image from the database
                    System.Diagnostics.Debug.WriteLine("No new cover image uploaded, retrieving existing image...");
                    try {
                        using (SqlConnection conn = new SqlConnection(connection_string)) {
                            SqlCommand cmd = new SqlCommand("SELECT cover_image FROM Book WHERE id = @id", conn);
                            cmd.Parameters.AddWithValue("@id", book.id);
                            conn.Open();
                            var result = cmd.ExecuteScalar();
                            coverImageBytes = result as byte[];
                            System.Diagnostics.Debug.WriteLine($"Existing cover_image: {(coverImageBytes != null ? $"Length: {coverImageBytes.Length}" : "null")}");
                        }
                    } catch (Exception ex) {
                        ModelState.AddModelError("", $"Error retrieving existing image: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Image retrieval error: {ex.Message}");
                        return View(book);
                    }
                }


                try {
                    using (SqlConnection conn = new SqlConnection(connection_string)) {
                        SqlCommand cmd = new SqlCommand("sp_Update_Book", conn);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", book.id);
                        cmd.Parameters.AddWithValue("@title", (object)book.title ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@physical_location", (object)book.physical_location ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@is_available", (object)book.is_available);
                        cmd.Parameters.AddWithValue("@isbn", (object)book.isbn ?? DBNull.Value);
                        cmd.Parameters.Add("@cover_image", SqlDbType.VarBinary).Value = (object)coverImageBytes ?? DBNull.Value;
                        cmd.Parameters.AddWithValue("@language", (object)book.language ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@publication_date", (object)book.publication_date ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@publisher_id", (object)book.publisher_id ?? DBNull.Value);

                        System.Diagnostics.Debug.WriteLine($"Updating database: @cover_image is {(coverImageBytes != null ? $"not null, length: {coverImageBytes.Length}" : "null")}");

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    return RedirectToAction("Index");
                } catch (Exception ex) {
                    ModelState.AddModelError("", $"Error updating book: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                    return View(book);
                }
            } else {
                System.Diagnostics.Debug.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors)) {
                    System.Diagnostics.Debug.WriteLine($"Error: {error.ErrorMessage}");
                }
            }

            return View(book);
        }

    }
}