using System;

namespace Digin_Kompetanse.Models
{
    public class LoginToken
    {
        public int Id { get; set; }
        public int BedriftId { get; set; }
        public Bedrift Bedrift { get; set; } = null!;
        public string CodeHash { get; set; } = null!;   
        public DateTime ExpiresAt { get; set; }        
        public int Attempts { get; set; } = 0;         
        public DateTime? ConsumedAt { get; set; }      
    }
}


