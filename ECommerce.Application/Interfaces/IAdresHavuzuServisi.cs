using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IAdresHavuzuServisi
    {
        void AdresleriYukle(string dosyaYolu);
        List<string> IlleriGetir();
        List<string> IlceleriGetir(string ilAdi);
        List<string> MahalleleriGetir(string ilAdi, string ilceAdi);
    }
}
