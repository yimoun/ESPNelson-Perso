using System.ComponentModel.DataAnnotations;
namespace StationnementAPI.Models
{
    public class Tarification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Niveau { get; set; }

        [Required]
        public decimal Prix { get; set; }

        [Required]
        public int DureeMin { get; set; }

        [Required]
        public int DureeMax { get; set; }
    }

}
