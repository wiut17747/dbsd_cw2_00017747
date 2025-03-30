using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbsd_cw2_00017747.Models
{
    public class BookLoanPublisher
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string PublisherName { get; set; }
        public DateTime? LoanDate { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class FilterParameters
    {
        public string PublisherName { get; set; }
        public DateTime? LoanDateFrom { get; set; }
        public DateTime? LoanDateTo { get; set; }
        public bool? IsAvailable { get; set; }
        public string SortColumn { get; set; } = "BookTitle";
        public string SortOrder { get; set; } = "ASC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
