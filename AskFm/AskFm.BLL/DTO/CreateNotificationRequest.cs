using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskFm.DAL.Enums;

namespace AskFm.BLL.DTO
{
    public class CreateNotificationRequest
    {
        public int UserId { get; set; }
        public NotificationStatus Type { get; set; }
        public int ResourceId { get; set; }
        public string Message { get; set; }
    }
}