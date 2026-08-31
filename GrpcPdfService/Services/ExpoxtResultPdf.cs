using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UserMqDto;
namespace GrpcPdfService.Services
{
    public static class ExpoxtResultPdf
    {
        static public void Expoxt(TestDto TestDto)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(14).FontColor(Colors.Grey.Darken3));


                    page.Header()
                        .PaddingBottom(1, Unit.Centimetre)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("ООО МедНорст").Bold().FontSize(14);
                                column.Item().Text("Отчет о предоставленной услуге #" + TestDto.Id.ToString() ).FontSize(14).Italic();
                            });

                            row.ConstantItem(100).AlignLeft().Text(DateTime.Now.ToString("yyyy-MM-dd")).FontSize(14);

                        });
                    page.Content().PaddingBottom(1, Unit.Centimetre).Row(row => {

                        row.RelativeItem().Column(colum => { 
                            colum.Item().Text("ФИО клиента: " + TestDto.PatientName);
                            colum.Item().Text("Наименование услуги: " + TestDto.Name);
                            colum.Item().Text("Примечания: " + TestDto.Description);
                            colum.Item().Text("Дата предоставления: " + TestDto.TestDate.ToString("dd-MM-yyyy"));
                            colum.Item().Text("Итого: " + TestDto.Price + "руб.");
                    
                        });
                    
                    });
                    page.Footer()
                        .AlignBottom()
                        .PaddingTop(1, Unit.Centimetre)
                        .Row(row =>
                        {
                            row.RelativeItem().Text("ООО МедНорст").FontSize(9).FontColor(Colors.Grey.Medium);
                            row.RelativeItem().AlignRight().Text(text =>
                            {
                                text.Span("Page ").FontSize(9);
                                text.CurrentPageNumber().FontSize(9); 
                                text.Span(" of ").FontSize(9);
                                text.TotalPages().FontSize(9);
                            });
                        });
               
                    });
            });

            
            string filename = "SalesReport.pdf";
            document.GeneratePdf(filename);
        }
    }
}
