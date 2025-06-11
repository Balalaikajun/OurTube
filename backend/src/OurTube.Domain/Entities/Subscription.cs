namespace OurTube.Domain.Entities;

public class Subscription
{
    public string SubscribedToId { get; set; }
    public string SubscriberId { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;

    //Navigation
    public ApplicationUser SubscribedTo { get; set; }
    public ApplicationUser Subscriber { get; set; }
}