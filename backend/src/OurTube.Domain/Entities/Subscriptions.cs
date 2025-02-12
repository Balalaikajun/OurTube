using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Domain.Entities
{
    internal class Subscriptions
    {
        public string SubscrubedToID { get; set; }
        public string SubscriberID { get; set; }
        [Required]
        public DateTime Created { get; set; } = DateTime.Now;

        //Navigation
        public ApplicationUser SubscrubedTo {  get; set; }
        public ApplicationUser Subscriber { get; set; }

    }
}
