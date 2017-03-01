using CommonComponents.Interfaces.Models;
using System;

namespace CommonComponents.Concrete.Models
{
    internal class MyModel : IMyModel
    {
        public string FName { get; set; }
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
}
