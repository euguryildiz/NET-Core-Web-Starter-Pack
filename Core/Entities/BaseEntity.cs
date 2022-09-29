using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class BaseEntity
    {
        [DisplayName("Id")]
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Oluşturma Tarihi")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Güncelleme Tarihi")]
        public DateTime? ModifiedDate { get; set; }

        [DisplayName("Güncelleyen Kişi")]
        public int? ModifiedUserId { get; set; }

        [DisplayName("Oluşturan Kullanıcı")]
        public int CreatedUserId { get; set; }

        [DisplayName("Silme Bilgisi")]
        public bool IsDeleted { get; set; }
    }
}

