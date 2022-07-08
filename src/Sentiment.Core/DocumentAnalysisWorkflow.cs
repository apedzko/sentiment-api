namespace Sentiment.Core
{
    public sealed class DocumentAnalysisWorkflow
    {
        private IDocumentRepository _documentRepository;
        private IDocumentParser _documentParser;
        private IDocumentAnalyzer _documentAnalyzer;
        private IDocumentAnalysisResultRepository _documentAnalysisResultRepository;

        public DocumentAnalysisWorkflow(IDocumentRepository documentRepository, IDocumentParser documentParser, IDocumentAnalyzer documentAnalyzer, IDocumentAnalysisResultRepository documentAnalysisResultRepository)
        {
            ArgumentNullException.ThrowIfNull(documentRepository, nameof(documentRepository));
            ArgumentNullException.ThrowIfNull(documentParser, nameof(documentParser));
            ArgumentNullException.ThrowIfNull(documentAnalyzer, nameof(documentAnalyzer));
            ArgumentNullException.ThrowIfNull(documentAnalysisResultRepository, nameof(documentAnalysisResultRepository));

            _documentRepository = documentRepository;
            _documentParser = documentParser;
            _documentAnalyzer = documentAnalyzer;
            _documentAnalysisResultRepository = documentAnalysisResultRepository;
        }

        public async Task AnalyzeDocumentAsync(DocumentFile documentFile)
        {
            ArgumentNullException.ThrowIfNull(documentFile);

            Document document = await _documentRepository.CreateDocumentAsync(documentFile);

            DocumentContent content = await _documentParser.ParseDocumentAsync(document);

            DocumentAnalysisResult result = await _documentAnalyzer.AnalyzeDocumentAsync(document);

            await _documentAnalysisResultRepository.CreateAsync(result);
        }
    }
}
