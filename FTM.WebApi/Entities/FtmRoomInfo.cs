using System.ComponentModel.DataAnnotations;

namespace FTM.WebApi.Entities
{
    public class FtmRoomInfo
    {
        [Key]
        public string RoomId { get; set; }
        public string RoomName { get; set; }
        public string Description { get; set; }
        public bool IsUseable { get; set; }
    }
}
