using DAL;
using DomainEntity.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KahveciLINQ
{
    public partial class GunlukRapor : Form
    {
        public GunlukRapor()
        {
            InitializeComponent();
        }

        private void GunlukRapor_Load(object sender, EventArgs e)
        {
            RaporGetir(DateTime.Today);
        }

        public void RaporGetir(DateTime secilenGun)
        {
            KahveContext db = new KahveContext();
            GunlukRaporViewModel rapor = new GunlukRaporViewModel();
            try
            {
                rapor.ToplamSatisTutari = db.Siparisler.Where(x => DbFunctions.TruncateTime(x.Tarih).Value == secilenGun.Date).Sum(x => x.SiparistekiUrunler.Sum(a => a.Tutar));
                //rapor.ToplamSatisTutari = (from x in db.Siparisler select x).Sum(x => x.SiparistekiUrunler.Sum(a => a.Tutar));

                rapor.ToplamSatilanUrunSayisi = db.Siparisler.Where(x => DbFunctions.TruncateTime(x.Tarih).Value == secilenGun.Date).Sum(x => x.SiparistekiUrunler.Sum(a => a.Miktar));

                //SELECT k.KullaniciAdi, COUNT(*) AS SiparisSayisi, SUM(sd.Tutar) AS SatisTutari
                //FROM Kullanicis k
                //INNER JOIN Siparis s
                //ON k.KullaniciID = s.EkleyenKullaniciID
                //INNER JOIN SiparisDetay sd
                //ON s.SiparisID = sd.SiparisID
                //GROUP BY k.KullaniciAdi
                rapor.KullaniciBasiSatislar = (from sd in db.Siparisler
                                               join k in db.Kullanicilar
                                               on sd.KaydedenKullaniciID equals k.KullaniciID
                                               where DbFunctions.TruncateTime(sd.Tarih).Value == secilenGun.Date
                                               group sd by k.KullaniciAdi into yeni
                                               select new KullaniciSatisViewModel()
                                               {
                                                   KullaniciAdi = yeni.Key,
                                                   ToplamSatisTutari = yeni.Sum(x => x.SiparistekiUrunler.Sum(a => a.Tutar))
                                               }
                                               ).ToList();

                rapor.UrunBasiSatislar = (from sd in db.SiparisDetaylari
                                          where DbFunctions.TruncateTime(sd.Siparis.Tarih).Value == secilenGun.Date
                                          group sd by sd.UrunID into yeni
                                          select new UrunBasiSatisViewModel()
                                          {
                                              UrunID = yeni.Key,
                                              UrunAdi = yeni.Max(x => x.Urun.UrunAdi),
                                              Adet = yeni.Sum(x => x.Miktar)
                                          }).ToList();
            }
            catch { }
            label3.Text = rapor.ToplamSatisTutari.ToString("C");
            label4.Text = rapor.ToplamSatilanUrunSayisi.ToString();
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            dataGridView1.DataSource = rapor.KullaniciBasiSatislar;
            dataGridView2.DataSource = rapor.UrunBasiSatislar;

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RaporGetir(dateTimePicker1.Value);
        }
    }
}
