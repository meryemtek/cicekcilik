using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Entities
{
    public class MenuItem : TemelVarlik
    {
        // Hangi menü grubuna (kutuya) ait?
        public Guid MenuGrupId { get; set; }
        public virtual MenuGrup MenuGrup { get; set; } = null!;

        public string Baslik { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? IconClass { get; set; }
        public string Target { get; set; } = "_self"; // "_blank" (Yeni sekme) veya "_self"
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; } = true; // Menü elemanını aktif/pasif yapmak için

        // Kendi içindeki hiyerarşi (Alt Menü)
        public Guid? ParentId { get; set; }
        public virtual MenuItem? Parent { get; set; }
        public virtual ICollection<MenuItem> AltMenuler { get; set; } = new List<MenuItem>();
    }
}