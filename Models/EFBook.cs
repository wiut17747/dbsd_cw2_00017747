using System;
using System.ComponentModel.DataAnnotations;

namespace dbsd_cw2_00017747.Models {
    public class EFBook {

        public int id {
            get;
            set;
        }
        [Required]
        public string title {
            get;
            set;
        }
        [Required]
        public string physical_location {
            get;
            set;
        }

        public bool is_available {
            get;
            set;
        }

        [Required]
        public string isbn {
            get;
            set;
        }

        [Required]
        public int publisher_id {
            get;
            set;
        }

        public byte[] cover_image {
            get;
            set;
        }

        [Required]
        public string language {
            get;
            set;
        }
        public DateTime? publication_date {
            get;
            set;
        }



    }
}