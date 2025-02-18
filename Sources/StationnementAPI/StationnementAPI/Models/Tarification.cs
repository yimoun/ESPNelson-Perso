using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Column(TypeName = "decimal(10,2)")]
        public decimal Prix { get; set; }

        [Required]
        public int DureeMin { get; set; }

        [Required]
        public int DureeMax { get; set; }
    }

}
