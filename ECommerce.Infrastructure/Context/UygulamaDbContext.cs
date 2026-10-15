using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ECommerce.Infrastructure.Context;

public class UygulamaDbContext : IdentityDbContext<UygulamaKullanicisi, IdentityRole<Guid>, Guid>
{
    public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Kategori> Kategoriler { get; set; } = null!;
    public DbSet<Urun> Urunler { get; set; } = null!;
    public DbSet<Siparis> Siparisler { get; set; } = null!;
    public DbSet<SiparisKalemi> SiparisKalemleri { get; set; } = null!;
    public DbSet<Sayfa> Sayfalar { get; set; } = null!;
    public DbSet<SikSorulanSoru> SikSorulanSorular { get; set; } = null!;
    public DbSet<BankaHesabi> BankaHesaplari { get; set; } = null!;
    public DbSet<IletisimMesaji> IletisimMesajlari { get; set; } = null!;
    public DbSet<BlogYazisi> BlogYazilari { get; set; } = null!;
    public DbSet<Slider> Sliderlar { get; set; } = null!;
    public DbSet<SiteAyari> SiteAyarlari { get; set; } = null!;
    public DbSet<Kupon> Kuponlar { get; set; } = null!;
    public DbSet<UrunYorum> UrunYorumlari { get; set; } = null!;
    public DbSet<SemtTeslimatBolgesi> SemtTeslimatBolgeleri { get; set; } = null!;
    public DbSet<UrunVaryant> UrunVaryantlari { get; set; } = null!;
    public DbSet<KartNotuSablonu> KartNotuSablonlari { get; set; } = null!;
    public DbSet<SiparisGorselOnay> SiparisGorselOnaylari { get; set; } = null!;
    public DbSet<Duyuru> Duyurular { get; set; }
    public DbSet<AnasayfaKategori> Anasayfa { get; set; }
    public DbSet<OneCikanAyar> OneCikanAyarlari { get; set; }
    public DbSet<KategoriBanner> KategoriBannerlar { get; set; }
    public DbSet<KampanyaUrun> KampanyaUrunler { get; set; }
    public DbSet<Hakkimizda> HakkimizdaAyarlari { get; set; }
    public DbSet<OdemeBildirimi> OdemeBildirimleri { get; set; }
    public DbSet<TeslimatSaati> TeslimatSaatleri { get; set; }
    public DbSet<Favori> Favoriler { get; set; } = null!;
    public DbSet<KayitliAdres> KayitliAdresler { get; set; } = null!;

    // --- YENİ MENÜ SİSTEMİ TABLOLARI ---
    public DbSet<MenuGrup> MenuGruplari { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<KayitliAdres>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Baslik).HasMaxLength(60).IsRequired();
            entity.Property(a => a.AliciAdSoyad).HasMaxLength(120).IsRequired();
            entity.Property(a => a.Telefon).HasMaxLength(25).IsRequired();
            entity.Property(a => a.Il).HasMaxLength(80).IsRequired();
            entity.Property(a => a.Ilce).HasMaxLength(80).IsRequired();
            entity.Property(a => a.AcikAdres).HasMaxLength(500).IsRequired();
            entity.HasIndex(a => a.KullaniciId);
            entity.HasOne(a => a.Kullanici)
                .WithMany(k => k.KayitliAdresler)
                .HasForeignKey(a => a.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Urun>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Fiyat).HasPrecision(18, 2);
            entity.Property(e => e.Ad).HasMaxLength(200).IsRequired();
            entity.Property(e => e.SeoUrl).HasMaxLength(250).IsRequired();

            entity.HasOne(e => e.Kategori)
                  .WithMany(k => k.Urunler)
                  .HasForeignKey(e => e.KategoriId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<UrunVaryant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FiyatFarki).HasPrecision(18, 2);

            entity.HasOne(e => e.Urun)
                  .WithMany(u => u.Varyantlar)
                  .HasForeignKey(e => e.UrunId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SemtTeslimatBolgesi>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EkTeslimatUcreti).HasPrecision(18, 2);
        });

        builder.Entity<Kategori>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Ad).HasMaxLength(100).IsRequired();
            entity.Property(e => e.SeoUrl).HasMaxLength(150).IsRequired();
        });

        builder.Entity<Siparis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SiparisNumarasi).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ToplamTutar).HasPrecision(18, 2);

            entity.HasOne(e => e.Kullanici)
                  .WithMany(k => k.Siparisler)
                  .HasForeignKey(e => e.KullaniciId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.GorselOnay)
                  .WithOne(g => g.Siparis)
                  .HasForeignKey<SiparisGorselOnay>(g => g.SiparisId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SiparisKalemi>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BirimFiyat).HasPrecision(18, 2);

            entity.HasOne(e => e.Siparis)
                  .WithMany(s => s.SiparisKalemleri)
                  .HasForeignKey(e => e.SiparisId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Urun)
                  .WithMany(u => u.SiparisKalemleri)
                  .HasForeignKey(e => e.UrunId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Kupon>(entity =>
        {
            entity.Property(e => e.IndirimMiktari).HasPrecision(18, 2);
        });

        builder.Entity<UrunYorum>(entity =>
        {
            entity.HasOne(e => e.Urun)
                  .WithMany()
                  .HasForeignKey(e => e.UrunId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --- MENUGRUP VE MENUITEM İLİŞKİ YAPILANDIRMASI & SEEDING ---
        builder.Entity<MenuGrup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Baslik).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Konum).HasMaxLength(50).IsRequired();
            entity.Property(e => e.SistemMi).HasDefaultValue(false);

            // Teslimata hazır, pasif ve silinemez sistem menü grupları
            entity.HasData(
                new MenuGrup
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Baslik = "Footer Kurumsal Menü",
                    Konum = "FooterKurumsal",
                    AktifMi = false,
                    SistemMi = true,
                    OlusturulmaTarihi = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new MenuGrup
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Baslik = "Footer Hızlı Erişim Menü",
                    Konum = "Footer",
                    AktifMi = false,
                    SistemMi = true,
                    OlusturulmaTarihi = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        });

        builder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Baslik).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Url).HasMaxLength(300).IsRequired();

            entity.HasOne(m => m.MenuGrup)
                  .WithMany(g => g.MenuItems)
                  .HasForeignKey(m => m.MenuGrupId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Parent)
                  .WithMany(m => m.AltMenuler)
                  .HasForeignKey(m => m.ParentId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
