using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using pdfquestAPI.Documents.Models;
using System.Collections.Generic;
using System.Linq;

namespace pdfquestAPI.Documents
{
    public class PerjanjianDocument : IDocument
    {
        private readonly PerjanjianDocumentModel _model;

        public PerjanjianDocument(PerjanjianDocumentModel model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Calibri));

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                    });

                    page.Content().Element(ComposeContent);
                });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(15);

                // --- JUDUL UTAMA ---
                column.Item().AlignCenter().Text("PERJANJIAN KERJA SAMA").Bold().FontSize(14);
                column.Item().AlignCenter().Text("ANTARA").Bold().FontSize(12);
                column.Item().AlignCenter().Text("PT ASURANSI JIWA INHEALTH INDONESIA").Bold().FontSize(12);
                column.Item().AlignCenter().Text("DAN").Bold().FontSize(12);
                column.Item().AlignCenter().Text((_model.PihakKedua.NamaEntitasCalonProvider ?? "").ToUpper()).Bold().FontSize(12);
                
                column.Item().PaddingTop(20).Text(text =>
                {
                    text.Span("No. PT Asuransi Jiwa Inhealth Indonesia : ").SemiBold();
                    text.Span(_model.Perjanjian.NoPtInhealth ?? "........................");
                });
                column.Item().Text(text =>
                {
                    text.Span($"No. {_model.PihakKedua.NamaEntitasCalonProvider} : ").SemiBold();
                    text.Span(_model.Perjanjian.NoPtPihakKedua ?? "........................");
                });

                // --- BAGIAN PEMBUKA ---
                column.Item().PaddingTop(20).Text(text =>
                {
                    text.DefaultTextStyle(x => x.LineHeight(1.5f));
                    text.Span("Perjanjian Kerja Sama ini dibuat dan ditandatangani di Jakarta pada hari ");
                    text.Span($"{_model.Perjanjian.TanggalTandaTangan:dddd, dd MMMM yyyy}").Bold();
                    text.Span(" oleh dan antara:");
                });
                
                // --- KETENTUAN KHUSUS (BAGIAN DINAMIS) ---
                column.Item().PageBreak(); 
                column.Item().Text("KETENTUAN KHUSUS").Bold().FontSize(14).AlignCenter();
                
                foreach (var bab in _model.KetentuanKhusus)
                {
                    column.Item().PaddingTop(15).Text($"{bab.UrutanTampil}. {bab.JudulTeks.ToUpper()}").Bold();

                    foreach (var subBab in bab.SubBab)
                    {
                        column.Item().PaddingLeft(20).Text(subBab.Konten);
                        RenderPoin(column, subBab.Poin, 40);
                    }
                }
            });
        }
        
        void RenderPoin(ColumnDescriptor column, List<PoinModel> poinList, float indent)
        {
            foreach (var poin in poinList)
            {
                column.Item().PaddingLeft(indent).Text(txt => {
                    txt.Span($"{poin.UrutanTampil}. ").NormalWeight();
                    txt.Span(poin.TeksPoin);
                });

                if (poin.SubPoin.Any())
                {
                    RenderPoin(column, poin.SubPoin, indent + 20);
                }
            }
        }
    }
}
