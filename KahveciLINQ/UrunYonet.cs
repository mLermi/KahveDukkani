using DAL;
using DomainEntity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KahveciLINQ
{
    public partial class UrunYonet : Form
    {
        public UrunYonet()
        {
            InitializeComponent();
        }

        KahveContext db = new KahveContext();
        private void UrunYonet_Load(object sender, EventArgs e)
        {
            Yenile();
            numericUpDown1.DecimalPlaces = 2;
        }

        private void Yenile()
        {
            listBox1.DataSource = null;
            listBox1.DataSource = db.Urunler.OrderBy(x => x.UrunAdi).ToList();
            listBox1.DisplayMember = "UrunAdi";
        }

        private void button1_Click(object sender, EventArgs e)
        { //ekle
            if(button1.Text == "Ekle")
            {
                Urun yeniUrun = new Urun();
                yeniUrun.UrunAdi = textBox1.Text;
                yeniUrun.Fiyat = numericUpDown1.Value;
                db.Urunler.Add(yeniUrun);
                db.SaveChanges();
                Yenile();
                SiparisEkran s =(SiparisEkran) Application.OpenForms["SiparisEkran"];
                s.Yenile();
            } else
            {
                var duzenlenecek = db.Urunler.Find(secilen.UrunID);
                duzenlenecek.UrunAdi = textBox1.Text;
                duzenlenecek.Fiyat = numericUpDown1.Value;
                db.Entry(duzenlenecek).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                listBox1.Enabled = true;
                textBox1.Clear();
                numericUpDown1.Value = 0;
                button1.Text = "Ekle";
                Yenile();
                SiparisEkran s = (SiparisEkran)Application.OpenForms["SiparisEkran"];
                s.Yenile();
            }
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var index = listBox1.IndexFromPoint(e.Location);
                listBox1.SelectedIndex = index;
                if (index == -1) //birşey seçilmediyse
                {
                    contextMenuStrip1.Items[0].Enabled = false;
                    contextMenuStrip1.Items[1].Enabled = false;
                }
                else
                {
                    contextMenuStrip1.Items[0].Enabled = true;
                    contextMenuStrip1.Items[1].Enabled = true;
                }
            }
        }

        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Urun secilen = (Urun)listBox1.SelectedItem;
            DialogResult sonuc= MessageBox.Show(secilen.UrunAdi+" - silmek istediğinize emin misiniz?", "Ürün Sil", MessageBoxButtons.YesNo);
            if(sonuc == DialogResult.Yes)
            {
                db.Urunler.Remove(secilen);
                db.SaveChanges();
                Yenile();
                SiparisEkran s = (SiparisEkran)Application.OpenForms["SiparisEkran"];
                s.Yenile();
            }
        }
        Urun secilen;
        private void düzenleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            secilen = (Urun)listBox1.SelectedItem;
            textBox1.Text = secilen.UrunAdi;
            numericUpDown1.Value = secilen.Fiyat;
            button1.Text = "Düzenle";
            listBox1.Enabled = false;
        }
    }
}
