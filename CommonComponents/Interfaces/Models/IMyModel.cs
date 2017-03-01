using System;

namespace CommonComponents.Interfaces.Models
{
    public interface IMyModel
    {
        string FName { get; set; }
        int Id { get; set; }
        DateTime Date { get; set; }
    }
}
