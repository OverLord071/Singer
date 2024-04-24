using iTextSharp.text;
using iTextSharp.text.pdf;
using Image = iTextSharp.text.Image;

namespace Singer.Utilities.Appearance;

public class QrAppearance : ICustomAppearance
{
    private readonly string signerName;
    private readonly string firstName;
    private readonly string lastName;
    private readonly string reason;
    private readonly string location;
    private readonly string signTime;

    public QrAppearance(string signerName, string reason, string location, string signTime)
    {
        this.signerName = signerName;
        var names = signerName.Split(' ');
        this.firstName = names[0] + " " + names[1];
        this.lastName = names[2] + " " + names[3];
        this.reason = reason;
        this.location = location;
        this.signTime = signTime;
    }

    public void CreateCustomAppearance(PdfSignatureAppearance signatureAppearance, int pageNumber, PdfReader pdfDocument, Rectangle signaturePositionOnPage)
    {

        signatureAppearance.SetVisibleSignature(signaturePositionOnPage, pageNumber, null);

        PdfContentByte layer2 = signatureAppearance.GetLayer(2);
        PdfContentByte canvas = layer2;

        BaseFont fontCourier = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        BaseFont fontCourierBold = BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        string text = "FIRMADO POR: " + signerName.Trim() + "\n";
        text += "RAZON: " + reason + "\n";
        text += "LOCALIZACION: " + location + "\n";
        text += "FECHA: " + signTime + "\n";
        text += "VALIDAR CON: www.firmadigital.gob.ec \n3.0.2";

        BarcodeQRCode qrCode = new BarcodeQRCode(text, 1, 1, null);
        Image qrImage = qrCode.GetImage();

        if (qrImage != null)
        {
            qrImage.ScaleToFit(signaturePositionOnPage.Width, signaturePositionOnPage.Height);
            qrImage.SetAbsolutePosition(0, 0);
            canvas.AddImage(qrImage);
        }
        else
        {
            Console.WriteLine("Error: qrImage is null");
        }

        canvas.BeginText();
        canvas.SetFontAndSize(fontCourier, 3);
        canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Firmado electrónicamente por:", signaturePositionOnPage.Width - 74, signaturePositionOnPage.Height - 17, 0);
        canvas.SetFontAndSize(fontCourierBold, 6);
        canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, firstName.Trim(), signaturePositionOnPage.Width - 74, signaturePositionOnPage.Height - 22, 0);
        canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, lastName.Trim(), signaturePositionOnPage.Width - 74, signaturePositionOnPage.Height - 27, 0);
        canvas.EndText();
    }
}
