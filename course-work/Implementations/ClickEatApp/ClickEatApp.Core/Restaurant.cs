using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        [Required]
        [MaxLength(300)]
        public string Description { get; set; }

        public bool IsOpen { get; set; }

        // Ако искаш връзка с потребител, който го управлява:
        public int OwnerId { get; set; }
        public User Owner { get; set; }

        public ICollection<Food> Foods { get; set; }
    }

}
