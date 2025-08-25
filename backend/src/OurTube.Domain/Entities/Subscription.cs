namespace OurTube.Domain.Entities;

public class Subscription : Base
{
    public Guid SubscribedToId { get; set; }
    public Guid SubscriberId { get; set; }
    
    //Navigation
    public ApplicationUser SubscribedTo { get; set; }
    public ApplicationUser Subscriber { get; set; }
}