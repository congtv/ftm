using System.ComponentModel.DataAnnotations;

namespace FTM.WebApi.Entities
{
    public class RoomInfo
    {
        [Key]
        public string RoomId { get; set; }
        public string RoomName { get; set; }
    }
}
