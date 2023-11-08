using System;
using System.Data.OleDb;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProjekatOOP2
{
    public partial class MainWindow : Window
    {
        private BazaDestinacije bazaDestinacije;
        private BazaRezervacija bazaRezervacija;
        private string odabranomesto = null;
        private string odabranaKolona = null;
        private OleDbConnection connection;

        public MainWindow()
        {
            InitializeComponent();
            bazaDestinacije = new BazaDestinacije();
            cmbDestinacije.ItemsSource = bazaDestinacije.CitanjePodataka().Items;
            string dbPath = "C:\\Users\\Laptop\\Desktop\\ProjekatOOP2\\ProjekatOOP2\\bin\\Debug\\rezervacije.accdb";
            bazaRezervacija = new BazaRezervacija(dbPath, bazaDestinacije); // bazaDestinacije kao argument konstruktoru
            datumLetaPicker.SelectedDateChanged += DatumLetaPicker_SelectedDateChanged;

            connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\Laptop\\Desktop\\ProjekatOOP2\\ProjekatOOP2\\bin\\Debug\\rezervacije.accdb;");
            connection.Open(); // Otvorite vezu sa bazom
        }

        private string GenerateInvoiceContent(string firstName, string lastName, string number, string odabranaDestinacija, string odabranomesto, double trenutnaCena, DateTime odabraniDatum)
        {
            StringBuilder contentBuilder = new StringBuilder();
            contentBuilder.AppendLine($"Poštovani {firstName} {lastName},");
            contentBuilder.AppendLine($"Broj pasoša: {number}");
            contentBuilder.AppendLine($"Izabrali ste destinaciju: {odabranaDestinacija}");
            contentBuilder.AppendLine($"Mesto u avionu: {odabranomesto}");
            contentBuilder.AppendLine($"Datum leta: {odabraniDatum.ToShortDateString()}");
            contentBuilder.AppendLine($"Cena: {trenutnaCena} EUR");
            contentBuilder.AppendLine();
            contentBuilder.AppendLine("Hvala Vam na izboru!");
            return contentBuilder.ToString();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ime.Text) || string.IsNullOrWhiteSpace(prezime.Text) || string.IsNullOrWhiteSpace(brojpasosa.Text) ||
                    cmbDestinacije.SelectedItem == null || string.IsNullOrEmpty(cmbDestinacije.SelectedItem.ToString()) || !datumLetaPicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Niste uneli sve potrebne podatke za izradu računa ili niste izabrali kolonu.");
                    return;
                }

                DateTime? odabraniDatum = datumLetaPicker.SelectedDate;

                if (odabraniDatum == null)
                {
                    MessageBox.Show("Morate odabrati datum leta.");
                    return;
                }

                //  da li je korisnik već rezervisao let za odabrani datum
                if (bazaRezervacija.ProveriKorisnikaNaDatumu(ime.Text, prezime.Text, cmbDestinacije.SelectedItem.ToString(), odabraniDatum.Value, connection))
                {
                    MessageBox.Show("Već ste rezervisali let za odabrani datum.");
                    return;
                }


                if (bazaRezervacija.RezervisiSediste(ime.Text, prezime.Text, cmbDestinacije.SelectedItem.ToString(), odabraniDatum.Value, odabranomesto, odabranaKolona))
                {
                    double trenutnaCena = bazaDestinacije.DohvatiOsnovnuCenu(cmbDestinacije.SelectedItem.ToString());

                    if (chkPrioritetnoUkrcavanje.IsChecked == true)
                    {
                        trenutnaCena += 50.0; // Primer doplate za prioritetno ukrcavanje
                    }
                    if (chkDodatniPrtljag.IsChecked == true)
                    {
                        trenutnaCena += 30.0; // Primer doplate za dodatni prtljag
                    }

                    string invoiceContent = GenerateInvoiceContent(ime.Text, prezime.Text, brojpasosa.Text, cmbDestinacije.SelectedItem.ToString(), odabranomesto, trenutnaCena, odabraniDatum.Value);

                    // PrintDialog za štampanje sadržaja
                    PrintDialog printDialog = new PrintDialog();
                    if (printDialog.ShowDialog() == true)
                    {
                        FlowDocument flowDocument = new FlowDocument(new Paragraph(new Run(invoiceContent)));
                        IDocumentPaginatorSource documentPaginatorSource = flowDocument;
                        printDialog.PrintDocument(documentPaginatorSource.DocumentPaginator, "Račun");
                    }

                    MessageBox.Show("Račun je uspešno odštampan!");

                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Rezervacija nije uspela ili već ste rezervisali let za odabrani datum.");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom štampanja računa: {ex.Message}");
            }
        }


        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (Sms.IsChecked == true)
            {
                string phoneNumber = phonenumber.Text;

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    MessageBox.Show("Unesite broj telefona.");
                    return;
                }
                DateTime odabraniDatum = datumLetaPicker.SelectedDate.Value;
                double trenutnaCena = bazaDestinacije.DohvatiOsnovnuCenu(cmbDestinacije.SelectedItem.ToString());
                string invoiceContent = GenerateInvoiceContent(ime.Text, prezime.Text, brojpasosa.Text, cmbDestinacije.SelectedItem.ToString(), odabranomesto, trenutnaCena, odabraniDatum);
                // Simulacija slanja SMS poruke

                MessageBox.Show("Račun je uspešno poslat putem SMS-a.");
            }
            else if (email.IsChecked == true)
            {
                string emailAddress = Email.Text;

                if (string.IsNullOrEmpty(emailAddress))
                {
                    MessageBox.Show("Unesite email adresu.");
                    return;
                }
                double trenutnaCena = bazaDestinacije.DohvatiOsnovnuCenu(cmbDestinacije.SelectedItem.ToString());
                DateTime odabraniDatum = datumLetaPicker.SelectedDate.Value;
                string invoiceContent = GenerateInvoiceContent(ime.Text, prezime.Text, brojpasosa.Text, cmbDestinacije.SelectedItem.ToString(), odabranomesto, trenutnaCena, odabraniDatum);

                // Simulacija slanja emaila

                MessageBox.Show("Račun je uspešno poslat putem emaila.");
            }
            else
            {
                MessageBox.Show("Izaberite način slanja.");
            }
        }

        private void ResetForm()
        {
            ime.Clear();
            prezime.Clear();
            brojpasosa.Clear();
            cmbDestinacije.SelectedItem = null;
            odabranaKolona = null;
            phonenumber.Clear();
            datumLetaPicker.SelectedDate = null;
            Email.Clear();
            email.IsChecked = false;
            Sms.IsChecked = false;
            chkPrioritetnoUkrcavanje.IsChecked = false;
            chkDodatniPrtljag.IsChecked = false;
        }

        private void Sms_Checked(object sender, RoutedEventArgs e)
        {
            if (email.IsChecked == true)
            {
                email.IsChecked = false;
            }
        }

        private void Email_Checked(object sender, RoutedEventArgs e)
        {
            if (Sms.IsChecked == true)
            {
                Sms.IsChecked = false;
            }
        }

        private void DatumLetaPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datumLetaPicker != null && datumLetaPicker.SelectedDate.HasValue)
            {
                DateTime odabraniDatum = datumLetaPicker.SelectedDate.Value;

                if (odabraniDatum < DateTime.Today)
                {
                    MessageBox.Show("Ne možete izabrati datum u prošlosti.");
                    datumLetaPicker.SelectedDate = null;
                    return;
                }

                // Provera da li su uneti ime, prezime i odabrana destinacija
                if (string.IsNullOrWhiteSpace(ime.Text) || string.IsNullOrWhiteSpace(prezime.Text) || string.IsNullOrEmpty(cmbDestinacije.SelectedItem?.ToString()))
                {
                    MessageBox.Show("Morate uneti ime, prezime i odabrati destinaciju pre nego što odaberete datum leta.");
                    datumLetaPicker.SelectedDate = null;
                    return;
                }

                // Provera da li je korisnik već rezervisao let za odabrani datum
                if (bazaRezervacija.ProveriKorisnikaNaDatumu(ime.Text, prezime.Text, cmbDestinacije.SelectedItem.ToString(), odabraniDatum, connection))
                {
                    MessageBox.Show("Već ste rezervisali let za odabrani datum.");
                    datumLetaPicker.SelectedDate = null;
                    return;
                }
            }

        }




        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
               
                string selectedSeat = clickedButton.Content.ToString();

                if (datumLetaPicker.SelectedDate.HasValue)
                {
                    DateTime odabraniDatum = datumLetaPicker.SelectedDate.Value;

                    // Provera da li je već odabrano mesto za tu kolonu i datum
                    if (bazaRezervacija.ProveriKorisnikaNaDatumu(ime.Text, prezime.Text, cmbDestinacije.SelectedItem.ToString(), odabraniDatum, connection))
                    {
                        MessageBox.Show("Već ste odabrali mesto za tu kolonu i datum.");
                        return;
                    }


                    int prvoSlobodnoMesto = bazaRezervacija.PronadjiPrviSlobodanRed(selectedSeat, cmbDestinacije.SelectedItem.ToString(), odabraniDatum);

                    if (prvoSlobodnoMesto != -1)
                    {
                        
                        odabranomesto = $"{selectedSeat}-{prvoSlobodnoMesto}";
                        odabranaKolona = selectedSeat;

                        
                        MessageBox.Show($"Dodeljeno vam je mesto {odabranomesto} u koloni {odabranaKolona}.");
                    }
                }
                else
                {
                    MessageBox.Show("Morate odabrati datum leta pre nego što odaberete mesto.");
                }
            }

        }

        

    }


}
