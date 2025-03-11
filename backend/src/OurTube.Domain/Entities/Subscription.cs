using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurTube.Domain.Entities
{

    public class Subscription
    {
        public string SubscribedToId { get; set; }
        public string SubscriberId { get; set; }
        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        //Navigation
        [ForeignKey("SubscribedToId")]
        public ApplicationUser SubscribedTo { get; set; }
        [ForeignKey("SubscriberId")]
        public ApplicationUser Subscriber { get; set; }

    }
}
