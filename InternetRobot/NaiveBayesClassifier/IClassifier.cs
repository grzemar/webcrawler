using System;
using WebAnalyzer;
using System.Collections.Generic;

namespace NaiveBayesClassifier
{
    public interface IClassifier
    {
        List<string> Classes { get; }
        void ClassifyDocuments(IEnumerable<Document> documents);
        void TrainClassifier(IEnumerable<Document> documents);
    }
}
