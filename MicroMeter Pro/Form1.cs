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
        private bool _blokadaZmian = false;
        private Color _defaultTextColor = Color.Black;
        private string _defaultLabelText = "-";

        public Form1()
        {
            InitializeComponent();
            tbox_waga.KeyPress += TylkoLiczbyIZnakDziesietny;
            tbox_wzrost.KeyPress += TylkoLiczbyIZnakDziesietny;
            tbox_wiek.KeyPress += TylkoLiczbyIZnakDziesietny;
            button_wyczysc.Click += button_wyczysc_Click;
            tbox_fat.KeyPress += TylkoLiczbyIZnakDziesietny;
            tbox_fat.TextChanged += (s, e) => UpdateAll();


            // Ustaw domyślne wartości
            cbox_wzor.SelectedIndex = 0;
            cbox_pal.SelectedIndex = 0;
            rb_utrzymanie.Checked = true;

            num_bialko.ValueChanged += (s, e) => ObliczMakroskladniki();
            num_tluszcze.ValueChanged += (s, e) => ObliczMakroskladniki();

            UpdateAll();
            UpdateKatchUI();

        }

        private void TylkoLiczbyIZnakDziesietny(object sender, KeyPressEventArgs e)
        {
            var tb = sender as Guna.UI2.WinForms.Guna2TextBox;
            if (tb == null)
                return;

            // Backspace, delete itp. - pozwól na te klawisze
            if (char.IsControl(e.KeyChar))
                return;

            // Cyfry - pozwól
            if (char.IsDigit(e.KeyChar))
                return;

            // Jeden separator dziesiętny (dla polskiej kultury)
            char decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if (e.KeyChar == decimalSeparator && !tb.Text.Contains(decimalSeparator))
            {
                return;
            }

            // Alternatywnie pozwól na kropkę jeśli kultura używa przecinka
            if (decimalSeparator == ',' && e.KeyChar == '.' && !tb.Text.Contains('.') && !tb.Text.Contains(','))
            {
                return;
            }

            // Reszta blokowana
            e.Handled = true;
        }

        private void ObliczBMI()
        {
            double waga = 0;
            double wzrostCm = 0;

            double.TryParse(tbox_waga.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out waga);

            double.TryParse(tbox_wzrost.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out wzrostCm);

            if (waga <= 0 || wzrostCm <= 0)
            {
                label16_BMI.Text = _defaultLabelText;
                label16_BMI.ForeColor = _defaultTextColor;
                return;
            }

            double wzrostM = wzrostCm / 100.0;
            double bmi = waga / (wzrostM * wzrostM);

            label16_BMI.Text = bmi.ToString("0.00");
            InterpretujBMI(bmi);
        }

        private void InterpretujBMI(double bmi)
        {
            if (bmi <= 0)
            {
                label16_BMI.Text = _defaultLabelText;
                label16_BMI.ForeColor = _defaultTextColor;
                return;
            }

            string bmiText = bmi.ToString("0.00");
            label16_BMI.Text = bmiText;

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

            bool wagaOk = double.TryParse(tbox_waga.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out waga);
            bool wzrostOk = double.TryParse(tbox_wzrost.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out wzrost);
            bool wiekOk = double.TryParse(tbox_wiek.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out wiek);

            if (!wagaOk || !wzrostOk || !wiekOk)
            {
                label16_ppm.Text = _defaultLabelText;
                return;
            }

            if (waga <= 0 || wzrost <= 0 || wiek <= 0)
            {
                label16_ppm.Text = _defaultLabelText;
                return;
            }

            // płeć
            bool isMale = rb_male.Checked;
            bool isFemale = rb_female.Checked;

            if (!isMale && !isFemale)
            {
                label16_ppm.Text = _defaultLabelText;
                return;
            }

            // wybrany wzór
            if (cbox_wzor.SelectedIndex < 0)
            {
                label16_ppm.Text = _defaultLabelText;
                return;
            }

            double ppm = 0;

            try
            {
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

                    case 2: // Katch–McArdle
                        {
                            if (!double.TryParse(tbox_fat.Text,
                                NumberStyles.Any,
                                CultureInfo.CurrentCulture,
                                out double fatPercent))
                            {
                                label16_ppm.Text = _defaultLabelText;
                                return;
                            }

                            if (fatPercent <= 0 || fatPercent >= 100)
                            {
                                label16_ppm.Text = _defaultLabelText;
                                return;
                            }

                            double ffm = waga * (1 - fatPercent / 100.0);
                            ppm = 370 + (21.6 * ffm);
                            break;
                        }

                    default:
                        label16_ppm.Text = _defaultLabelText;
                        return;
                }
            }
            catch (Exception)
            {
                label16_ppm.Text = _defaultLabelText;
                return;
            }

            label16_ppm.Text = Math.Round(ppm).ToString();
        }

        private void ObliczCPM()
        {
            // najpierw musimy mieć PPM
            if (!double.TryParse(label16_ppm.Text.Split(' ')[0], out double ppm) || ppm <= 0)
            {
                label16_cpm.Text = _defaultLabelText;
                label16_cpm.ForeColor = _defaultTextColor;
                return;
            }

            // PAL
            if (cbox_pal.SelectedIndex < 0)
            {
                label16_cpm.Text = _defaultLabelText;
                label16_cpm.ForeColor = _defaultTextColor;
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
            string cel = "";

            if (rb_redukcja.Checked)
            {
                korekta = -300;
                cel = "Redukcja";
            }
            else if (rb_tycie.Checked)
            {
                korekta = 300;
                cel = "Przyrost";
            }
            else if (rb_miesniowa.Checked)
            {
                korekta = 500;
                cel = "Masa mięśniowa";
            }
            else if (rb_utrzymanie.Checked)
            {
                korekta = 0;
                cel = "Utrzymanie";
            }
            else
            {
                label16_cpm.Text = _defaultLabelText;
                label16_cpm.ForeColor = _defaultTextColor;
                return;
            }

            double cpm = ppm * pal + korekta;

            // Interpretuj CPM
            InterpretujCPM(cpm, cel);

            // Oblicz makroskładniki
            ObliczMakroskladniki();
        }

        private void InterpretujCPM(double cpm, string cel)
        {
            string cpmText = Math.Round(cpm).ToString();

            switch (cel)
            {
                case "Redukcja":
                    label16_cpm.ForeColor = Color.OrangeRed;
                    label16_cpm.Text = $"{cpmText}  (Redukcja -300 kcal)";
                    break;
                case "Przyrost":
                    label16_cpm.ForeColor = Color.DodgerBlue;
                    label16_cpm.Text = $"{cpmText}  (Przyrost +300 kcal)";
                    break;
                case "Masa mięśniowa":
                    label16_cpm.ForeColor = Color.MediumPurple;
                    label16_cpm.Text = $"{cpmText}  (Masa mięśniowa +500 kcal)";
                    break;
                case "Utrzymanie":
                    label16_cpm.ForeColor = Color.LimeGreen;
                    label16_cpm.Text = $"{cpmText}  (Utrzymanie)";
                    break;
            }

            // Dodatkowe ostrzeżenia
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

        private void ObliczMakroskladniki()
        {
            if (_blokadaZmian) return;
            _blokadaZmian = true;

            try
            {
                // Pobieramy CPM (pierwszą część przed spacją)
                string cpmText = label16_cpm.Text.Split(' ')[0];
                if (!double.TryParse(cpmText, out double cpm) || cpm <= 0)
                {
                    tb_bialko.Text = "0g (0%)";
                    tb_tluszcze.Text = "0g (0%)";
                    tb_wegle.Text = "0g (0%)";
                    return;
                }

                // Pobieramy procenty białka i tłuszczu
                double procentBialka = (double)num_bialko.Value;
                double procentTluszczow = (double)num_tluszcze.Value;

                // Sprawdź, czy suma nie przekracza 100%
                if (procentBialka + procentTluszczow >= 100)
                {
                    procentTluszczow = 100 - procentBialka;
                    num_tluszcze.Value = (decimal)procentTluszczow;
                }

                // Automatyczne wyliczenie węglowodanów
                double procentWeglowodanow = 100 - (procentBialka + procentTluszczow);

                // Ustawiamy wartość w kontrolce
                num_wegle.Value = (decimal)Math.Max(0, procentWeglowodanow);

                // Przeliczenia kalorii
                double kalorieBialko = cpm * (procentBialka / 100.0);
                double kalorieTluszcze = cpm * (procentTluszczow / 100.0);
                double kalorieWeglowodany = cpm * (procentWeglowodanow / 100.0);

                // Przeliczenia gramów
                double gramyBialko = kalorieBialko / 4.0;
                double gramyTluszcze = kalorieTluszcze / 9.0;
                double gramyWeglowodany = kalorieWeglowodany / 4.0;

                // Wyświetlanie wyników
                tb_bialko.Text = $"{Math.Round(gramyBialko)}g ({procentBialka}%)";
                tb_tluszcze.Text = $"{Math.Round(gramyTluszcze)}g ({procentTluszczow}%)";
                tb_wegle.Text = $"{Math.Round(gramyWeglowodany)}g ({procentWeglowodanow}%)";

                tb_bialko.ForeColor = Color.OrangeRed;
                tb_tluszcze.ForeColor = Color.Gold;
                tb_wegle.ForeColor = Color.LimeGreen;
            }
            catch (Exception)
            {
                // W przypadku błędu ustaw domyślne wartości
                tb_bialko.Text = "0g (0%)";
                tb_tluszcze.Text = "0g (0%)";
                tb_wegle.Text = "0g (0%)";
            }
            finally
            {
                _blokadaZmian = false;
            }
        }

        private void UpdateKatchUI()
        {
            bool isKatch = cbox_wzor.SelectedIndex == 2;
            tbox_fat.Enabled = isKatch;

            if (!isKatch)
                tbox_fat.Text = string.Empty;
        }


        private void button_wyczysc_Click(object sender, EventArgs e)
        {
            // Czyszczenie wszystkich pól tekstowych
            tbox_waga.Clear();
            tbox_wzrost.Clear();
            tbox_wiek.Clear();
            tbox_fat.Clear();
            tbox_fat.Enabled = false;

            // Resetowanie radio buttonów
            rb_male.Checked = false;
            rb_female.Checked = false;
            rb_utrzymanie.Checked = true; // Ustaw domyślny cel
            rb_redukcja.Checked = false;
            rb_tycie.Checked = false;
            rb_miesniowa.Checked = false;

            // Resetowanie comboboxów
            cbox_wzor.SelectedIndex = 0;
            cbox_pal.SelectedIndex = 0;

            // Resetowanie wyników
            label16_ppm.Text = _defaultLabelText;
            label16_ppm.ForeColor = _defaultTextColor;
            label16_cpm.Text = _defaultLabelText;
            label16_cpm.ForeColor = _defaultTextColor;
            label16_BMI.Text = _defaultLabelText;
            label16_BMI.ForeColor = _defaultTextColor;

            // Resetowanie makroskładników
            tb_bialko.Text = "0g (0%)";
            tb_bialko.ForeColor = _defaultTextColor;
            tb_tluszcze.Text = "0g (0%)";
            tb_tluszcze.ForeColor = _defaultTextColor;
            tb_wegle.Text = "0g (0%)";
            tb_wegle.ForeColor = _defaultTextColor;

            // Resetowanie procentów makroskładników
            num_bialko.Value = 25;
            num_tluszcze.Value = 26;
            num_wegle.Value = 49;
        }

        private void UpdateAll()
        {
            ObliczBMI();
            ObliczPPM();
            ObliczCPM();
            // ObliczMakroskladniki() jest już wywoływane w ObliczCPM()
        }

        #region Event Handlers
        private void tbox_waga_TextChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void tbox_wzrost_TextChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void tbox_wiek_TextChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void rb_male_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_male.Checked)
                UpdateAll();
        }

        private void rb_female_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_female.Checked)
                UpdateAll();
        }

        private void cbox_wzor_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateKatchUI();
            UpdateAll();
        }

        private void cbox_pal_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void rb_utrzymanie_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_utrzymanie.Checked)
                UpdateAll();
        }

        private void rb_redukcja_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_redukcja.Checked)
                UpdateAll();
        }

        private void rb_tycie_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_tycie.Checked)
                UpdateAll();
        }

        private void rb_miesniowa_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_miesniowa.Checked)
                UpdateAll();
        }
        #endregion
    }
}