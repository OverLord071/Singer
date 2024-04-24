using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Singer.Utilities.Appearance;

public interface ICustomAppearance
{
    void CreateCustomAppearance(PdfSignatureAppearance signatureAppearance, int pageNumber, PdfReader pdfDocument, Rectangle signaturePositionOnPage);
}
