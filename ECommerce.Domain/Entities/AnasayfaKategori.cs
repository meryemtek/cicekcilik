namespace ECommerce.Domain.Entities
{
    public class AnasayfaKategori : TemelVarlik
    {
        public Guid KategoriId { get; set; }
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; } = true;
        public virtual Kategori Kategori { get; set; } = null!;
    }
}