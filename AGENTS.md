# ECommerce.Solution

## Teknoloji ve yapı
- Çözüm .NET 8 / C# kullanır; nullable reference types ve implicit usings tüm projelerde açıktır.
- `ECommerce.Domain`: varlıklar ve enum'lar. `ECommerce.Application`: DTO'lar, arayüzler, servisler ve repository sözleşmeleri; Domain'e bağlıdır.
- `ECommerce.Infrastructure`: EF Core 8, PostgreSQL (Npgsql), `UygulamaDbContext`, repository/servis uygulamaları ve PostgreSQL migration'ları. Application ve Domain'e bağlıdır. Eski SQL Server migration'ları `ECommerce.Infrastructure.Migrations.SqlServer` arşiv klasöründe saklanır.
- `ECommerce.WebAPI`: ASP.NET Core API, Identity/JWT ve Swagger; Application ve Infrastructure'ı kullanır.
- `ECommerce.WebUI`: ASP.NET Core MVC mağaza ve admin arayüzü; Application'a bağlıdır ve API'ye `ApiClient` adlı HttpClient ile istek atar.
- Katman sınırlarını koru: iş sözleşmeleri/servisleri Application'da, kalıcılık ve dış servis uygulamaları Infrastructure'da, HTTP uçları WebAPI'de, MVC arayüzü WebUI'da tutulur.

## Kod biçimi
- Dosyanın mevcut biçimini (girinti, satır sonu ve adlandırma) koru; depoda merkezi `.editorconfig` bulunmuyor.
- C# türleri ve metotlar PascalCase kullanır; asenkron metot adları mevcut kodda `Async` ile biter.
- Yeni veritabanı şema değişikliklerini Infrastructure migration'ı olarak ekle ve migration'ları uygulama başlangıç projesi WebAPI olacak şekilde çalıştır.

## Geliştirme ve doğrulama
Komutları çözüm kökünden çalıştır:

```powershell
dotnet restore ECommerce.Solution.sln
dotnet build ECommerce.Solution.sln
dotnet run --project ECommerce.WebAPI --launch-profile https  # https://localhost:7192, Swagger: /swagger
dotnet run --project ECommerce.WebUI --launch-profile http   # http://localhost:5037
dotnet ef database update --project ECommerce.Infrastructure --startup-project ECommerce.WebAPI
```

API veritabanı bağlantısını `ConnectionStrings:VarsayilanBaglanti` ayarından alır. Yerel geliştirmede Neon bağlantısını Git'e eklenmeyen User Secrets alanına kaydedin:

```powershell
dotnet user-secrets set "ConnectionStrings:VarsayilanBaglanti" "Host=...;Database=...;Username=...;Password=...;SSL Mode=Require" --project ECommerce.WebAPI
```

Canlı ortamda `ConnectionStrings__VarsayilanBaglanti` ortam değişkenini kullanın. Veritabanı migration'larını WebAPI başlangıç projesiyle uygulayın:

```powershell
dotnet ef database update --project ECommerce.Infrastructure --startup-project ECommerce.WebAPI
```
