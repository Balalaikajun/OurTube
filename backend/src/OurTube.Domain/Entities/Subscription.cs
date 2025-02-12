using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Domain.Entities
{
    
    public class Subscription
    {
        public string SubscribedToId { get; set; }
        public string SubscriberId { get; set; }
        [Required]
        public DateTime Created { get; set; } = DateTime.Now;

        //Navigation
        [ForeignKey("SubscribedToId")]
        public ApplicationUser SubscribedTo { get; set; }
        [ForeignKey("SubscriberId")]
        public ApplicationUser Subscriber { get; set; }

    }
}
