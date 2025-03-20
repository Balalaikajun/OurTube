using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using OurTube.Domain.Events.VideoVote;

namespace OurTube.Domain.Entities
{
    [PrimaryKey(nameof(VideoId), nameof(ApplicationUserId))]
    public class VideoVote:BaseEntity
    {
        public int VideoId { get; private set; }
        public string ApplicationUserId { get; private set; }

        [Required]
        public bool Type { get; private set; }
        [Required]
        public DateTime Created { get; private set; } = DateTime.UtcNow;

        //Navigation
        public Video Video { get; }
        public ApplicationUser ApplicationUser { get;}

        public VideoVote()
        {
        }

        public VideoVote(int videoId, string applicationUserId, bool type)
        {
            VideoId = videoId;
            ApplicationUserId = applicationUserId;
            Type = type;
            
            CreateEvent();
        }
        
        public void Update(bool type)
        {
            if(Type == type)
                return;

            var oldType = Type;
            Type = type;
            Created = DateTime.UtcNow;
                
            UpdateEvent(oldType);
        }
        
        private void CreateEvent( )
        {
            AddDomainEvent( new VideoVoteCreateEvent(
                VideoId,
                ApplicationUserId,
                Type,
                Created));
        }
        
        private void UpdateEvent(bool oldValue)
        {
            AddDomainEvent( new VideoVoteUpdateEvent(
                VideoId,
                ApplicationUserId,
                oldValue,
                Type,
                Created));
        }
        
        public void RemoveEvent()
        {
            AddDomainEvent( new VideoVoteDeleteEvent(
                VideoId,
                ApplicationUserId,
                Type));
        }
    }
}
