using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using dbsd_cw2_00017747.Models;

namespace dbsd_cw2_00017747.Controllers
{
    public class BookLoanController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
        public ActionResult Index(FilterParameters filterParameters = null)
        {
            if (filterParameters == null)
            {
                filterParameters = new FilterParameters();
            }
            
            List<BookLoanPublisher> result = new List<BookLoanPublisher>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetFiltered_Book_Loan_Publisher", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PublisherName", string.IsNullOrEmpty(filterParameters.PublisherName) ? (object)DBNull.Value : filterParameters.PublisherName);
                    cmd.Parameters.AddWithValue("@IsAvailable", filterParameters.IsAvailable.HasValue ? (object)filterParameters.IsAvailable.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoanDateFrom", filterParameters.LoanDateFrom.HasValue ? (object)filterParameters.LoanDateFrom.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoanDateTo", filterParameters.LoanDateTo.HasValue ? (object)filterParameters.LoanDateTo.Value : DBNull.Value); cmd.Parameters.AddWithValue("@SortColumn", filterParameters.SortColumn);
                    cmd.Parameters.AddWithValue("@SortOrder", filterParameters.SortOrder);
                    cmd.Parameters.AddWithValue("@PageNumber", filterParameters.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", filterParameters.PageSize);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["LoanDate"] != DBNull.Value)
                            {
                                result.Add(new BookLoanPublisher
                            {
                                BookId = Convert.ToInt32(reader["BookId"]),
                                BookTitle = Convert.ToString(reader["BookTitle"]),
                                PublisherName = Convert.ToString(reader["PublisherName"]),
                                LoanDate = Convert.ToDateTime(reader["LoanDate"]),
                                IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
                            });
                            }
                            else
                            {
                                result.Add(new BookLoanPublisher
                            {
                                BookId = Convert.ToInt32(reader["BookId"]),
                                BookTitle = Convert.ToString(reader["BookTitle"]),
                                PublisherName = Convert.ToString(reader["PublisherName"]),
                                LoanDate = null,
                                IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
                            });
                            }
                        }
                    }
                }
            }
            // ViewBag.Filter = filter TODO()
            ViewBag.Filter = filterParameters;
            return View(result);
        }

        public ActionResult ExportXml(dbsd_cw2_00017747.Models.FilterParameters filter)
        {
            StringBuilder xmlData = new StringBuilder();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("get_Xml", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PublisherName", string.IsNullOrEmpty(filter.PublisherName) ? (object)DBNull.Value : filter.PublisherName);
                    command.Parameters.AddWithValue("@LoanDateFrom", filter.LoanDateFrom.HasValue ? (object)filter.LoanDateFrom.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@LoanDateTo", filter.LoanDateTo.HasValue ? (object)filter.LoanDateTo.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@IsAvailable", filter.IsAvailable.HasValue ? (object)filter.IsAvailable.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@SortColumn", filter.SortColumn);
                    command.Parameters.AddWithValue("@SortOrder", filter.SortOrder);

                    using (XmlReader reader = command.ExecuteXmlReader())
                    {
                        while (reader.Read())
                        {
                            xmlData.Append(reader.ReadOuterXml());
                        }
                    }
                }
            }

            if (xmlData.Length == 0)
            {
                return Content("No data to export.");
            }

            return File(Encoding.UTF8.GetBytes(xmlData.ToString()), "application/xml", "BookLoanData.xml");
        }
    }
}