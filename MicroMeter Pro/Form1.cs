using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace MicroMeter_Pro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tbox_waga.KeyPress += TylkoLiczbyIZnakDziesietny;
            tbox_wzrost.KeyPress += TylkoLiczbyIZnakDziesietny;
            button_wyczysc.Click += button_wyczysc_Click;
            UpdateAll();
        }

        private void guna2CustomRadioButton4_CheckedChanged(object sender, EventArgs e)
        {
            ObliczCPM();
        }

        private void ObliczBMI()
        {
            double waga = 0;
            double wzrostCm = 0;
            double wiek = 0;

            double.TryParse(tbox_waga.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out waga);

            double.TryParse(tbox_wzrost.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out wzrostCm);

            double.TryParse(tbox_wiek.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out wiek);

            if (waga <= 0 || wzrostCm <= 0)
            {
                label16_BMI.Text = "0";
                return;
            }

            double wzrostM = wzrostCm / 100.0;
            double bmi = waga / (wzrostM * wzrostM);

            label16_BMI.Text = bmi.ToString("0.00");
            InterpretujBMI(bmi);
        }

        private void TylkoLiczbyIZnakDziesietny(object sender, KeyPressEventArgs e)
        {
            var tb = sender as Guna.UI2.WinForms.Guna2TextBox;
            if (tb == null)
                return;

            // Backspace, delete itp.
            if (char.IsControl(e.KeyChar))
                return;

            // Cyfry
            if (char.IsDigit(e.KeyChar))
                return;

            // Jeden separator dziesiętny
            if ((e.KeyChar == ',' || e.KeyChar == '.') &&
                !tb.Text.Contains(",") &&
                !tb.Text.Contains("."))
            {
                return;
            }

            // Reszta blokowana
            e.Handled = true;
        }

        private void tbox_waga_TextChanged(object sender, EventArgs e)
        {
            ObliczBMI();
            ObliczPPM();
        }

        private void tbox_wzrost_TextChanged(object sender, EventArgs e)
        {
            ObliczBMI();
            ObliczPPM();
        }

        private void tbox_wiek_TextChanged(object sender, EventArgs e)
        {
            ObliczBMI();
            ObliczPPM();
        }

        private void InterpretujBMI(double bmi)
        {
            if (bmi <= 0)
            {
                label16_BMI.ForeColor = Color.Black;
                label16_BMI.Text = "0";
                return;
            }

            if (bmi < 18.5)
            {
                label16_BMI.ForeColor = Color.DeepSkyBlue;
                label16_BMI.Text += "  (Niedowaga)";
            }
            else if (bmi < 25)
            {
                label16_BMI.ForeColor = Color.LimeGreen;
                label16_BMI.Text += "  (Norma)";
            }
            else if (bmi < 30)
            {
                label16_BMI.ForeColor = Color.Gold;
                label16_BMI.Text += "  (Nadwaga)";
            }
            else if (bmi < 35)
            {
                label16_BMI.ForeColor = Color.Orange;
                label16_BMI.Text += "  (Otyłość I)";
            }
            else if (bmi < 40)
            {
                label16_BMI.ForeColor = Color.OrangeRed;
                label16_BMI.Text += "  (Otyłość II)";
            }
            else
            {
                label16_BMI.ForeColor = Color.Red;
                label16_BMI.Text += "  (Otyłość III)";
            }
        }

        private void ObliczPPM()
        {
            // parsowanie danych
            double waga, wzrost, wiek;

            if (!double.TryParse(tbox_waga.Text, out waga) ||
                !double.TryParse(tbox_wzrost.Text, out wzrost) ||
                !double.TryParse(tbox_wiek.Text, out wiek))
            {
                label16_ppm.Text = "-";
                return;
            }

            if (waga <= 0 || wzrost <= 0 || wiek <= 0)
            {
                label16_ppm.Text = "-";
                return;
            }

            // płeć
            bool isMale = rb_male.Checked;
            bool isFemale = rb_female.Checked;

            if (!isMale && !isFemale)
            {
                label16_ppm.Text = "-";
                return;
            }

            // wybrany wzór
            if (cbox_wzor.SelectedIndex < 0)
            {
                label16_ppm.Text = "-";
                return;
            }

            double ppm = 0;

            switch (cbox_wzor.SelectedIndex)
            {
                case 0: // Mifflin–St Jeor
                    ppm = isMale
                        ? (10 * waga + 6.25 * wzrost - 5 * wiek + 5)
                        : (10 * waga + 6.25 * wzrost - 5 * wiek - 161);
                    break;

                case 1: // Harris–Benedict
                    ppm = isMale
                        ? (88.362 + 13.397 * waga + 4.799 * wzrost - 5.677 * wiek)
                        : (447.593 + 9.247 * waga + 3.098 * wzrost - 4.330 * wiek);
                    break;

                case 2: // Katch–McArdle (wymaga % tkanki tłuszczowej - nie zaimplementowane)
                    label16_ppm.Text = "Wzór niedostępny";
                    return;

                default:
                    label16_ppm.Text = "-";
                    return;
            }

            label16_ppm.Text = Math.Round(ppm).ToString();
        }

        private void rb_male_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void rb_female_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void cbox_wzor_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void ObliczCPM()
        {
            // najpierw musimy mieć PPM
            if (!double.TryParse(label16_ppm.Text, out double ppm) || ppm <= 0)
            {
                label16_cpm.Text = "-";
                label16_cpm.ForeColor = Color.Black;
                return;
            }

            // PAL
            if (cbox_pal.SelectedIndex < 0)
            {
                label16_cpm.Text = "-";
                label16_cpm.ForeColor = Color.Black;
                return;
            }

            double pal = 1.2;

            switch (cbox_pal.SelectedIndex)
            {
                case 0: pal = 1.2; break;
                case 1: pal = 1.4; break;
                case 2: pal = 1.6; break;
                case 3: pal = 1.8; break;
                case 4: pal = 2.0; break;
            }

            // cel
            int korekta = 0;

            if (rb_redukcja.Checked)
                korekta = -300;
            else if (rb_tycie.Checked)
                korekta = 300;
            else if (rb_miesniowa.Checked)
                korekta = 500;
            else if (rb_utrzymanie.Checked)
                korekta = 0;
            else
            {
                label16_cpm.Text = "-";
                label16_cpm.ForeColor = Color.Black;
                return;
            }

            double cpm = ppm * pal + korekta;
            label16_cpm.Text = Math.Round(cpm).ToString();

            // INTERPRETACJA CPM
            InterpretujCPM(cpm, ppm, pal, korekta);
        }

        private void InterpretujCPM(double cpm, double ppm, double pal, int korekta)
        {
            // Podstawowa kolorystyka zależna od celu
            if (rb_redukcja.Checked)
            {
                label16_cpm.ForeColor = Color.OrangeRed;
                label16_cpm.Text += "  (Redukcja -300 kcal)";
            }
            else if (rb_tycie.Checked)
            {
                label16_cpm.ForeColor = Color.DodgerBlue;
                label16_cpm.Text += "  (Przyrost +300 kcal)";
            }
            else if (rb_miesniowa.Checked)
            {
                label16_cpm.ForeColor = Color.MediumPurple;
                label16_cpm.Text += "  (Masa mięśniowa +500 kcal)";
            }
            else if (rb_utrzymanie.Checked)
            {
                label16_cpm.ForeColor = Color.LimeGreen;
                label16_cpm.Text += "  (Utrzymanie)";
            }

            // Dodatkowe informacje w zależności od wartości CPM
            if (cpm < 1200)
            {
                MessageBox.Show(
                    "UWAGA: CPM poniżej 1200 kcal jest bardzo niskie!\n" +
                    "Może być szkodliwe dla zdrowia. Skonsultuj się z dietetykiem.",
                    "Ostrzeżenie",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else if (cpm > 4000)
            {
                MessageBox.Show(
                    "CPM przekracza 4000 kcal - bardzo wysokie zapotrzebowanie!\n" +
                    "Upewnij się, że dane są poprawne.",
                    "Informacja",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void cbox_pal_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void rb_utrzymanie_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void rb_redukcja_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void rb_tycie_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void rb_miesniowa_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void button_wyczysc_Click(object sender, EventArgs e)
        {
            // Czyszczenie wszystkich pól tekstowych
            tbox_waga.Clear();
            tbox_wzrost.Clear();
            tbox_wiek.Clear();

            // Resetowanie radio buttonów
            rb_male.Checked = false;
            rb_female.Checked = false;
            rb_utrzymanie.Checked = false;
            rb_redukcja.Checked = false;
            rb_tycie.Checked = false;
            rb_miesniowa.Checked = false;

            // Resetowanie comboboxów
            cbox_wzor.SelectedIndex = 0;
            cbox_pal.SelectedIndex = 0;

            // Resetowanie wyników
            label16_ppm.Text = "-";
            label16_ppm.ForeColor = Color.Black;
            label16_cpm.Text = "-";
            label16_cpm.ForeColor = Color.Black;
            label16_BMI.Text = "-";
            label16_BMI.ForeColor = Color.Black;

            // Resetowanie makroskładników
            tb_bialko.Text = "0g (0%)";
            tb_bialko.ForeColor = Color.Black;
            tb_tluszcze.Text = "0g (0%)";
            tb_tluszcze.ForeColor = Color.Black;
            tb_wegle.Text = "0g (0%)";
            tb_wegle.ForeColor = Color.Black;
        }

        private void ObliczMakroskladniki()
        {
            // Pobieramy CPM
            if (!double.TryParse(label16_cpm.Text.Split(' ')[0], out double cpm) || cpm <= 0)
            {
                tb_bialko.Text = "0g (0%)";
                tb_tluszcze.Text = "0g (0%)";
                tb_wegle.Text = "0g (0%)";
                return;
            }

            // Pobieramy procenty z kontrolek (np. NumericUpDown)
            double procentBialka = (double)num_bialko.Value;
            double procentTluszczow = (double)num_tluszcze.Value;
            double procentWeglowodanow = (double)num_wegle.Value;

            // --- WALIDACJA ZAKRESÓW ---
            if (procentBialka < 15 || procentBialka > 35)
            {
                MessageBox.Show("Białko musi być w zakresie 15–35%.");
                return;
            }

            if (procentTluszczow < 20 || procentTluszczow > 35)
            {
                MessageBox.Show("Tłuszcze muszą być w zakresie 20–35%.");
                return;
            }

            if (procentWeglowodanow < 35 || procentWeglowodanow > 60)
            {
                MessageBox.Show("Węglowodany muszą być w zakresie 35–60%.");
                return;
            }

            // --- SUMA MUSI WYNOSIĆ 100% ---
            double suma = procentBialka + procentTluszczow + procentWeglowodanow;

            if (Math.Abs(suma - 100) > 0.1)
            {
                MessageBox.Show("Suma makroskładników musi wynosić dokładnie 100%.");
                return;
            }

            // --- PRZELICZENIA ---
            double kalorieBialko = cpm * (procentBialka / 100.0);
            double kalorieTluszcze = cpm * (procentTluszczow / 100.0);
            double kalorieWeglowodany = cpm * (procentWeglowodanow / 100.0);

            double gramyBialko = kalorieBialko / 4.0;
            double gramyTluszcze = kalorieTluszcze / 9.0;
            double gramyWeglowodany = kalorieWeglowodany / 4.0;

            // --- WYNIKI ---
            tb_bialko.Text = $"{Math.Round(gramyBialko)}g ({procentBialka}%)";
            tb_tluszcze.Text = $"{Math.Round(gramyTluszcze)}g ({procentTluszczow}%)";
            tb_wegle.Text = $"{Math.Round(gramyWeglowodany)}g ({procentWeglowodanow}%)";

            tb_bialko.ForeColor = Color.OrangeRed;
            tb_tluszcze.ForeColor = Color.Gold;
            tb_wegle.ForeColor = Color.LimeGreen;
        }

        private void UpdateAll()
        {
            ObliczBMI();
            ObliczPPM();
            ObliczCPM();
            ObliczMakroskladniki();
        }
    }
}