using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;

namespace ProjekatOOP2
{
    public class BazaRezervacija
    {
        private readonly string connectionString;
        private readonly BazaDestinacije bazaDestinacije; // BazaDestinacije kao član klase
        private int brojIzboraZaKolonu = 0;
        public BazaRezervacija(string dbPath, BazaDestinacije bazaDestinacije) //  BazaDestinacije kao argument konstruktoru
        {
            connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data source={dbPath}; Persist security info=false";
            this.bazaDestinacije = bazaDestinacije; // Postavite član klase
        }

        private OleDbConnection OpenConnection()
        {
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            return connection;
        }

        private void CloseConnection(OleDbConnection connection)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public bool RezervisiSediste(string ime, string prezime, string destinacija, DateTime datumLeta, string odabranomesto, string odabranaKolona)
        {
            using (OleDbConnection connection = OpenConnection())
            {
                try
                {
                    if (ProveriKorisnikaNaDatumu(ime, prezime, destinacija, datumLeta, connection))
                    {
                        MessageBox.Show("Već ste rezervisali let za odabrani datum.");
                        return false;
                    }

                    using (OleDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO rezervacije (ime, prezime, destinacija, datumLeta, odabranomesto, odabranaKolona) " +
                            "VALUES (?, ?, ?, ?, ?, ?)";
                        command.Parameters.AddWithValue("?", ime);
                        command.Parameters.AddWithValue("?", prezime);
                        command.Parameters.AddWithValue("?", destinacija);
                        command.Parameters.AddWithValue("?", datumLeta);
                        command.Parameters.AddWithValue("?", odabranomesto);
                        command.Parameters.AddWithValue("?", odabranaKolona);

                        int affectedRows = command.ExecuteNonQuery();

                        return affectedRows > 0;
                    }
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show($"Greška prilikom rezervacije: {ex.Message}");
                    return false;
                }
                finally
                {
                    CloseConnection(connection);
                }
            }
        }

        public bool ProveriKorisnikaNaDatumu(string ime, string prezime, string destinacija, DateTime datumLeta, OleDbConnection connection)
        {
            try
            {
                using (OleDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM rezervacije WHERE ime = ? AND prezime = ? " +
                        "AND destinacija = ? AND datumLeta = ? AND odabranaKolona IS NOT NULL";

                    command.Parameters.AddWithValue("?", ime);
                    command.Parameters.AddWithValue("?", prezime);
                    command.Parameters.AddWithValue("?", destinacija);
                    command.Parameters.AddWithValue("?", datumLeta);

                    int brojRezervacija = Convert.ToInt32(command.ExecuteScalar());

                    return brojRezervacija > 0;
                }
            }
            catch (OleDbException ex)
            {
                MessageBox.Show($"Greška prilikom provjere korisnika: {ex.Message}");
                return false;
            }
        }

        public int PronadjiPrviSlobodanRed(string odabranaKolona, string destinacija, DateTime datumLeta)
        {
            using (OleDbConnection connection = OpenConnection())
            {
                try
                {
                    if (string.IsNullOrEmpty(odabranaKolona))
                    {
                        MessageBox.Show("Molimo odaberite kolonu.");
                        return -1;
                    }

                    if (brojIzboraZaKolonu >= 30)
                    {
                        MessageBox.Show("Kapacitet u toj koloni za taj datum je popunjen.");
                        return -1;
                    }
                    else
                    {
                        if (brojIzboraZaKolonu < 30)
                        {
                            brojIzboraZaKolonu++;
                        }

                        return brojIzboraZaKolonu;
                    }
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show($"Greška prilikom pronalaženja prvog slobodnog reda: {ex.Message}");
                    return -1;
                }
            }
        }
    


}
}
