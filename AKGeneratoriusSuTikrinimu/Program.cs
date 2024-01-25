using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

class Asmenys
{
    public string vardas { get; set; }  // Vardas
    public string pavarde { get; set; } // Pavarde
    public string lytis { get; set; }   //Lytis
    public int metai { get; set; }  // Gimimo metai
    public int menuo { get; set; }  // Gimimo menuo
    public int diena { get; set; }  //Gimimo diena
    public string asmens_kodas { get; set; }    // Asmens kodas
}

class Program
{
    static void Main(string[] args)
    {
        // Paimame duomenis iš json failo
        string jsonContent = File.ReadAllText("C:\\Users\\justi\\OneDrive\\Stalinis kompiuteris\\CSharp\\AKGeneratoriusSuTikrinimu\\Duomenys.json");
        List<Asmenys> json = JsonConvert.DeserializeObject<List<Asmenys>>(jsonContent);

        // Atskiriam validius ir nevalidius duomenys
        List<Asmenys> validusDuomenys = new List<Asmenys>();
        List<Asmenys> nevalidusDuomenys = new List<Asmenys>();

        foreach (var asmuo in json)
        {
            if (PatikrintDuomenis(asmuo))
            {
                validusDuomenys.Add(asmuo);
            }
            else
            {
                nevalidusDuomenys.Add(asmuo);
            }
        }

        // Tikriname ar yra validžių duomenų
        if (validusDuomenys.Count > 0)
        {
            // Generuojame asmens kodus tik su validžiais duomenimis
            SukurtiAsmensKodus(validusDuomenys);

            // Išspausdiname rezultatus į konsole
            Console.WriteLine("Teisingi duomenys ir sukurtas asmens kodas:");
            Console.WriteLine("");
            foreach (var asmuo in validusDuomenys)
            {
                Console.WriteLine(asmuo.vardas + " " + asmuo.pavarde);
                Console.WriteLine(asmuo.lytis);
                Console.WriteLine($"{asmuo.metai}-{asmuo.menuo:D2}-{asmuo.diena:00}");
                Console.WriteLine(asmuo.asmens_kodas);
                Console.WriteLine("");
            }

            // Įrašome validžius duomenis į naują failą
            string outputJson = JsonConvert.SerializeObject(validusDuomenys, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("C:\\Users\\justi\\OneDrive\\Stalinis kompiuteris\\CSharp\\AKGeneratoriusSuTikrinimu\\Rezultatai.json", outputJson);
        }

        // Spausdiname nevalidžius duomenis į konsole
        Console.WriteLine("Neteisingi duomenys:");
        Console.WriteLine("");
        foreach (var asmuo in nevalidusDuomenys)
        {
            Console.WriteLine(asmuo.vardas + " " + asmuo.pavarde);
            Console.WriteLine(asmuo.lytis);
            Console.WriteLine($"{asmuo.metai}-{asmuo.menuo:D2}-{asmuo.diena:00}");
            Console.WriteLine("");
        }

        // Išsaugome nevalidžius duomenis į naują failą
        string nevalidusJson = JsonConvert.SerializeObject(nevalidusDuomenys, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText("C:\\Users\\justi\\OneDrive\\Stalinis kompiuteris\\CSharp\\AKGeneratoriusSuTikrinimu\\RezultataiSuKlaidom.json", nevalidusJson);
    }

    static void SukurtiAsmensKodus(List<Asmenys> asmenuSarasas)
    {
        Random random = new Random();

        foreach (var asmuo in asmenuSarasas)
        {
            // Patikriname ar visi duomenys yra teisingi.
            if (!PatikrintDuomenis(asmuo))
            {
                Console.WriteLine($"{asmuo.vardas} {asmuo.pavarde}: Ivesti neteisingi duomenys.");
                continue;
            }

            // Sukuriame skirtingus atsitiktinius trys priespaskutinius skaičius kiekvienam asmeniui
            int atsitiktinisSkaicius = random.Next(1000);

            //Sukuriam pirma skaiciu, ji nustatome pagal gimimo šimtmetį ir lytį
            int pirmasSkaicius = NustatytiPirmaSkaiciu(asmuo);

            // Nustatome kontrolini skaiciu
            int s = pirmasSkaicius * 1 + (asmuo.metai / 10 % 10) * 2 + (asmuo.metai % 10) * 3 + (asmuo.menuo / 10) * 4 + (asmuo.menuo % 10) * 5 + (asmuo.diena / 10) * 6 + (asmuo.diena % 10) * 7 + (atsitiktinisSkaicius / 100) * 8 + (atsitiktinisSkaicius / 10 % 10) * 9 + (atsitiktinisSkaicius % 10) * 1;
            int kontrolinisSkaicius = s % 11;
            int paskutinisSkaicius;

            if (kontrolinisSkaicius != 10)
            {
                paskutinisSkaicius = kontrolinisSkaicius;
            }
            else
            {
                int s2 = pirmasSkaicius * 3 + (asmuo.metai / 10 % 10) * 4 + (asmuo.metai % 10) * 5 + (asmuo.menuo / 10) * 6 + (asmuo.menuo % 10) * 7 + (asmuo.diena / 10) * 8 + (asmuo.diena % 10) * 9 + (atsitiktinisSkaicius / 100) * 1 + (atsitiktinisSkaicius / 10 % 10) * 2 + (atsitiktinisSkaicius % 10) * 3;
                kontrolinisSkaicius = s2 % 11;

                if (kontrolinisSkaicius != 10)
                {
                    paskutinisSkaicius = kontrolinisSkaicius;
                }
                else
                {
                    paskutinisSkaicius = 0;
                }
            }

            // Sukuriam asmens kodą pagal turimus duomenis
            string kodas = $"{pirmasSkaicius}{asmuo.metai % 100:00}{asmuo.menuo:00}{asmuo.diena:00}{atsitiktinisSkaicius:000}{paskutinisSkaicius}";

            // Įrašome sugeneruotą kodą į objektą
            asmuo.asmens_kodas = kodas;
        }
    }

    static bool PatikrintDuomenis(Asmenys asmuo)
    {
        if (asmuo.metai < 1800 ||
            asmuo.menuo < 1 || asmuo.menuo > 12 ||
            asmuo.diena < 1 || asmuo.diena > 31 ||
            NustatytiPirmaSkaiciu(asmuo) == 0)
        {
            return false;
        }
        return true;
    }

    static bool PatikrintDuomenis(Asmenys asmuo, string kodas)
    {
        if (!PatikrintDuomenis(asmuo) || kodas.Length != 11)
        {
            return false;
        }

        int pirmasSkaicius = NustatytiPirmaSkaiciu(asmuo);

        int s = pirmasSkaicius * 1 + (asmuo.metai / 10 % 10) * 2 + (asmuo.metai % 10) * 3 + (asmuo.menuo / 10) * 4 + (asmuo.menuo % 10) * 5 + (asmuo.diena / 10) * 6 + (asmuo.diena % 10) * 7 + (int.Parse(kodas.Substring(6, 3)) / 100) * 8 + (int.Parse(kodas.Substring(6, 3)) / 10 % 10) * 9 + (int.Parse(kodas.Substring(6, 3)) % 10) * 1;
        int kontrolinisSkaicius = s % 11;
        int paskutinisSkaicius;

        if (kontrolinisSkaicius != 10)
        {
            paskutinisSkaicius = kontrolinisSkaicius;
        }
        else
        {
            int s2 = pirmasSkaicius * 3 + (asmuo.metai / 10 % 10) * 4 + (asmuo.metai % 10) * 5 + (asmuo.menuo / 10) * 6 + (asmuo.menuo % 10) * 7 + (asmuo.diena / 10) * 8 + (asmuo.diena % 10) * 9 + (int.Parse(kodas.Substring(6, 3)) / 100) * 1 + (int.Parse(kodas.Substring(6, 3)) / 10 % 10) * 2 + (int.Parse(kodas.Substring(6, 3)) % 10) * 3;
            kontrolinisSkaicius = s2 % 11;

            if (kontrolinisSkaicius != 10)
            {
                paskutinisSkaicius = kontrolinisSkaicius;
            }
            else
            {
                paskutinisSkaicius = 0;
            }
        }
        return paskutinisSkaicius == int.Parse(kodas.Substring(10, 1));
    }

    static int NustatytiPirmaSkaiciu(Asmenys asmuo)
    {
        int pirmasSkaicius = 0;

        if (asmuo.metai >= 1800 && asmuo.metai < 1900)
        {
            if (asmuo.lytis == "Male")
            {
                pirmasSkaicius = 1;
            }
            else
            {
                pirmasSkaicius = 2;
            }
        }
        else if (asmuo.metai >= 1900 && asmuo.metai < 2000)
        {
            if (asmuo.lytis == "Male")
            {
                pirmasSkaicius = 3;
            }
            else
            {
                pirmasSkaicius = 4;
            }
        }
        else if (asmuo.metai >= 2000 && asmuo.metai < 2100)
        {
            if (asmuo.lytis == "Male")
            {
                pirmasSkaicius = 5;
            }
            else
            {
                pirmasSkaicius = 6;
            }
        }
        return pirmasSkaicius;
    }
}