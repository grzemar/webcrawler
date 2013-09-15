using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebAnalyzer;

namespace NaiveBayesClassifier
{
    public class Classifier : IClassifier
    {
        private DataSet classifierData;

        private DataTable trainData;

        private readonly List<string> usedWords;

        public List<string> Classes
        {
            get;
            private set;
        }

        public Classifier(int orgin)
        {
            classifierData = new DataSet();
            trainData = new DataTable();

            usedWords = new List<string> { "ja", "ty", "admin", "user", "uzytkownik", "forum",
                "moderator", "ile", "czas",
            "poker", "party", "stars", "rozdanie",
            "sport", "trening", "medal", "gol", 
            "algorytm", "programowanie", "kod", "debugowanie", "bezpieczenstwo",
            "gry", "diablo", "logiczne", "zrecznosciowe", "strategiczne",
            "film", "serial", "kino", "lektor",
            "książka", "audiobook", "tolkien", "potter", "fantasy",
            "samochód", "tuning", "motoryzacja", "auto",
            "fotografia", "grafika", "draw", "photo",
            "faq", "pomoc", "informacja", "regulamin",
            "sprzet", "procesor", "maszyna", "plyta", "podzespol"
            };

            Classes = new List<string> { "poker",
            "sport", "programowanie", "gry", "filmy", "ksiazki",
            "motoryzacja", "grafika", "organizacja", "sprzet"};
        }

        public Classifier()
        {
            classifierData = new DataSet();
            trainData = new DataTable();


            usedWords = new List<string> {
                "poker", "party", "stars", "holdem", "rozdanie",
                "sport", "trening", "medal", "piłka", "ćwiczenia", "wyskok",
                "programowanie", "algorytm", "kod", "debugowanie", "obiektowe", "c++", "java",
                "gry", "gra", "grafika", "DirectX",
            };

            Classes = new List<string> { 
                "poker",
                "sport", 
                "programowanie", 
                "gry"};
        }



        public void TrainClassifier(IEnumerable<Document> documents)
        {
            DataTable table = new DataTable();
            table.Columns.Add("class");
            foreach (string word in usedWords)
                table.Columns.Add(word, typeof(double));

            foreach (Document document in documents)
                if (document.AmIProperDocument == true && document.DocumentClass != String.Empty)
                {
                    DataRow row = table.NewRow();
                    row["class"] = document.DocumentClass;
                    foreach (string word in usedWords)
                    {
                        if (document.WordsCount.ContainsKey(word))
                            row[word] = (double)document.WordsCount[word];
                        else
                            row[word] = 0.01d;
                    }
                    table.Rows.Add(row);
                }
            Train(table);
        }

        public void ClassifyDocuments(IEnumerable<Document> documents)
        {
            foreach (Document document in documents)
                if (document.AmIProperDocument == true && document.DocumentClass == String.Empty)
                {
                    double[] wordsCount = new double[usedWords.Count];
                    for (int i = 0; i < usedWords.Count; i++)
                    {
                        if (document.WordsCount.ContainsKey(usedWords[i]))
                            wordsCount[i] = (double)document.WordsCount[usedWords[i]];
                        else
                            wordsCount[i] = 0.01d;
                    }
                    document.DocumentClass = Classify(wordsCount);
                }

        }


        private void Train(DataTable table)
        {
            classifierData.Tables.Add(table);

            DataTable distribution = classifierData.Tables.Add("Distribution");
            distribution.Columns.Add(table.Columns[0].ColumnName);

            for (int i = 1; i < table.Columns.Count; i++)
            {
                distribution.Columns.Add(table.Columns[i].ColumnName + "Mean");
                distribution.Columns.Add(table.Columns[i].ColumnName + "Variance");
            }


            var results = (from myRow in table.AsEnumerable()
                           group myRow by myRow.Field<string>(table.Columns[0].ColumnName) into g
                           select new { Name = g.Key, Count = g.Count() }).ToList();

            for (int j = 0; j < results.Count; j++)
            {
                DataRow row = distribution.Rows.Add();
                row[0] = results[j].Name;

                int a = 1;
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    row[a] = (table.SelectRows(i,
                             string.Format("{0} = '{1}'",
                             table.Columns[0].ColumnName,
                             results[j].Name))).Mean();
                    row[a + 1] = (table.SelectRows(i,
                               string.Format("{0} = '{1}'",
                               table.Columns[0].ColumnName,
                               results[j].Name))).Variance();
                    a += 2;
                }
            }
            int x = 0;
        }

        private string Classify(double[] oneFile)
        {
            Dictionary<string, double> score = new Dictionary<string, double>();

            var results = (from myRow in classifierData.Tables[0].AsEnumerable()
                           group myRow by myRow.Field<string>(
                                 classifierData.Tables[0].Columns[0].ColumnName) into g
                           select new { Name = g.Key, Count = g.Count() }).ToList();

            for (int i = 0; i < results.Count; i++)
            {
                List<double> subScoreList = new List<double>();
                int a = 1, b = 1;
                for (int k = 1; k < classifierData.Tables["Distribution"].Columns.Count; k = k + 2)
                {
                    double mean = Convert.ToDouble(classifierData.Tables["Distribution"].Rows[i][a]);
                    double variance = Convert.ToDouble(classifierData.Tables["Distribution"].Rows[i][a + 1]);
                    double result = MathUtility.NormalDistance(oneFile[b - 1], mean,
                        MathUtility.SquareRoot(variance));
                    subScoreList.Add(result);
                    a += 2; b++;
                }

                double finalScore = 0;
                for (int z = 0; z < subScoreList.Count; z++)
                {
                    if (finalScore == 0)
                    {
                        finalScore = subScoreList[z];
                        continue;
                    }

                    finalScore = finalScore * subScoreList[z];
                }

                score.Add(results[i].Name, finalScore * 0.5);
            }

            if (score.Count == 0)
            {
                return "non";
            }
            else
            {
                double maxOne = score.Max(c => c.Value);
                var name = (from c in score
                            where c.Value == maxOne
                            select c.Key).First();

                return name;
            }
        }
    }
}

