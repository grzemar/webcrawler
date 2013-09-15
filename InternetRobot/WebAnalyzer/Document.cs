using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace WebAnalyzer
{
    public class Document
    {
        // ścieżka do pliku na dysku
        public string Path { get; private set; }

        //słownik zawierający słowa z pliku wraz z ich licznością
        public Dictionary<string, int> WordsCount { get; set; }

        // ilosc obrazow na stronie
        public List<string> Images { get; private set; }

        // klasa, która zostaje przydzielona ręcznie (w przypadku danych uczących) lub wyznaczona przez klasyfikator;
        public string DocumentClass { get; set; }

        // flaga oznajmiajaca czy ten dokmuent nadaje sie to analizy
        // poki co jedynym kryterium jest jego istnienie w systemie plikow
        public bool AmIProperDocument { get; private set; }

        //dalej konstruktor i metody, w tym metoda publiczna przetwarzająca plik i zliczająca w nim słowa
        public Document(string path)
        {
            this.Images = new List<string>();

            this.Path = path;
            this.DocumentClass = string.Empty;

            if (!System.IO.File.Exists(this.Path))
            {
                AmIProperDocument = false;
            }
            else
            {
                AmIProperDocument = true;
            }
        }

        public void AnalyzeMe()
        {
            if (AmIProperDocument)
            {
                //string tempPath = @"C:\crawler\9-14-2013\http___web.mit.edu_facts_mission.html";
                //string fileText = File.ReadAllText(tempPath);
                this.analyzeImages();
                this.analyzeText();
            }
        }

        private void analyzeImages()
        {
            string documentText = File.ReadAllText(this.Path);

            const string imgsrcPattern = @"<img.*?src=""(?<url>.*?)"".*?>";
            Regex rx = new Regex(imgsrcPattern);
            foreach (Match m in rx.Matches(documentText))
            {
                Images.Add(m.Groups["url"].Value);
            }
        }

        private void analyzeText()
        {
            const string allowedCharactersPattern = @"^[a-zA-Z]+$";
            Regex allowedCharacters = new Regex(allowedCharactersPattern);

            HtmlAgilityPack.Samples.HtmlToText toText = new HtmlAgilityPack.Samples.HtmlToText();
            string pureHtmlTxt = toText.Convert(this.Path);
            pureHtmlTxt = pureHtmlTxt.ToLower();

            string[] wordsTab = pureHtmlTxt
                .Trim()
                .Split()
                .Where(str => str != "")
                .Where(str => allowedCharacters.IsMatch(str) == true)
                .ToArray();

            WordsCount = wordsTab
                .GroupBy(w => w)
                .Select(g => new { Word = g.Key, Count = g.Count() })
                .ToDictionary(t => t.Word, t => t.Count);
        }
    }

}
