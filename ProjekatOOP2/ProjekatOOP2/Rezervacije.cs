using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatOOP2
{
    public class Rezervacije
    {
        private string Ime, Prezime, Destinacija, Odabranomesto, OdabranaKolona;
        public string ime
        {
            get { return Ime; }
            set
            {
                Ime = value;
            }
        }
        public string prezime
        {
            get { return Prezime; }
            set
            {
                Prezime = value;
            }
        }
        public string destinacija
        {
            get { return Destinacija; }
            set
            {
                Destinacija = value;
            }
        }
        public string odabranomesto
        {
            get { return Odabranomesto; }
            set
            {
                Odabranomesto = value;
            }
        }
        public string odabranaKolona
        {
            get { return OdabranaKolona; }
            set
            {
                OdabranaKolona = value;
            }
        }
    }
}
