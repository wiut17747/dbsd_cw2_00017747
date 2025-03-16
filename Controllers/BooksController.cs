using dbsd_cw2_00017747.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

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


                    }
                }
            } catch (Exception ex) {
                ViewBag.Error = $"Message: {ex.Message} | Inner Exception: {(ex.InnerException?.Message ?? "None")} | Stack Trace: {ex.StackTrace}";
                return View("Error");
            }
            return View(books);
        }
    }
}