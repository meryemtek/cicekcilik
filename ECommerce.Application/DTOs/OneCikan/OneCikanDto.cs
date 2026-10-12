namespace ECommerce.Application.DTOs.OneCikan
{
    public class OneCikanDto
    {
        public bool TumUrunleriGoster { get; set; }
        public List<Guid> SecilenKategoriIdleri { get; set; } = new List<Guid>();
        public List<Guid> SecilenUrunIdleri { get; set; } = new List<Guid>(); 
    }
}