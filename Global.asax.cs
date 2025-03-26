using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace dbsd_cw2_00017747 {
    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            create_access_control_trigger();

            string path = Server.MapPath("~/App_Data");
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        private void create_access_control_trigger() {
            string connection_str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            string drop_trigger_sql = @"
        IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'trg_restrict_books_operations')
            DROP TRIGGER trg_restrict_books_operations;
    ";


            string create_trigger_sql = @"
        CREATE TRIGGER trg_restrict_books_operations
        ON Book
        FOR INSERT, UPDATE, DELETE
        AS
        BEGIN
            DECLARE @CurrentHour INT;
            SET @CurrentHour = DATEPART(HOUR, GETDATE());

            IF (@CurrentHour < 10 OR @CurrentHour >= 17)
            BEGIN
                RAISERROR ('Operations on Book table are not allowed outside office hours (10:00 - 17:00).', 16, 1);
                ROLLBACK TRANSACTION;
                RETURN;
            END;
        END;
    ";

            using (SqlConnection connection = new SqlConnection(connection_str)) {
                try {
                    connection.Open();

                    using (SqlCommand dropCommand = new SqlCommand(drop_trigger_sql, connection)) {
                        dropCommand.ExecuteNonQuery();
                    }

                    using (SqlCommand createCommand = new SqlCommand(create_trigger_sql, connection)) {
                        createCommand.ExecuteNonQuery();
                    }
                } catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"Error creating trigger: {ex.Message}");
                    throw;
                } finally {
                    connection.Close();
                }
            }
        }
    }
}
