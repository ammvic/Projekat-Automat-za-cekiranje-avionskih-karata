using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;

namespace ProjekatOOP2
{
    public class BazaDestinacije
    {
        public OleDbConnection connection = new OleDbConnection(); // veza sa bazom
        public OleDbCommand command; // sql naredbe koje ce se izvrsavati nad bazom

        public BazaDestinacije()
        {
            connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0; Data source=destinacije.accdb; Persist security info=false";
            command = connection.CreateCommand();
        }

        public ComboBox CitanjePodataka()
        {
            ComboBox cb = new ComboBox();
            try
            {
                connection.Open();
                command.CommandText = "SELECT NazivDestinacije FROM destinacije";
                OleDbDataReader reader = command.ExecuteReader(); //izvrsavanje sql upita i citanje podataka iz baze
                while (reader.Read())
                {
                    cb.Items.Add(reader["NazivDestinacije"].ToString());
                }
                connection.Close();
            }
            catch (OleDbException ex)
            {
                if (connection != null)
                {
                    connection.Close();
                    MessageBox.Show(ex.Message);
                }
            }
            return cb;
        }

        public double DohvatiOsnovnuCenu(string odabranaDestinacija)
        {
            double osnovnaCena = 0.0;

            try
            {
                connection.Open();
                command.CommandText = $"SELECT Cena FROM destinacije WHERE NazivDestinacije = '{odabranaDestinacija}'";
                object result = command.ExecuteScalar(); // izvrsavanje sql upita i cuvanje procitanih podataka

                if (result != null && double.TryParse(result.ToString(), out osnovnaCena))
                {
                    connection.Close();
                    return osnovnaCena;
                }
                else
                {

                    connection.Close();
                    return 0.0;
                }
            }
            catch (OleDbException ex)
            {
                if (connection != null)
                {
                    connection.Close();
                    MessageBox.Show(ex.Message);
                }

                return 0.0;
            }
        }


    }
}

