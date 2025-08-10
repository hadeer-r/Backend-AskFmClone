using System.Runtime.InteropServices.JavaScript;

namespace AskFm.DAL.Models;

public interface ITrackable
{
    bool IsDeleted { get; set; }
    DateTime DeletedAt { get; set; }
}