using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using pdfquestAPI.Documents.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace pdfquestAPI.Documents
{
    public class PerjanjianDocument : IDocument
    {
        private readonly PerjanjianDocumentModel _model;
        private int _chapterNumber = 10;
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
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Calibri).LineHeight(1.5f));
                    page.Footer().AlignCenter().Text(text => text.CurrentPageNumber().FontSize(8));

                    page.Content().Column(column =>
                    {
                        ComposeHeader(column.Item());
                        ComposeOpeningPageContent(column);
                        ComposeSecondPageContent(column);
                        ComposeContent(column);

                        if (_model.Lampiran != null && _model.Lampiran.Any())
                        {
                            ComposeLampiran(column);
                        }
                    });
                });
        }


        void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(12).Bold();
            var numberLabelStyle = TextStyle.Default.FontSize(10);
            var boldNumberStyle = TextStyle.Default.FontSize(10).Bold();

            container.PaddingBottom(1).Column(column =>
            {
                column.Spacing(1);
                column.Item().AlignCenter().Text(text =>
                {
                    text.Span("PERJANJIAN KERJA SAMA").Style(titleStyle);
                    text.EmptyLine();
                    text.Span("ANTARA").Style(titleStyle);
                    text.EmptyLine();
                    text.Span("PT ASURANSI JIWA INHEALTH INDONESIA").Style(titleStyle);
                    text.EmptyLine();
                    text.Span("DAN").Style(titleStyle);
                    text.EmptyLine();
                    text.Span($"PT {_model.PihakKedua.NamaEntitasCalonProvider.ToUpper()}").Style(titleStyle);
                    text.EmptyLine();
                    text.Span("TENTANG").Style(titleStyle);
                    text.EmptyLine();
                    text.Span("PEMBERIAN PELAYANAN KESEHATAN").Style(titleStyle);
                    text.EmptyLine();
                    text.Span("BAGI PESERTA ASURANSI KESEHATAN PT ASURANSI JIWA INHEALTH INDONESIA").Style(titleStyle);
                });

                column.Item().LineHorizontal(1).LineColor(Colors.Black);

                column.Item().PaddingTop(2).Row(row =>
                {
                    row.ConstantItem(220).Text("No. PT Asuransi Jiwa Inhealth Indonesia").Style(boldNumberStyle);
                    row.ConstantItem(10).Text(":").Style(numberLabelStyle);
                    row.RelativeItem().Text($"{_model.Perjanjian.NoPtInhealth}").Style(boldNumberStyle);
                });

                column.Item().PaddingTop(2).Row(row =>
                {
                    row.ConstantItem(220).Text($"No. PT {_model.PihakKedua.NamaEntitasCalonProvider.ToUpper()}").Style(boldNumberStyle);
                    row.ConstantItem(10).Text(":").Style(numberLabelStyle);
                    row.RelativeItem().Text($"{_model.Perjanjian.NoPtPihakKedua}").Style(boldNumberStyle);
                });
            });
        }


        void ComposeOpeningPageContent(ColumnDescriptor column)
        {
            var textStyle = TextStyle.Default.FontSize(10);
            var boldTextStyle = TextStyle.Default.FontSize(10).Bold();

            column.Item().PaddingTop(10).Text(text =>
            {
                text.Justify();
                text.Span("Perjanjian Kerja Sama Antara PT Asuransi Jiwa Inhealth Indonesia dan ").Style(textStyle);
                text.Span($"PT {_model.PihakKedua.NamaEntitasCalonProvider.ToUpper()}").Style(boldTextStyle);
                text.Span(", tentang Pelayanan Kesehatan, Obat, dan Alat Kesehatan Bagi Peserta Asuransi Kesehatan PT Asuransi Jiwa Inhealth Indonesia (untuk selanjutnya disebut sebagai ”").Style(textStyle);
                text.Span("Perjanjian").Style(boldTextStyle);
                text.Span("”) dibuat dan ditandatangani di Jakarta pada hari ").Style(textStyle);
                text.Span($"{_model.Perjanjian.TanggalTandaTangan.ToString("dddd")} tanggal {_model.Perjanjian.TanggalTandaTangan.ToString("dd")} Bulan {_model.Perjanjian.TanggalTandaTangan.ToString("MMMM")} Tahun {_model.Perjanjian.TanggalTandaTangan.ToString("yyyy")}").Style(textStyle);
                text.Span(" oleh dan antara:").Style(textStyle);
            });

            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(35).AlignTop().Text("1.").Style(textStyle);
                row.RelativeItem().Text(text =>
                {
                    text.Justify();
                    text.Span("PT ASURANSI JIWA INHEALTH INDONESIA").Style(boldTextStyle);
                    text.Span(", suatu perseroan terbatas yang didirikan berdasarkan hukum negara Republik Indonesia, pemegang Nomor Induk Berusaha No. ").Style(textStyle);
                    text.Span($"{_model.PihakPertama.NomorNibPihakPertama}").Style(boldTextStyle);
                    text.Span(", beralamat terdaftar di Mandiri Inhealth Tower, Jl. Prof. DR. Satrio Kav E-IV No. 6, Mega Kuningan, Jakarta Selatan, yang dalam hal ini diwakili oleh ").Style(textStyle);
                    text.Span($"{_model.PihakPertama.NamaPerwakilanPihakPertama}").Style(boldTextStyle);
                    text.Span(" dalam kapasitasnya selaku ").Style(textStyle);
                    text.Span($"{_model.PihakPertama.JabatanPerwakilanPihakPertama}").Style(boldTextStyle);
                    text.Span(", dan oleh karenanya berhak bertindak untuk dan atas nama ").Style(textStyle);
                    text.Span("PT ASURANSI JIWA INHEALTH INDONESIA (“Pihak Pertama”);").Style(boldTextStyle);
                    text.Span("dan").Style(textStyle);
                });
            });

            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(35).AlignTop().Text("2.").Style(textStyle);
                row.RelativeItem().Text(text =>
                {
                    text.Justify();
                    text.Span($"{_model.PihakKedua.NamaEntitasCalonProvider}").Style(boldTextStyle);
                    text.Span(", suatu ").Style(textStyle);
                    text.Span($"{_model.PihakKedua.JenisPerusahaan}").Style(boldTextStyle);
                    text.Span(" yang didirikan berdasarkan hukum negara Republik Indonesia, pemegang Nomor Induk Berusaha No. ").Style(textStyle);
                    text.Span($"{_model.PihakKedua.NoNibPihakKedua}").Style(boldTextStyle);
                    text.Span(", beralamat terdaftar di ").Style(textStyle);
                    text.Span($"{_model.PihakKedua.AlamatPemegangPolis}").Style(boldTextStyle);
                    text.Span(", yang dalam hal ini diwakili oleh ").Style(textStyle);
                    text.Span($"{_model.PihakKedua.NamaPerwakilan}").Style(boldTextStyle);
                    text.Span(" dalam kapasitasnya selaku ").Style(textStyle);
                    text.Span($"{_model.PihakKedua.JabatanPerwakilan}").Style(boldTextStyle);
                    text.Span(", dan oleh karenanya berhak bertindak untuk dan atas nama ").Style(textStyle);
                    text.Span($"{_model.PihakKedua.NamaEntitasCalonProvider}").Style(boldTextStyle);
                    text.Span(" (“Pihak Kedua”).").Style(boldTextStyle);
                });
            });

            column.Item().PaddingTop(10).Text(text =>
            {
                text.Justify();
                text.Span("Pihak Pertama dan Pihak Kedua untuk selanjutnya secara bersama-sama disebut sebagai “").Style(textStyle);
                text.Span("Para Pihak").Style(boldTextStyle);
                text.Span("” dan masing-masing selanjutnya disebut sebagai “").Style(textStyle);
                text.Span("Pihak").Style(boldTextStyle);
                text.Span("”.").Style(textStyle);
                text.EmptyLine();
                text.Span("Para Pihak terlebih dahulu menerangkan hal-hal sebagai berikut:").Style(textStyle);
            });

            column.Item().PaddingTop(10).Column(list =>
            {
                list.Spacing(3);
                list.Item().Row(row =>
                {
                    row.ConstantItem(35).AlignTop().Text("1.").Style(textStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Bahwa Pihak Pertama merupakan perseroan terbatas yang bergerak di bidang asuransi jiwa yang telah memperoleh izin dan persetujuan yang dibutuhkan untuk menjalankan kegiatan usahanya dan dalam menjalankan kegiatan usaha diawasi oleh Otoritas Jasa Keuangan Republik Indonesia.").Style(textStyle);
                    });
                });

                list.Item().PaddingTop(3).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().Text("2.").Style(textStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Bahwa Pihak Kedua adalah sarana/fasilitas layanan kesehatan yang telah memperoleh izin untuk menjalankan kegiatan usaha sebagai fasilitas layanan kesehatan ").Style(textStyle);
                        text.Span($"{_model.PihakKedua.JenisFasilitas} {_model.PihakKedua.NamaFasilitasKesehatan}").Style(boldTextStyle);
                        text.Span($" berdasarkan").Style(textStyle);
                        text.Span($" {_model.PihakKedua.NamaDokumenIzin}").Style(boldTextStyle);
                        text.Span(" Nomor :").Style(textStyle);
                        text.Span($" {_model.PihakKedua.NomorDokumenIzin}").Style(boldTextStyle);
                        text.Span(" tertanggal").Style(textStyle);
                        text.Span($" {_model.PihakKedua.TanggalDokumenIzin?.ToString("dd/MM/yyyy")}").Style(boldTextStyle);
                        text.Span(" yang diterbitkan oleh").Style(textStyle);
                        text.Span($" {_model.PihakKedua.InstansiPenerbitIzin}.").Style(boldTextStyle);
                    });
                });

                list.Item().PaddingTop(3).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().Text("3.").Style(textStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Bahwa Pihak Pertama bermaksud mengadakan kerja sama dengan Pihak Kedua sehubungan dengan penyediaan Pelayanan Kesehatan, Obat, dan Alat Kesehatan bagi Peserta (sebagaimana didefinisikan di bawah) dan Pihak Kedua bersedia untuk menyediakan Pelayanan Kesehatan, Obat, dan Alat Kesehatan bagi Peserta (sebagaimana didefinisikan di bawah) Pihak pertama.").Style(textStyle);
                    });
                });
            });
        }

        void ComposeSecondPageContent(ColumnDescriptor column)
        {
            var textStyle = TextStyle.Default.FontSize(10);
            var boldTextStyle = TextStyle.Default.FontSize(10).Bold();

            column.Item().PaddingTop(10).Column(list =>
            {
                list.Spacing(10);
                list.Item().Row(row =>
                {
                    row.ConstantItem(35).AlignTop().Text("4.").Style(textStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Bahwa Para Pihak telah menandatangani Berita Acara Kesepakatan Penyediaan Layanan Kesehatan No. ").Style(textStyle);
                        text.Span($"{_model.Perjanjian.NomorBeritaAcara}").Style(boldTextStyle);
                        text.Span(" tertanggal").Style(textStyle);
                        text.Span($" {_model.Perjanjian.TanggalBeritaAcara.ToString("dd/MM/yyyy")} (“Berita Acara”)").Style(boldTextStyle);
                        text.Span("dan menyepakati syarat dan ketentuan penyediaan Pelayanan Kesehatan, Obat, dan Alat kesehatan oleh Pihak Kedua kepada Peserta (sebagaimana didefinisikan di bawah) Pihak Pertama .").Style(textStyle);
                    });
                });

                list.Item().PaddingTop(3).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().Text("5.").Style(textStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Bahwa Para Pihak sepakat untuk menandatangani Perjanjian ini untuk mengatur syarat dan ketentuan kerja sama antara Para Pihak dan penyediaan Pelayanan Kesehatan, Obat, dan Alat Kesehatan bagi Peserta (sebagaimana didefinisikan di bawah).").Style(textStyle);
                    });
                });
            });

            column.Item().PaddingTop(10).Text(text =>
            {
                text.Span("Para Pihak ").Style(TextStyle.Default.FontSize(10));
                text.Span("DENGAN INI MENYEPAKATI SEBAGAI BERIKUT:").Style(boldTextStyle);
            });

            ComposeAllDefinitions(column);


            ComposeGeneralProvisions(column);
        }

        void ComposeGeneralProvisions(ColumnDescriptor column)
        {
            var titleStyle = TextStyle.Default.FontSize(12).Bold();
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();


            column.Item().PaddingBottom(10).PaddingTop(10).Text("II. KETENTUAN UMUM").Style(titleStyle);

            ComposeProvisionSection(column, "1.", "KERJA SAMA", new[]
            {
                "Para Pihak sepakat untuk bekerja sama dalam penyediaan pelayanan kesehatan dan obat kepada Peserta dengan tunduk pada syarat dan ketentuan yang terdapat dalam Perjanjian ini (“Kerja Sama”).",
                "Sehubungan dengan pelaksanaan Kerja Sama, Pihak Pertama akan berperan sebagai penjamin bagi Peserta untuk mendapatkan pelayanan kesehatan dan obat dari Pihak Kedua. Adapun Pihak Kedua akan menyediakan pelayanan kesehatan dan obat kepada Peserta sesuai dengan manfaat layanan asuransi yang dimiliki oleh Peserta.",
                "Para Pihak sepakat bahwa Kerja Sama akan berlangsung selama Jangka Waktu Perjanjian sebagaimana disebutkan dalam Ketentuan Khusus Pasal [•]."
            });

            ComposeProvisionSection(column, "2.", "TAHAP PENINJAUAN", new[]
            {
                "Pihak Pertama berhak untuk melakukan Peninjauan terhadap Pihak Kedua. Pihak Pertama akan melakukan Peninjauan pada waktu yang telah diberitahukan sebelumnya secara tertulis kepada Pihak Kedua. Pihak Pertama",
                "Pihak Kedua wajib memberikan akses dan bantuan yang dibutuhkan oleh Pihak Pertama dalam rangka Peninjauan.",
                "Pihak Pertama akan memberitahukan secara tertulis hasil Peninjauan serta rekomendasi sehubungan dengan hasil Peninjauan kepada Pihak Kedua.",
                "Apabila berdasarkan Peninjauan yang dilakukan oleh Pihak Pertama diketahui bahwa Pihak Kedua tidak memenuhi persyaratan yang ditetapkan oleh Pihak Pertama, maka Pihak Pertama, atas kebijakan tunggalnya berhak untuk : meninjau kembali ketentuan-ketentuan dalam Perjanjian ini. Atas kebijakan tunggalnya, Pihak Pertama berhak memilih untuk mengakhiri atau melanjutkan kerja sama berdasarkan Perjanjian ini, termasuk untuk melakukan perubahan terhadap syarat dan ketentuan yang berlaku dalam Perjanjian ini."
            }, isBulletedList: true, subListContent: new[]
            {
                "Melakukan pengakhiran terhadap Perjanjian ini, pengakhiran mana berlaku efektif dalam waktu 30 (tiga puluh) Hari Kalender sejak tanggal pemberitahuan pengakhiran yang diberikan oleh Pihak Pertama kepada Pihak Kedua; atau",
                "Melanjutkan Kerja Sama berdasarkan Perjanjian ini dengan atau tanpa melakukan perubahan atau penyesuaian terhadap syarat dan ketentuan yang berlaku dalam Perjanjian ini."
            }, subListPrefix: "2.4", subListType: "numbered");

            column.Item().PaddingTop(5).PaddingLeft(25).Row(row =>
            {
                row.ConstantItem(35).AlignTop().Text("2.5").Style(bodyStyle);
                row.RelativeItem().Text(text =>
                {
                    text.Justify();
                    text.Span("Hasil Peninjauan dapat dilampirkan pada ").Style(bodyStyle);
                    text.Span("Lampiran I").Style(boldBodyStyle);
                    text.Span(".").Style(bodyStyle);
                });
            });

            ComposeProvisionSection(column, "3.", "RUANG LINGKUP PELAYANAN KESEHATAN", new[]
            {
                "Ruang lingkup pelayanan kesehatan yang wajib disediakan oleh Pihak Kedua kepada Peserta adalah sebagaimana disebutkan pada Ketentuan Khusus Pasal [•]."
            }, isBulletedList: false);

            ComposeExclusionsSection(column);

            ComposeProvisionSection(column, "5.", "BIAYA PELAYANAN KESEHATAN", new[]
            {
                "Pihak Pertama wajib melakukan pembayaran atas biaya pelayanan kesehatan yang disediakan oleh Pihak Kedua kepada Peserta dengan tunduk pada besaran manfaat asuransi kesehatan yang dimiliki oleh Peserta.",
                "Besaran, jenis pembayaran dan tata cara pembayaran biaya pelayanan kesehatan adalah sebagaimana diatur dalam Ketentuan Khusus Pasal [•] dan Lampiran III Perjanjian ini.",
                "Segala pajak dan biaya yang timbul sehubungan dengan Perjanjian ini akan menjadi tanggung jawab masing-masing Pihak sesuai dengan ketentuan peraturan perundang-undangan yang berlaku di bidang perpajakan."
            });

            ComposeWanprestasiSection(column);
            ComposeMisuseAndFraud(column);
            ComposeIndemnity(column);
            ComposeRepresentationsAndWarranties(column);
            ComposeForceMajeure(column);
            ComposeConfidentiality(column);
            ComposePersonalData(column);
            ComposeAntiBribery(column);
            ComposeAML(column);
            ComposeTermination(column);
            ComposeCorrespondence(column);
            ComposeGoverningLaw(column);
            ComposeMiscellaneous(column);
        }


        void ComposeProvisionSection(
                ColumnDescriptor column,
                string sectionNumber,
                string sectionTitle,
                IEnumerable<string> items,
                bool isBulletedList = true,
                IEnumerable<string>? subListContent = null,
                string subListPrefix = "",
                string subListType = "")
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // Judul section 
            column.Item().ShowOnce().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text(sectionNumber).Style(bodyStyle);
                row.RelativeItem().Text(sectionTitle).Style(boldBodyStyle);
            });

            // Isi section
            column.Item().PaddingTop(5).ShowOnce().PaddingLeft(25).Column(list =>
            {
                list.Spacing(5);
                int itemNumber = 1;

                foreach (var item in items)
                {
                    if (isBulletedList)
                    {
                        var cleanSectionNumber = sectionNumber.TrimEnd('.');
                        var itemLabel = $"{cleanSectionNumber}.{itemNumber++}";

                        list.Item().Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text(itemLabel).Style(bodyStyle);


                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(item).Style(bodyStyle);
                            });
                        });
                    }
                    else
                    {

                        list.Item().ShowOnce().Text(text =>
                        {
                            text.Justify();
                            text.Span(item).Style(bodyStyle);
                        });
                    }
                }

                // Sublist jika ada
                if (subListContent != null)
                {
                    list.Item().PaddingLeft(35).ShowOnce().Column(subList =>
                    {
                        subList.Spacing(5);
                        int subItemNumber = 1;

                        foreach (var subItem in subListContent)
                        {
                            subList.Item().ShowOnce().Row(row =>
                            {
                                if (subListType == "numbered")
                                {
                                    var cleanPrefix = subListPrefix.TrimEnd('.');
                                    var subItemLabel = $"{cleanPrefix}.{subItemNumber++}";

                                    row.ConstantItem(35).AlignTop().Text(subItemLabel).Style(bodyStyle);
                                }
                                else
                                {
                                    row.ConstantItem(20).AlignTop().Text("-").Style(bodyStyle);
                                }


                                row.RelativeItem().Text(text =>
                                {
                                    text.Justify();
                                    text.Span(subItem).Style(bodyStyle);
                                });
                            });
                        }
                    });
                }
            });
        }


        void ComposeExclusionsSection(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // Judul Bagian
            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("4.").Style(bodyStyle);
                row.RelativeItem().Text("PELAYANAN KESEHATAN YANG TIDAK DITANGGUNG PIHAK PERTAMA").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).PaddingLeft(25).Column(list =>
            {
                list.Spacing(5);

                // MODIFIKASI: Teks pembuka dibuat justified
                list.Item().Text(text =>
                {
                    text.Justify();
                    text.Span("Pelayanan kesehatan yang tidak ditanggung Pihak Pertama namun dapat disesuaikan dengan manfaat yang diterima oleh Peserta sesuai dengan ketentuan polis terdiri dari:").Style(bodyStyle);
                });

                var exclusionItems = new Dictionary<string, string>
                {
                    { "a.", "Prosedur/ketentuan yang tidak sesuai diagnosa dan/atau indikasi medis;" },
                    { "b.", "Pelayanan/pengobatan terhadap gangguan mental dan perilaku yang termasuk kategori psikosa dengan acuan kode ICD-10 F-20 sampai dengan F-29 (sumber: https://icd.who.int/browse10/2019/en)." },
                    { "c.", "Konsultasi psikolog;" },
                    { "d.", "Penyakit dan/atau kecelakaan yang diakibatkan oleh perbuatan sendiri (tidak terbatas pada upaya bunuh diri, penyalahgunaan Narkoba/Zat adiktif lain, bermain petasan);" },
                    { "e.", "" },
                    { "f.", "Pelayanan bersifat kosmetik dan estetik;" },
                    { "g.", "Imunisasi selain imunisasi dasar kecuali disebutkan lain;" },
                    { "h.", "Khitanan tanpa indikasi medis;" },
                    { "i.", "Pelayanan yang belum disahkan atau diakui oleh Kementerian Kesehatan RI (masih dalam uji coba atau teknologi eksperimental) tidak terbatas pada sleep study, theraphy ozone;" },
                    { "j.", "Pelayanan program dalam Upaya Memperoleh Keturunan;" },
                    { "k.", "Alat bantu kesehatan antara lain tidak terbatas pada: kursi roda, tongkat penyangga, korset, kantong es batu/air hangat, pispot, kasur decubitus, decker, underpad;" },
                    { "l.", "Biaya administrasi lain yang tidak terkait dengan pengobatan (misalnya, administrasi pengurusan surat-surat keterangan kelahiran dan/atau kematian resume medis, Visum et repertum), Biaya transportasi, foto kopi, telepon, biaya penerjemah dokumen;" },
                    { "m.", "Memulihkan kesehatan selain di Rumah Sakit (Homecare, Sanatorium, Home Nursing (perawatan di rumah) atau untuk perawatan pribadi dan sejenisnya);" },
                    { "n.", "Perawatan dan pengobatan di klinik atau Rumah Sakit Holistic." },
                    { "o.", "AIDS dan ARC (Aids Related Complex), HIV positive, termasuk dan tidak terbatas pada pemeriksaan HIV dalam darah;" },
                    { "p.", "Kelainan bawaan/congenital dan herediter yang tidak terbatas pada hernia usia sampai dengan 8 (delapan) tahun, VSD, ASD, cretinism, thallasaemia, haemophili, phymosis, aneurisma, scoliosis;" },
                    { "q.", "Kelainan Tumbuh Kembang termasuk tidak terbatas pada ADHD dan autisme;" },
                    { "r.", "General Check Up, screening kesehatan dan tes kesehatan yang tidak berhubungan dengan pengobatan;" },
                    { "s.", "Screening ulang kantong darah oleh Rumah Sakit;" },
                    { "t.", "Pembersihan karang gigi (scaling), upaya-upaya/tindakan perawatan letak gigi (orthodontie), pemutihan gigi (bleaching) atau tindakan/layanan kesehatan gigi lainnya yang berhubungan dengan estetika;" },
                    { "u.", "Vitamin, multivitamin, obat-obatan herbal dan suplemen di luar FOI;" },
                    { "v.", "Kategori Penyakit Menular Seksual;" },
                    { "w.", "Pengobatan akibat tindakan melanggar hukum negara, kriminal, melawan penahanan yang sah, Tertanggung diserang akibat tindakan provokasi yang dilakukannya;" },
                    { "x.", "Ikut berpartisipasi aktif dalam peperangan (baik yang dinyatakan maupun tidak), keadaan seperti perang, pendudukan, gerakan pengacauan, pemberontakan, perebutan kekuasaan, pemogokan, huru-hara, dan keributan;" },
                    { "y.", "Bencana Alam (bencana yang diakibatkan oleh peristiwa atau serangkaian peristiwa yang disebabkan oleh alam antara lain berupa gempa bumi, tsunami, gunung meletus, banjir, kekeringan, angin topan, dan tanah longsor, dan sebagainya), atau kondisi lain berdasarkan keputusan Pemerintah dinyatakan sebagai kondisi dengan status keadaan darurat bencana atau keadaan kedaruratan lainnya;" },
                    { "z.", "Penerbangan bukan sebagai penumpang pesawat yang memiliki jadwal tetap, diakui secara internasional, penerbangan komersial kecuali Tertanggung diberikan perlindungan terhadap manfaat tersebut berdasarkan Polis;" },
                    { "aa.", "Penyakit atau luka yang disebabkan oleh atau berhubungan dengan radiasi ionisasi atau kontaminasi oleh radioaktif dari setiap bahan bakar nuklir atau limbah nuklir dari proses fusi/fisi nuklir atau dari setiap bahan senjata nuklir;" },
                    { "bb.", "Pembelian obat-obatan tradisional, herbal, kuasi, kosmetik, produk MLM, obat yang belum terdaftar pada Kementerian kesehatan/BP POM, atau obat tanpa resep dokter (over the counter drug);" },
                    { "cc.", "Alat yang ditanam dalam tubuh (Implant);" },
                    { "dd.", "Haemodialisa (cuci darah);" },
                    { "ee.", "Komplikasi kehamilan kecuali mengambil manfaat Persalinan;" },
                    { "ff.", "Pengobatan akupuntur yang dilakukan bertujuan untuk kecantikan, estetika, fertilisasi (usaha mendapatkan keturunan), Bayi Tabung, atau menurunkan/menaikan berat badan;" },
                    { "gg.", "Wabah, epidemi atau pandemi baik yang dinyatakan atau tidak dinyatakan oleh Pemerintah;" },
                    { "hh.", "Pembelian organ tubuh dan/atau perawatan bagi pendonor organ;" },
                    { "ii.", "Pelayanan Khusus tidak dijamin pada Plan Ruby, Silver, Blue, dan Alba;" },
                    { "jj.", "Retainer, kawat gigi, night guard;" },
                    { "kk.", "Penyakit dengan etiologic Idiopathic;" }
                };

                list.Item().PaddingLeft(15).Column(subList =>
                {
                    subList.Spacing(5);
                    foreach (var exclusion in exclusionItems)
                    {
                        if (exclusion.Key == "e.")
                        {
                            // Bagian untuk item 'e.' dengan sub-list
                            subList.Item().Row(row =>
                            {
                                row.ConstantItem(15).AlignTop().Text("e.").Style(bodyStyle);
                                row.RelativeItem().Column(subSubList =>
                                {
                                    subSubList.Spacing(5);

                                    // MODIFIKASI: 
                                    subSubList.Item().Text(text =>
                                    {
                                        text.Justify();
                                        text.Span("Perawatan dan/atau pengobatan karena keikutsertaan dalam aktivitas atau olahraga berbahaya yaitu:").Style(bodyStyle);
                                    });

                                    subSubList.Item().PaddingLeft(3).Column(numberedSubList =>
                                    {
                                        numberedSubList.Spacing(2);
                                        // MODIFIKASI: Semua item di sub-list dibuat justified
                                        string[] dangerousSports = {
                                            "Mendaki gunung, panjat tebing, panjat gedung;",
                                            "Bungee jumping;",
                                            "Arung jeram;",
                                            "Semua aktifitas terbang di udara (terjun payung, terbang layang, sky diving, ultrakite dan lain-lain);",
                                            "Semua aktifitas menyelam yang menggunakan alat bantu pernapasan (diving dan lain-lain);",
                                            "semua aktifitas lomba kecepatan dengan kendaraan mesin (balap motor, mobil, perahu dan lain-lain);",
                                            "semua aktifitas olahraga yang bersifat profesional."
                                        };

                                        foreach (var (sport, index) in dangerousSports.Select((value, i) => (value, i)))
                                        {

                                            numberedSubList.Item().Row(row =>
                                            {
                                                row.Spacing(5);
                                                row.ConstantItem(10).AlignTop().Text($"{index + 1})").Style(bodyStyle);
                                                row.RelativeItem().Text(text =>
                                                {
                                                    text.Justify();
                                                    text.Span(sport).Style(bodyStyle);
                                                });
                                            });
                                        }
                                    });
                                });
                            });
                        }
                        else
                        {
                            // Bagian untuk item lain (sudah justified, tidak ada perubahan)
                            subList.Item().Row(row =>
                            {
                                row.ConstantItem(15).AlignTop().Text(exclusion.Key).Style(bodyStyle);
                                row.RelativeItem().Text(text =>
                                {
                                    text.Justify();
                                    text.Span(exclusion.Value).Style(bodyStyle);
                                });
                            });
                        }
                    }
                });
            });
        }


        void ComposeWanprestasiSection(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // BAGIAN 6 - WANPRESTASI
            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("6.").Style(bodyStyle);
                row.RelativeItem().Text("WANPRESTASI").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).PaddingLeft(25).Column(list =>
            {
                list.Spacing(5);
                list.Item().Row(row =>
                {
                    row.ConstantItem(35).ShowOnce().AlignTop().Text("6.1").Style(bodyStyle);
                    row.RelativeItem().Column(subList =>
                    {
                        subList.Spacing(5);

                        // Teks pembuka (sudah justified, tidak ada perubahan)
                        subList.Item().ShowOnce().Text(text =>
                        {
                            text.Justify();
                            text.Span("Dalam hal Pihak Kedua melakukan hal-hal sebagai berikut:").Style(bodyStyle);
                        });

                        string[] wanprestasiItems =
                        {
                            "Melakukan pelanggaran atas ketentuan mana pun dari Perjanjian ini;",
                            "Tidak melayani Peserta sesuai dengan kewajibannya sebagai Provider dan/atau sesuai Perjanjian ini;",
                            "Tidak memberikan pelayanan kesehatan kepada Peserta dari Pihak Pertama sesuai dengan hak Peserta berdasarkan manfaat asuransi kesehatan yang dimiliki oleh Peserta; dan/atau",
                            "Memungut biaya tambahan kepada Peserta di luar kesepakatan yang ada,"
                        };

                        // MODIFIKASI: Daftar item dibuat justified
                        subList.Item().ShowOnce().PaddingLeft(4).Column(wanprestasiList =>
                        {
                            wanprestasiList.Spacing(5);
                            for (int i = 0; i < wanprestasiItems.Length; i++)
                            {
                                wanprestasiList.Item().Row(itemRow =>
                                {
                                    itemRow.ConstantItem(35).AlignTop().Text($"6.1.{i + 1}").Style(bodyStyle);
                                    // Perubahan di baris berikut
                                    itemRow.RelativeItem().Text(text =>
                                    {
                                        text.Justify();
                                        text.Span(wanprestasiItems[i]).Style(bodyStyle);
                                    });
                                });
                            }
                        });

                        // Teks penutup (sudah justified, tidak ada perubahan)
                        subList.Item().ShowOnce().Text(text =>
                        {
                            text.Justify();
                            text.Span("maka Pihak Pertama akan memberikan teguran kepada Pihak Kedua untuk melakukan perbaikan atas pelanggaran yang terjadi. Teguran akan diberikan dengan jumlah maksimal sebanyak 3 (tiga) kali dengan tenggang waktu setiap surat teguran selama 14 (empat belas) ").Style(bodyStyle);
                            text.Span("Hari Kalender").Style(boldBodyStyle);
                            text.Span(". Pihak Pertama berhak untuk menangguhkan pembayaran atas tagihan/invoice yang telah diajukan oleh Pihak Kedua selama Pihak Kedua belum melakukan perbaikan atas pelanggaran tersebut. Apabila Pihak Kedua gagal melakukan perbaikan dalam waktu yang telah ditentukan, maka Pihak Pertama berhak untuk mengakhiri Perjanjian ini berdasarkan pemberitahuan tertulis kepada Pihak Kedua.").Style(bodyStyle);
                        });
                    });
                });

                // Klausa 6.2 (sudah justified, tidak ada perubahan)
                list.Item().ShowOnce().Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("6.2").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Dalam hal Pihak Pertama tidak dapat melaksanakan kewajiban pembayaran atas tagihan/invoice Pihak Kedua yang telah disetujui oleh Pihak Pertama, maka Pihak Kedua berhak  sebanyak 3 (tiga) kali untuk segera menyelesaikan kewajiban pembayaran tersebut. Apabila surat teguran ketiga tidak mendapatkan tanggapan dari Pihak Pertama maka Pihak Kedua berhak memutuskan Perjanjian ini dan Pihak Pertama tetap berkewajiban untuk melakukan pembayaran tagihan/invoice dimaksud.").Style(bodyStyle);
                    });
                });
            });
        }


        void ComposeMisuseAndFraud(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // PASAL 7 - PENYALAHGUNAAN ATAU PENYIMPANGAN
            column.Item().PaddingTop(10).ShowOnce().Row(row =>
            {
                row.ConstantItem(25).AlignTop().ShowOnce().Text("7.").Style(bodyStyle);
                row.RelativeItem().Text("PENYALAHGUNAAN ATAU PENYIMPANGAN").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).ShowOnce().PaddingLeft(25).Column(list =>
            {
                list.Spacing(5);

                list.Item().Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("7.1").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Dalam hal terjadi indikasi penyalahgunaan atau penyimpangan (fraud) yang dilakukan oleh Pihak Kedua (hal mana tidak perlu dibuktikan oleh adanya putusan pengadilan atau pihak lainnya yang bersifat berwenang) atau diketahui melakukan hal-hal yang bersifat merugikan Pihak Pertama (moral hazard) pada masa kerja sama, maka:")
                            .Style(bodyStyle);
                    });
                });

                string[] fraudHandling =
                {
                    "Pihak Pertama akan menyampaikan pemberitahuan secara tertulis kepada Pihak Kedua terkait indikasi penyalahgunaan atau penyimpangan (fraud) tersebut.",
                    "Pihak Kedua wajib memberikan sanggahan dalam waktu 14 (empat belas) hari kalender setelah pemberitahuan tertulis dari Pihak Pertama dan disertai bukti tertulis yang mendukung sanggahan tersebut.",
                    "Pihak Pertama dan Pihak Kedua akan menyelesaikan sanggahan dengan itikad baik dalam waktu 10 (sepuluh) hari kalender dengan kewenangan keputusan ada di Pihak Pertama.",
                    "Apabila dalam jangka waktu 14 (empat belas) hari kalender setelah pemberitahuan tertulis tersebut dari Pihak Pertama tidak terdapat sanggahan dari Pihak Kedua, maka Pihak Kedua dianggap setuju atas keputusan Pihak Pertama dan Pihak Pertama berhak untuk tetap menolak dan tidak membayarkan klaim kepada Pihak Kedua, sekaligus dapat melakukan pemutusan kerjasama."
                };

                list.Item().PaddingLeft(35).ShowOnce().Column(subList =>
                {
                    subList.Spacing(5);
                    for (int i = 0; i < fraudHandling.Length; i++)
                    {
                        subList.Item().Row(row =>
                        {
                            row.ConstantItem(35).ShowOnce().AlignTop().Text($"7.1.{i + 1}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(fraudHandling[i]).Style(bodyStyle);
                            });
                        });
                    }
                });

                list.Item().Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("7.2").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Pihak Kedua bertanggung jawab untuk memulihkan kerugian yang diderita oleh Pihak Pertama, termasuk dalam proses pengembalian Manfaat/Klaim yang sudah dibayarkan Pihak Pertama kepada Pihak Kedua sesuai dengan prosedur dan tata cara yang ditetapkan oleh Pihak Pertama dalam jangka waktu 30 (tiga puluh) hari kalender").Style(bodyStyle);
                        text.Span(" sejak surat pemberitahuan tertulis disampaikan oleh Pihak Pertama kepada Pihak Kedua.").Style(bodyStyle);
                    });
                });

                list.Item().Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("7.3").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Apabila dalam jangka waktu tersebut Pihak Kedua belum juga mengembalikan Manfaat klaim dimaksud, maka Pihak Kedua dianggap berhutang kepada Pihak Pertama dan Pihak Pertama berhak untuk menagihkan dengan atau tanpa disertai dengan pengajuan tuntutan hukum sekaligus dapat melakukan pemutusan kerjasama.")
                            .Style(bodyStyle);
                    });
                });
            });
        }


        void ComposeIndemnity(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // BAGIAN 8 - GANTI RUGI
            column.Item().PaddingTop(10).ShowOnce().Row(row =>
            {
                row.ConstantItem(25).AlignTop().ShowOnce().Text("8.").Style(bodyStyle);
                row.RelativeItem().Text("GANTI RUGI").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).Column(list =>
            {
                list.Spacing(5);

                // PASAL 8.1
                list.Item().PaddingLeft(25).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("8.1").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Dalam hal Pihak Kedua melakukan pelanggaran apa pun terhadap ketentuan Perjanjian ini, kelalaian, kesalahan, malapraktik, fraud (termasuk namun tidak terbatas pada tindakan upcoding, pemalsuan dokumen, atau tindakan sejenis) atau tindakan lainnya yang berakibat pada:")
                            .Style(bodyStyle);
                    });
                });

                // SUBPASAL 8.1.x
                list.Item().PaddingLeft(25).ShowOnce().Column(subList =>
                {
                    subList.Spacing(5);
                    string[] consequences =
                    {
                        "timbulnya kerugian bagi Pihak Pertama; dan/atau",
                        "timbulnya kerugian bagi Peserta dan/atau pihak ketiga mana pun sehingga mengakibatkan Peserta dan/atau pihak ketiga mana pun meminta Pihak Pertama untuk mengganti kerugian dan/atau mengajukan upaya hukum kepada Pihak Pertama,"
                    };
                    for (int i = 0; i < consequences.Length; i++)
                    {
                        subList.Item().PaddingLeft(35).ShowOnce().Row(row =>
                        {
                            row.ConstantItem(35).Text($"8.1.{i + 1}").Style(bodyStyle);
                            row.RelativeItem().Text(consequences[i]).Style(bodyStyle);
                        });
                    }
                    subList.Item().PaddingLeft(35).ShowOnce().Text(text =>
                    {
                        text.Justify();
                        text.Span("maka Pihak Kedua berjanji untuk melepaskan, membebaskan, dan mengganti kerugian Pihak Pertama atas setiap kerugian yang diderita oleh Pihak Pertama termasuk namun tidak terbatas pada klaim, tuntutan, atau ganti rugi yang diajukan oleh Peserta atau pihak ketiga mana pun serta biaya hukum yang timbul sehubungan dengan tindakan pelanggaran, kelalaian, kesalahan, atau malpraktik yang dilakukan oleh Pihak Kedua.")
                            .Style(bodyStyle);
                    });
                });
            });
        }


        void ComposeRepresentationsAndWarranties(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // BAGIAN 9 - PERNYATAAN DAN JAMINAN
            column.Item().PaddingTop(10).ShowOnce().Row(row =>
            {
                row.ConstantItem(25).AlignTop().ShowOnce().Text("9.").Style(bodyStyle);
                row.RelativeItem().Text("PERNYATAAN DAN JAMINAN").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).ShowOnce().Column(list =>
            {
                list.Spacing(5);

                // Klausa 9.1 (sudah justified, tidak perlu diubah)
                list.Item().PaddingLeft(25).ShowOnce().Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("9.1").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Masing-masing Pihak dengan ini menyatakan dan menjamin kepada Pihak lainnya, baik pada tanggal penandatanganan Perjanjian ini dan selama Jangka Waktu Perjanjian, bahwa:")
                            .Style(bodyStyle);
                    });
                });

                string[] commonWarranties =
                {
                    "Pihak tersebut adalah badan usaha, badan hukum, perkumpulan, atau organisasi yang didirikan secara sah dan tunduk pada hukum yang berlaku di negara tempat pendiriannya;",
                    "Pihak tersebut memiliki hak, kekuasaan dan kewenangan untuk membuat Perjanjian ini dan untuk melaksanakan atau menyebabkan pelaksanaan setiap dan seluruh kewajibannya dalam Perjanjian ini;",
                    "Perjanjian ini dan setiap dokumen yang ditandatangani sehubungan dengan Perjanjian ini merupakan kewajiban yang sah, berlaku dan dapat dilaksanakan oleh Pihak tersebut;",
                    "Penandatanganan Perjanjian ini dan setiap dokumen yang ditandatangani sehubungan dengan Perjanjian ini serta pelaksanaan hak dan kewajiban berdasarkan Perjanjian ini oleh Pihak tersebut tidak akan bertentangan atau berakibat pada pelanggaran ketentuan hukum, peraturan, putusan, perintah, kewenangan, perjanjian, anggaran dasar atau kewajiban lain yang berlaku atau mengikat Pihak tersebut;",
                    "Setiap dan seluruh informasi dan fakta material yang diberikan oleh masing-masing Pihak kepada Pihak lainnya adalah terkini dan akurat, dan sepanjang pengetahuan Pihak tersebut, tidak ada fakta, informasi atau kondisi mengenai Pihak tersebut atau hal-hal dalam Perjanjian ini yang belum diungkapkan, yang jika diungkapkan kepada Pihak lainnya, mempunyai pengaruh material terhadap keputusan Pihak lainnya untuk menandatangani Perjanjian ini",
                    "Pihak tersebut tidak sedang terlibat dalam pelanggaran ketentuan hukum, peraturan, kewenangan, dimana pelanggaran tersebut dapat berakibat pada peninjauan kembali, penangguhan atau pencabutan izin usaha Pihak tersebut yang berkaitan dengan pelaksanaan Perjanjian ini oleh otoritas yang berwenang;",
                    "Tidak ada proses hukum, proses arbitrase atau proses administratif di hadapan pengadilan atau mahkamah lain yang tengah menunggu putusan atau diancamkan terhadap masing-masing Pihak dan yang mungkin menimbulkan suatu dampak yang merugikan terhadap kemampuan Pihak tersebut untuk melaksanakan kewajiban-kewajibannya berdasarkan Perjanjian ini; dan",
                    "Pihak tersebut akan mematuhi setiap ketentuan peraturan perundang-undangan yang berlaku sehubungan dengan pelaksanaan asuransi kesehatan."
                };

                // MODIFIKASI: Teks di dalam loop dibuat justified
                list.Item().PaddingLeft(25).ShowOnce().Column(subList =>
                {
                    subList.Spacing(5);
                    for (int i = 0; i < commonWarranties.Length; i++)
                    {
                        subList.Item().PaddingLeft(35).ShowOnce().Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text($"9.1.{i + 1}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(commonWarranties[i]).Style(bodyStyle);
                            });
                        });
                    }
                });

                // Klausa 9.2 (sudah justified, tidak perlu diubah)
                list.Item().PaddingLeft(25).ShowOnce().Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("9.2").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Pihak Kedua dengan ini menyatakan dan menjamin kepada Pihak Pertama, baik pada tanggal penandatanganan Perjanjian ini dan selama Jangka Waktu Perjanjian, bahwa :")
                            .Style(bodyStyle);
                    });
                });

                string[] secondPartyWarranties =
                {
                    "Dalam hal diperlukan oleh Pihak Pertama, Pihak Kedua bersedia sewaktu-waktu memberikan dokumen-dokumen yang dibutuhkan oleh Pihak Pertama dalam rangka melaksanakan kewajibannya berdasarkan Perjanjian ini; dan",
                    "Setiap dan seluruh dokumen perusahaan, keuangan, teknis, informasi dan data yang diberikan oleh Pihak Kedua kepada Pihak Pertama adalah benar dan akurat dan tidak menyesatkan."
                };

                // MODIFIKASI: Teks di dalam loop dibuat justified
                list.Item().PaddingLeft(25).ShowOnce().Column(subList =>
                {
                    subList.Spacing(5);
                    for (int i = 0; i < secondPartyWarranties.Length; i++)
                    {
                        subList.Item().PaddingLeft(35).Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text($"9.2.{i + 1}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(secondPartyWarranties[i]).Style(bodyStyle);
                            });
                        });
                    }
                });

                // MODIFIKASI: Klausa 9.3 dibuat justified
                list.Item().PaddingLeft(25).ShowOnce().ShowOnce().Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("9.3").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Masing-masing Pihak tidak akan memberikan dan/atau menerima uang dan/atau barang gratifikasi dan/atau bingkisan yang berhubungan dengan jabatan serta memastikan kepatuhan terhadap ketentuan Undang-Undang Pemberantasan Tindak Pidana Korupsi.").Style(bodyStyle);
                    });
                });

                // MODIFIKASI: Klausa 9.4 dibuat justified
                list.Item().PaddingLeft(25).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("9.4").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Setiap Pihak wajib segera memberitahukan secara tertulis kepada Pihak lainnya setiap hal, peristiwa atau keadaan yang dapat timbul atau diketahui setelah tanggal Perjanjian ini yang merupakan suatu pelanggaran dari atau bertentangan dengan setiap pernyataan dan jaminan dari Pihak tersebut.").Style(bodyStyle);
                    });
                });
            });
        }


        void ComposeForceMajeure(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // BAGIAN 10 - KEADAAN KAHAR (FORCE MAJEURE)
            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().ShowOnce().Text("10.").Style(bodyStyle);
                row.RelativeItem().Text("KEADAAN KAHAR (FORCE MAJEURE)").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).Column(list =>
            {
                list.Spacing(5);

                list.Item().PaddingLeft(25).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("10.1").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("keadaan yang termasuk dalam keadaan kahar (Force Majeure) adalah peristiwa-peristiwa sebagai berikut:").Style(bodyStyle);
                    });
                });

                string[] forceMajeureItems =
                {
                    "Bencana alam termasuk akan tetapi tidak terbatas pada gempa bumi, tanah longsor, banjir dan kebakaran;",
                    "Perang, huru-hara, pemogokan, pemberontakan, wabah/pandemi/epidemi; dan/atau",
                    "Hal-hal lain yang di luar kuasa dan kendali oleh suatu Pihak,yang berdampak langsung pada kemampuan suatu PIHAK untuk melaksanakan kewajibannya berdasarkan Perjanjian ini ('keadaan Kahar')."
                };

                list.Item().PaddingLeft(25).Column(subList =>
                {
                    subList.Spacing(5);
                    for (int i = 0; i < forceMajeureItems.Length; i++)
                    {
                        subList.Item().PaddingLeft(35).ShowOnce().Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text($"10.1.{i + 1}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(forceMajeureItems[i]).Style(bodyStyle);
                            });
                        });
                    }
                });

                list.Item().PaddingLeft(25).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("10.2").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Apabila terjadi Keadaan Kahar, Pihak yang mengalami Keadaan Kahar harus memberitahukan kepada Pihak Lainnya secara tertulis selambat-lambatnya dalam waktu 5 (lima) Hari Kerja sejak terjadinya atau diketahuinya Keadaan Kahar disertai bukti-bukti yang menunjukkan terjadinya Keadaan Kahar dan memberitahukan waktu terbaik bagi pihak yang mengalami keaadaan kabar untuk melanjutkan pelaksanaan kewajibannya berdasarkan perjanjian ini. Apabila pihak yang mengalami keadaan kahar gagal memberikan pemberitahuan terjadinya keadaan kahar dalam jangka waktu yang ditentukan tersebut, maka keadaan kahar dianggap tidak pernah terjadi dan pihak yang mengalami tetap harus melaksanakan kewajibannya berdasarkan perjanjian ini").Style(bodyStyle);
                    });
                });

                list.Item().PaddingLeft(25).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("10.3").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Pihak yang mengalami Keadaan Kahar akan dibebaskan, selama jangka waktu terjadinya Keadaan Kahar tersebut, dari pelaksanaan kewajiban-kewajibannya berdasarkan Perjanjian ini.").Style(bodyStyle);
                    });
                });

                list.Item().PaddingLeft(25).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text("10.4").Style(bodyStyle);
                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Apabila Keadaan Kahar tersebut berlangsung melebihi atau diduga akan melebihi jangka waktu 30 (tiga puluh) Hari Kalender, maka Para Pihak sepakat untuk meninjau kembali Jangka Waktu Perjanjian ini.").Style(bodyStyle);
                    });
                });
            });
        }


        void ComposeConfidentiality(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();


            _chapterNumber++;
            string sectionNumber = $"{_chapterNumber}";

            // BAGIAN 11 - KERAHASIAAN
            column.Item().PaddingTop(10).ShowOnce().Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text(sectionNumber).Style(bodyStyle);
                row.RelativeItem().Text("KERAHASIAAN").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).Column(list =>
            {
                list.Spacing(5);

                list.Item().PaddingLeft(25).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text($"{sectionNumber}.1").Style(bodyStyle);

                    row.RelativeItem().Text(text =>
                    {
                        text.Justify();
                        text.Span("Untuk tujuan perjannjian ini:").Style(bodyStyle);
                    });
                });

                // Sub-list untuk definisi (sudah menggunakan justifikasi, tidak diubah)
                list.Item().PaddingLeft(60).ShowOnce().Text(text =>
                {
                    text.Justify();
                    text.Span(" 'Informasi Rahasia' ").Style(boldBodyStyle);
                    text.Span("berarti keberadaan dan isi dari Perjanjian ini dan kesepakatan lain atau pengaturan yang terdapat dalam Perjanjian ini, dan:").Style(bodyStyle);
                });

                string[] confidentialInfoItems =
                {
                    "Informasi dalam bentuk apa pun sehubungan dengan Data Pribadi, baik umum maupun spesifik, selain informasi publik",
                    "Informasi dalam bentuk apa pun sehubungan dengan usaha (termasuk namun tidak terbatas pada, setiap informasi dan data yang berkaitan dengan keuangan, metode operasional, rencana usaha, rahasia dagang atau informasi eksklusif), kekayaan intelektual, data, urusan, hal, usaha lainnya atau tindakan Pihak Pengungkap, afiliasi, para pemegang saham, anak perusahaan, karyawan, kontraktor, direktur, pejabat, agen dari Pihak Pengungkap atau mitra usahanya atau setiap hal serupa yang mungkin diketahui Pihak Penerima dan pemberitahuan informasi tersebut oleh Pihak Pengungkap; dan",
                    "Setiap informasi yang secara tegas dimaksudkan untuk menjadi rahasia sehubungan dengan pihak pengungkap (atau afiliasinya) dari waktu ke waktu",
                };

                string[] romanNumerals = { "i", "ii", "iii", };
                list.Item().PaddingLeft(25).ShowOnce().Column(subList =>
                {
                    subList.Spacing(5);
                    for (int i = 0; i < confidentialInfoItems.Length; i++)
                    {
                        subList.Item().PaddingLeft(35).Row(row =>
                        {
                            row.ConstantItem(25).AlignTop().ShowOnce().Text($"{romanNumerals[i]}.").Style(bodyStyle);

                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(confidentialInfoItems[i]).Style(bodyStyle);
                            });
                        });
                    }
                });


                list.Item().PaddingLeft(60).ShowOnce().Text(text =>
                {
                    text.Justify();
                    text.Span("dimana Pihak Penerima mungkin dari waktu ke waktu menerima atau mendapatkan (secara lisan atau secara tertulis atau dalam bentuk elektronik) dari Pihak Pengungkap sebagai hasil dari negosiasi, penandatanganan atau pelaksanaan kewajiban-kewajibannya berdasarkan Perjanjian ini.").Style(bodyStyle);
                });
                list.Item().PaddingLeft(60).ShowOnce().ShowOnce().Text(text =>
                {
                    text.Justify();
                    text.Span("'Pihak Penerima'").Style(boldBodyStyle);
                    text.Span(" berarti setiap Pihak yang menerima Informasi Rahasia berdasarkan Perjanjian ini.").Style(bodyStyle);
                });
                list.Item().PaddingLeft(60).ShowOnce().Text(text =>
                {
                    text.Justify();
                    text.Span("'Pihak Pengungkap'").Style(boldBodyStyle);
                    text.Span(" berarti setiap Pihak yang memberikan Informasi Rahasia berdasarkan Perjanjian ini.").Style(bodyStyle);
                });
                list.Item().PaddingLeft(60).ShowOnce().Text(text =>
                {
                    text.Justify();
                    text.Span("'Perwakilan'").Style(boldBodyStyle);
                    text.Span(" berarti afiliasi, direksi, dewan komisaris, karyawan, agen, penasihat, manajemen dan sub-kontraktor dari suatu Pihak.").Style(bodyStyle);
                });

                int clauseNumber = 2;

                // Clause 11.2
                list.Item().PaddingLeft(25).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text($"{sectionNumber}.{clauseNumber++}").Style(bodyStyle);

                    row.RelativeItem().ShowOnce().Text(text =>
                    {
                        text.Justify();
                        text.Span("Pihak Penerima berjanji kepada Pihak Pengungkap bahwa, dengan tunduk pada ketentuan Pasal 10.4. Perjanjian ini, kecuali persetujuan tertulis dari Pihak Pengungkap telah diperoleh sebelumnya, Pihak Penerima wajib, dan wajib melakukan usaha terbaiknya untuk memastikan bahwa Perwakilannya, menjaga kerahasiaan dan tidak akan karena kegagalan menjalankan kehati-hatian atau karena tindakan atau kelalaian apa pun mengungkapkan kepada siapa pun, atau menggunakan atau mengeksploitasi secara komersial untuk kepentingannya sendiri, setiap Informasi Rahasia dari Pihak Pengungkap.").Style(bodyStyle);
                    });
                });

                // Clause 11.3 dengan sub-list
                list.Item().PaddingLeft(25).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text($"{sectionNumber}.{clauseNumber++}").Style(bodyStyle);

                    row.RelativeItem().ShowOnce().Text(text =>
                    {
                        text.Justify();
                        text.Span("Persetujuan yang dirujuk dalam Pasal 11.2 Perjanjian ini tidak akan disyaratkan untuk pengungkapan oleh Pihak Penerima atas setiap Informasi Rahasia:").Style(bodyStyle);
                    });
                });

                string[] confidentialityExclusions =
                {
                    "kepada Perwakilan dari Pihak Penerima, dimana dalam setiap kasus, sebagaimana dimaksud dalam Perjanjian ini atau, sepanjang dibutuhkan untuk memungkinkan Pihak Penerima untuk melaksanakan kewajiban-kewajibannya berdasarkan Perjanjian ini dan kepada siapa, dalam setiap kasus, harus diberitahukan oleh Pihak Penerima perihal kewajibannya berdasarkan Pasal ini dan Pihak Penerima wajib untuk memastikan kepatuhan yang sama atas pembatasan penggunaan Informasi Rahasia sebagaimana terdapat pada Pasal 11.2 dan Pasal 10.4. Perjanjian ini dan tunduk pada pengecualian yang sama sebagaimana terdapat dalam Pasal 10.3 Perjanjian ini;",
                    "dengan tunduk pada Pasal 10.4 Perjanjian ini, sepanjang dipersyaratkan oleh peraturan perundang-undangan yang berlaku atau oleh peraturan dari bursa efek atau otoritas dimana Pihak Penerima tunduk atau mungkin tunduk atau berdasarkan setiap perintah pengadilan atau badan peradilan atau otoritas berwenang lainnya;",
                    "Sepanjang Informasi Rahasia yang relevan berada dalam domain publik selain karena pelanggaran Perjanjian ini oleh Pihak mana pun;",
                    "yang diungkapkan kepada Pihak Penerima oleh pihak ketiga yang tidak melanggar janji atau kewajiban apa pun untuk menjaga kerahasiaan baik secara tersirat maupun tersurat;",
                    "ang dimiliki secara sah oleh Pihak Penerima sebelum mendapatkan pengungkapan dari Pihak Pengungkap;",
                    "kepada penasihat profesional dari Pihak Penerima yang terikat dengan Pihak Penerima dengan kewajiban untuk menjaga kerahasiaan yang berlaku atas setiap Informasi Rahasia yang diungkapkan kepada penasihat profesional tersebut; atau",
                    "yang dilakukan sesuai dengan ketentuan Perjanjian ini."
                };
                list.Item().PaddingLeft(25).Column(subList =>
                {
                    subList.Spacing(5);
                    for (int j = 0; j < confidentialityExclusions.Length; j++)
                    {
                        subList.Item().PaddingLeft(35).Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text($"11.3.{j + 1}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(confidentialityExclusions[j]).Style(bodyStyle);
                            });
                        });
                    }
                });

                // Clause 11.4 dengan sub-sub-list
                list.Item().PaddingLeft(25).Row(row =>
                {
                    row.ConstantItem(35).AlignTop().ShowOnce().Text($"{sectionNumber}.{clauseNumber++}").Style(bodyStyle);
                    row.RelativeItem().ShowOnce().Text(text =>
                    {
                        text.Justify();
                        text.Span("Jika Pihak Penerima diminta untuk melakukan pengungkapan berdasarkan Pasal 10.3 Perjanjian ini, Pihak Penerima harus:").Style(bodyStyle);
                    });
                });

                string[] disclosureSteps =
                {
                    "sepanjang memungkinkan, memberitahu Pihak Pengungkap segera jika Pihak Penerima mengantisipasi bahwa Pihak Penerima dapat diminta untuk mengungkapkan salah satu dari Informasi Rahasia;",
                    "berkonsultasi dengan dan mengikuti setiap arahan wajar dari Pihak Pengungkap untuk meminimalisir pengungkapan; dan",
                    "jika pengungkapan tidak dapat dihindari :"
                };

                list.Item().PaddingLeft(25).ShowOnce().Column(subList =>
                {
                    subList.Spacing(5);
                    for (int j = 0; j < disclosureSteps.Length; j++)
                    {
                        subList.Item().PaddingLeft(35).ShowOnce().Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text($"{sectionNumber}.4.{j + 1}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(disclosureSteps[j]).Style(bodyStyle);
                            });
                        });
                    }
                });

                list.Item().PaddingLeft(95).Column(subSubList =>
                {
                    subSubList.Spacing(5);
                    subSubList.Item().Row(row =>
                    {
                        row.ConstantItem(25).AlignTop().ShowOnce().Text($"i.").Style(bodyStyle);
                        row.RelativeItem().Text(text =>
                        {
                            text.Justify();
                            text.Span("hanya mengungkapkan Informasi Rahasia sepanjang diperlukan untuk kepatuhan; dan").Style(bodyStyle);
                        });
                    });
                    subSubList.Item().Row(row =>
                    {
                        row.ConstantItem(25).AlignTop().ShowOnce().Text($"ii.").Style(bodyStyle);
                        row.RelativeItem().Text(text =>
                        {
                            text.Justify();
                            text.Span("melakukan upaya yang wajar untuk memastikan bahwa Informasi Rahasia yang diungkapkan dijaga kerahasiaannya.").Style(bodyStyle);
                        });
                    });
                });

                // Sisa klausa
                string[] remainingClauses =
                {
                    "Segera setelah dimintakan oleh Pihak Pengungkap, Pihak Penerima wajib, atas biaya dan pengeluaran Pihak Pengungkap, mengembalikan kepada Pihak Pengungkap, atau jika diminta oleh Pihak Pengungkap, memusnahkan atau memerintahkan agar dimusnahkannya, seluruh Informasi Rahasia termasuk kekayaan intelektual, dan seluruh atau sebagian salinan daripadanya yang berada dalam kepemilikannya serta seluruh catatan, pesan dalam surat elektronik, ringkasan, analisis, laporan dan dokumen lain, data, materi atau data cadangan (baik fisik atau elektronik) yang dibuat atau yang diketahui Pihak Penerima kapanpun selama dan sehubungan dengan Perjanjian ini.",
                    "Pihak Penerima akan memastikan bahwa Perwakilan akan menandatangani suatu perjanjian kerahasiaan atau perjanjian lain yang secara substansial serupa dengan ketentuan kerahasiaan yang terdapat dalam Perjanjian ini.",
                    "Pihak Penerima memahami bahwa tidak ada dalam Perjanjian yang menciptakan suatu kewajiban hukum apa pun dalam bentuk apa pun yang mengharuskan diungkapkannya setiap Informasi Rahasia, kecuali untuk kewajiban-kewajiban yang diatur dalam Perjanjian.",
                    "Pihak Pengungkap bertanggung jawab untuk memastikan kebenaran dan keakuratan dari kelengkapan setiap Informasi Rahasia yang dibuat tersedia bagi Pihak Penerima, Perwakilannya dan/atau para penasihatnya.",
                    "Ketentuan kerahasiaan yang terdapat dalam Pasal 11 Perjanjian ini akan tetap berlaku setelah pengakhiran atau berakhirnya Perjanjian karena alasan apa pun."
                };

                for (int i = 0; i < remainingClauses.Length; i++)
                {
                    list.Item().PaddingLeft(25).Row(row =>
                    {
                        row.ConstantItem(35).AlignTop().ShowOnce().Text($"{sectionNumber}.{clauseNumber++}").Style(bodyStyle);
                        row.RelativeItem().Text(text =>
                        {
                            text.Justify();
                            text.Span(remainingClauses[i]).Style(bodyStyle);
                        });
                    });
                }
            });
        }


        void ComposePersonalData(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // BAGIAN 12 - DATA PRIBADI
            column.Item().PaddingTop(10).ShowOnce().Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("12").Style(bodyStyle);
                row.RelativeItem().Text("DATA PRIBADI").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).ShowOnce().Column(list =>
            {
                list.Spacing(5);

                void AddItem(string number, Action<TextDescriptor> contentBuilder)
                {
                    list.Item().PaddingLeft(25).ShowOnce().Row(row =>
                    {
                        row.ConstantItem(35).ShowOnce().AlignTop().ShowOnce().Text(number).Style(bodyStyle);
                        row.RelativeItem().Text(contentBuilder);
                    });
                }

                AddItem("12.1", text =>
                {
                    text.Justify();
                    text.Span("Dalam menjalankan kewajiban-kewajiban berdasarkan Perjanjian ini, Para Pihak wajib mematuhi ketentuan peraturan perundang-undangan, kode etik dan kebijakan industri, sehubungan dengan perlindungan Data Pribadi, termasuk namun tidak terbatas pada ketentuan peraturan perundang-undangan mengenai perlindungan Data Pribadi.").Style(bodyStyle);
                });

                AddItem("12.2", text =>
                {
                    text.Justify();
                    text.Span("Masing-masing pihak wajib:").Style(bodyStyle);
                });

                string?[] personalDataObligations =
                {
                    "Memperoleh persetujuan yang sah secara eksplisit dalam bentuk tertulis dari subjek Data Pribadi mengenai pemrosesan Data Pribadi untuk tujuan pelaksanaan Perjanjian, serta wajib menyampaikan tujuan pemrosesan Data Pribadi kepada para subjek Data Pribadi.",
                    "Melakukan pengawasan terhadap setiap Perwakilan dan/atau pihak ketiga lain yang terlibat dalam pemrosesan Data Pribadi di bawah kendali masing-masing Pihak dalam pelaksanaan Perjanjian.",
                    "Menjaga kerahasiaan Data Pribadi atas segala bentuk komunikasi antara Pihak Kedua dan Pihak Pertama dalam media apa pun.",
                    "Dalam hal salah satu Para Pihak hendak melibatkan pihak ketiga dalam pemrosesan Data Pribadi, maka Pihak tersebut wajib terlebih dahulu mendapatkan persetujuan tertulis dari Pihak lainnya, dan memastikan bahwa asuradur dan/atau pihak ketiga tersebut akan melakukan pemrosesan Data Pribadi berdasarkan perintah masing-masing Pihak.",
                    "Jika Terdapat permintaan dari salah satu Pihak untuk mengembalikan, menghancurkan dan/atau menghapus seluruh dokumen, catatan, dan materi lainnya yang mengandung Data Pribadi yang berada di bawah kendali salah satu Pihak, maka pengembalian, penghancuran, dan/atau penghapusan dokumen wajib dilakukan dalam jangka waktu 10 (sepuluh) Hari Kerja sejak permintaan diterima. Pihak yang melakukan pengembalian, penghancuran, dan/atau penghapusan wajib memberikan kepada Pihak lainnya suatu dokumentasi yang menunjukkan bahwa pengembalian, penghancuran, dan/atau penghapusan telah dilakukan;",
                    "Mengembalikan, menghancurkan, dan/atau menghapus seluruh dokumen, catatan, dan materi lainnya yang mengandung Data Pribadi yang berada di bawah kendali salah satu Pihak sesuai dengan ketentuan berikut:",
                    null,
                    "Menyampaikan pemberitahuan tertulis kepada Pihak lainnya, sesegera mungkin akan tetapi tidak lebih dari 48 (empat puluh delapan) jam setelah terjadinya suatu tindakan pelanggaran Data Pribadi oleh salah satu Pihak, baik yang disengaja maupun tidak disengaja, termasuk akses, pengolahan, perusakan, penghapusan, penghilangan, perubahan, pengungkapan atau penggunaan Data Pribadi tanpa izin yang dilakukan atas nama Pihak tersebut dan/atau Perwakilannya, atau tindakan mencurigakan yang dilakukan dalam pengelolaan Data Pribadi (termasuk tapi tidak terbatas pada akses rutin yang tidak biasa pada Data Pribadi oleh pegawai atau karyawan dari masing-masing Pihak) atau pelanggaran lainnya dari kewajiban perlindungan Data Pribadi sesuai dengan Perjanjian ini atau penegakan hukum terhadapnya sesuai dengan peraturan perundang-undangan mengenai perlindungan Data Pribadi."
                };

                list.Item().PaddingLeft(25).Column(subList =>
                {
                    subList.Spacing(5);

                    // 1. Buat variabel penghitung terpisah, dimulai dari 1
                    int itemCounter = 1;

                    for (int i = 0; i < personalDataObligations.Length; i++)
                    {
                        if (personalDataObligations[i] != null)
                        {
                            subList.Item().PaddingLeft(35).Row(row =>
                            {
                                // 2. Gunakan 'itemCounter++' untuk penomoran
                                row.ConstantItem(35).ShowOnce().Text($"12.2.{itemCounter++}").Style(bodyStyle);
                                row.RelativeItem().Text(text =>
                                {
                                    text.Justify();
                                    text.Span(personalDataObligations[i]).Style(bodyStyle);
                                });
                            });
                        }
                        else
                        {

                            subList.Item().PaddingLeft(35).Column(subSubList =>
                            {
                                subSubList.Spacing(5);
                                subSubList.Item().PaddingLeft(35).Row(row =>
                                {
                                    row.ConstantItem(25).ShowOnce().Text("i.").Style(bodyStyle);
                                    row.RelativeItem().Text(text =>
                                    {
                                        text.Justify();
                                        text.Span("Dalam jangka waktu 30 (tiga puluh) Hari Kalender terhitung sejak Data Pribadi tidak lagi diperlukan untuk tujuan pelaksanaan Perjanjian ini; dan/atau").Style(bodyStyle);
                                    });
                                });
                                subSubList.Item().PaddingLeft(35).Row(row =>
                                {
                                    row.ConstantItem(25).ShowOnce().Text("ii.").Style(bodyStyle);
                                    row.RelativeItem().Text(text =>
                                    {
                                        text.Justify();
                                        text.Span("Dalam jangka waktu 30 (tiga puluh) Hari Kalender sejak tanggal pengakhiran Perjanjian ini;").Style(bodyStyle);
                                    });
                                });
                                subSubList.Item().PaddingLeft(35).Text(text =>
                                {
                                    text.Justify();
                                    text.Span("dan menyediakan keterangan penghancuran atau penghapusan tersebut kepada Pihak lainnya; dan").Style(bodyStyle);
                                });
                            });
                        }
                    }
                });

                AddItem("12.3", text =>
                {
                    text.Justify();
                    text.Span("Untuk menghindari keraguan, Pihak Kedua wajib mendapatkan persetujuan yang sah secara eksplisit dalam bentuk tertulis dari subjek Data Pribadi untuk tujuan pemrosesan Data Pribadi oleh Pihak Pertama sebagai berikut:").Style(bodyStyle);
                });

                string[] dataPurposes =
                {
                    "Penyediaan layanan asuransi oleh Pihak Pertama, termasuk namun tidak terbatas pada penilaian risiko asuransi, layanan konsumen dan pengaduan, serta penyediaan layanan asuransi melalui media elektronik oleh Pihak Pertama;",
                    "Penyediaan layanan asuransi melalui kerja sama dengan pihak ketiga dan/atau melalui media elektronik, termasuk platform layanan kesehatan oleh pihak lain yang ditunjuk oleh Pihak Pertama; dan",
                    "Proses administrasi dan pembayaran layanan kesehatan."
                };

                list.Item().PaddingLeft(25).Column(subList =>
                {
                    subList.Spacing(5);
                    for (int i = 0; i < dataPurposes.Length; i++)
                    {
                        subList.Item().PaddingLeft(35).Row(row =>
                        {
                            row.ConstantItem(35).ShowOnce().Text($"12.3.{i + 1}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(dataPurposes[i]).Style(bodyStyle);
                            });
                        });
                    }
                });

                AddItem("12.4", text =>
                {
                    text.Justify();
                    text.Span("Dalam hal masing-masing Pihak melakukan transfer Data Pribadi kepada pihak lain, baik di dalam dan/atau keluar wilayah Indonesia, wajib dilakukan dengan menerapkan perlindungan data sesuai peraturan perundang-undangan yang berlaku. Apabila tingkat perlindungan Data Pribadi di negara penerima transfer Data Pribadi tersebut dianggap tidak memadai atau tidak dapat memenuhi ketentuan minimum perlindungan Data Pribadi di Indonesia, masing-masing Pihak wajib menerapkan tingkat perlindungan Data Pribadi setidaknya memenuhi ketentuan minimum perlindungan Data Pribadi di Indonesia. Pihak yang melakukan transfer Data Pribadi akan menandatangani perjanjian tertulis dengan pihak yang menerima transfer Data Pribadi untuk menjamin penerapan perlindungan data yang setara seperti yang dipersyaratkan oleh peraturan perundang-undangan. Sebelum melakukan transfer Data Pribadi, masing-masing Pihak wajib terlebih dahulu mendapatkan persetujuan dari otoritas yang berwenang (apabila dipersyaratkan oleh peraturan perundang-undangan yang berlaku).").Style(bodyStyle);
                });

                AddItem("12.5", text =>
                {
                    text.Justify();
                    text.Span("Apabila subjek Data Pribadi membuat suatu permintaan tertulis untuk akses terhadap Data Pribadi yang relevan, suatu Pihak wajib memberitahu Pihak lainnya secara langsung (jika permintaan telah ditujukan oleh subjek Data Pribadi hanya kepada salah satu Pihak) dan, tunduk pada instruksi dari Pihak yang menerima permintaan dari subjek Data Pribadi, Pihak lainnya wajib menyediakan rincian Data Pribadi yang dikendalikan oleh Pihak lainnya tersebut dalam jangka waktu 30 (tiga puluh) Hari Kalender setelah penerimaannya atas permintaan akses terhadap Data Pribadi.").Style(bodyStyle);
                });
            });
        }


        void ComposeAntiBribery(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // BAGIAN 13 - KETENTUAN SISTEM MANAJEMEN ANTI PENYUAPAN
            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("13").Style(bodyStyle);
                row.RelativeItem().Text("KETENTUAN SISTEM MANAJEMEN ANTI PENYUAPAN").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).Column(list =>
            {
                list.Spacing(5);

                void AddItem(string number, Action<TextDescriptor> contentBuilder)
                {
                    list.Item().PaddingLeft(25).Row(row =>
                    {
                        row.ConstantItem(35).ShowOnce().AlignTop().Text(number).Style(bodyStyle);
                        row.RelativeItem().Text(contentBuilder);
                    });
                }

                AddItem("13.1", text =>
                {
                    text.Justify();
                    text.Span("Masing-masing Pihak berjanji tidak akan, dan akan menyebabkan Perwakilannya tidak akan, menawarkan, menjanjikan, memberikan, menerima, maupun meminta keuntungan yang tidak semestinya, dalam bentuk uang tunai maupun non-tunai, secara langsung maupun tidak langsung, yang melanggar peraturan perundang-undangan, serta tidak akan menawarkan atau memberikan suap, gratifikasi atau uang pelicin dalam bentuk apa pun sebagai bujukan atau hadiah kepada karyawan maupun pejabat Pihak lainnya untuk mendapatkan berbagai bentuk manfaat dan/atau kemudahan sehubungan dengan Perjanjian ini.").Style(bodyStyle);
                });

                AddItem("13.2", text =>
                {
                    text.Justify();
                    text.Span("Para Pihak berkomitmen untuk menerapkan Anti-Fraud, Pengendalian Gratifikasi dan Anti Penyuapan guna mendukung pemberantasan korupsi di lingkungan Para Pihak dengan menjalankan usaha di atas nilai integritas, berpedoman pada kode etik dan prinsip sebagai berikut:").Style(bodyStyle);
                });

                string[] antiBriberyPrinciples =
                {
                    "No Bribery (tidak boleh ada suap menyuap dan pemerasan);",
                    "No Kickback (tidak boleh ada komisi, tanda terima kasih baik dalam bentuk uang dan dalam bentuk lainnya);",
                    "No Gift (tidak boleh ada hadiah atau gratifikasi yang bertentangan dengan peraturan dan ketentuan yang berlaku);",
                    "No Luxurious Hospitality (tidak boleh ada penyambutan dan jamuan yang berlebihan)"
                };

                list.Item().PaddingLeft(25).Column(subList =>
                {
                    subList.Spacing(5);
                    for (int i = 0; i < antiBriberyPrinciples.Length; i++)
                    {
                        subList.Item().PaddingLeft(35).Row(row =>
                        {
                            row.ConstantItem(35).ShowOnce().Text($"13.2.{i + 1}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(antiBriberyPrinciples[i]).Style(bodyStyle);
                            });
                        });
                    }

                    subList.Item().PaddingLeft(35).Text(text =>
                    {
                        text.Justify();
                        text.Span("untuk selanjutnya disebut sebagai ").Style(bodyStyle);
                        text.Span("'Prinsip 4 No's'").Style(boldBodyStyle);
                    });
                });

                AddItem("13.3", text =>
                {
                    text.Justify();
                    text.Span("Para Pihak berkomitmen untuk selalu berupaya meningkatkan dan memperbaiki secara berkelanjutan Sistem Manajemen Anti Penyuapan pada setiap proses bisnis agar sejalan dengan prinsip-prinsip Good Corporate Governance (GCG), pedoman perilaku dan etika bisnis perusahaan.").Style(bodyStyle);
                });

                AddItem("13.4", text =>
                {
                    text.Justify();
                    text.Span("Para Pihak dengan ini menyatakan akan menjalankan prinsip zero tolerance terhadap tindakan yang berkaitan dengan pelanggaran peraturan perundang-undangan.").Style(bodyStyle);
                });

                AddItem("13.5", text =>
                {
                    text.Justify();
                    text.Span("Para Pihak tidak akan, dalam bentuk apa pun, memperkenankan Karyawan dan stakeholder Perusahaan untuk menggunakan kode etik perusahaan dan Prinsip 4 No’s dalam menjalankan tugasnya di Perusahaan.").Style(bodyStyle);
                });

                AddItem("13.6", text =>
                {
                    text.Justify();
                    text.Span("Para Pihak wajib menghindari konflik kepentingan dan mengelola setiap konflik kepentingan yang menimbulkan risiko fraud.").Style(bodyStyle);
                });

                AddItem("13.7", text =>
                {
                    text.Justify();
                    text.Span("Para Pihak wajib menghindari konflik kepentingan dan mengelola setiap konflik kepentingan yang menimbulkan risiko fraud.").Style(bodyStyle);
                });

                AddItem("13.8", text =>
                {
                    text.Justify();
                    text.Span("Para Pihak wajib melakukan pengawasan terhadap pelaksanaan komitmen Sistem Manajemen Anti Penyuapan dan setiap pelanggaran akan dikenai sanksi sesuai peraturan perusahaan dan perundang-undangan yang berlaku.").Style(bodyStyle);
                });

                AddItem("13.9", text =>
                {
                    text.Justify();
                    text.Span("Para Pihak akan mendukung penerapan ISO 37001:2016 Sistem Manajemen Anti Penyuapan (SMAP).").Style(bodyStyle);
                });

                AddItem("13.10", text =>
                {
                    text.Justify();
                    text.Span("Para pihak wajib mematuhi dan melaksanakan Komitmen Sistem Manajemen Anti Penyuapan dengan sungguh-sungguh.").Style(bodyStyle);
                });

                AddItem("13.11", text =>
                {
                    text.Justify();
                    text.Span("Apabila salah satu pihak terbukti melanggar ketentuan dalam pasal ini, maka Para Pihak sepakat untuk menghentikan/mengakhiri Perjanjian ini, dan pihak yang melakukan pelanggaran akan diberikan sanksi sesuai dengan ketentuan dan peraturan perundang-undangan yang berlaku.").Style(bodyStyle);
                });
            });
        }


        void ComposeAML(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("14").Style(bodyStyle);
                row.RelativeItem().Text("PRINSIP MENGENAL NASABAH DAN PENERAPAN PROGRAM ANTI PENCUCIAN UANG DAN PENCEGAHAN PENDANAAN TERORISME DAN PENCEGAHAN PENDANAAN PROLIFERASI SENJATA PEMUSNAH MASSAL").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).Column(list =>
            {
                list.Spacing(5);

                void AddItem(string number, Action<TextDescriptor> contentBuilder)
                {
                    list.Item().PaddingLeft(25).Row(row =>
                    {
                        row.ConstantItem(35).ShowOnce().AlignTop().Text(number).Style(bodyStyle);
                        row.RelativeItem().Text(contentBuilder);
                    });
                }

                AddItem("14.1", text =>
                {
                    text.Justify(); text.Span("Pihak Kedua mengakui dan memahami bahwa Pihak Pertama adalah suatu lembaga jasa keuangan yang berdasarkan ketentuan dan peraturan perundang-undangan yang berlaku wajib menerapkan ketentuan mengenai Customer Due Diligence sebagai bentuk penerapan program Anti Pencucian Uang dan Pencegahan Pendanaan Terorisme dan Pencegahan Proliferasi Senjata Pemusnah Massal. Sehubungan dengan hal tersebut, Pihak Kedua bersedia menyampaikan dan segera melaporkan kepada Pihak Pertama data, informasi, dokumen, serta setiap perubahan data Peserta yang menerima pelayanan kesehatan dari Pihak Kedua.\n").Style(bodyStyle);
                    text.Span("Untuk menghindari keraguan, ").Style(bodyStyle);
                    text.Span("'Customer Due Diligence'").Style(boldBodyStyle);
                    text.Span(" berarti kegiatan identifikasi, verifikasi, dan pemantauan yang dilakukan oleh Pihak Pertama untuk memastikan transaksi sesuai dengan profil, karakteristik, dan/atau pola transaksi Peserta.").Style(bodyStyle);
                });

                AddItem("14.2", text =>
                {
                    text.Justify();
                    text.Span("Pihak Kedua dengan ini berjanji untuk bekerja sama dengan Pihak Pertama termasuk untuk menyediakan setiap data, informasi serta dokumen yang dibutuhkan Pihak Pertama dalam rangka pemenuhan ketentuan dan peraturan perundang-undangan di bidang Anti Pencucian Uang dan Pencegahan Pendanaan Terorisme dan Pencegahan Proliferasi Senjata Pemusnah Massal.").Style(bodyStyle);
                });
            });
        }


        void ComposeTermination(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // BAGIAN 15 - PENGAKHIRAN PERJANJIAN
            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("15").Style(bodyStyle);
                row.RelativeItem().Text("PENGAKHIRAN PERJANJIAN").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).Column(list =>
            {
                list.Spacing(5);

                void AddItem(string number, Action<TextDescriptor> contentBuilder)
                {
                    list.Item().PaddingLeft(25).Row(row =>
                    {
                        row.ConstantItem(35).ShowOnce().AlignTop().Text(number).Style(bodyStyle);
                        row.RelativeItem().Text(contentBuilder);
                    });
                }

                AddItem("15.1", text =>
                {
                    text.Justify();
                    text.Span("Para Pihak dapat mengakhiri Perjanjian ini sebelum berakhirnya Jangka Waktu berdasarkan suatu kesepakatan tertulis antara Para Pihak.").Style(bodyStyle);
                });

                AddItem("15.2", text =>
                {
                    text.Justify();
                    text.Span("Dengan tunduk pada ketentuan pengakhiran, salah satu Pihak dapat mengakhiri Perjanjian ini sebelum berakhirnya Jangka Waktu, dengan ketentuan bahwa pengakhiran Perjanjian akan berlaku efektif secara seketika pada tanggal surat pemberitahuan pengakhiran Perjanjian ini dari suatu Pihak kepada Pihak lainnya, dengan alasan sebagai berikut:").Style(bodyStyle);
                });

                string[] terminationReasons =
                {
                    "Karena wanprestasi sebagaimana diatur dalam Ketentuan Umum Pasal 6;",
                    "Telah diajukannya permohonan Penundaan Kewajiban Pembayaran Utang (PKPU), insolvensi, kepailitan, atau dikeluarkannya pernyataan untuk membubarkan perusahaan dalam bentuk apa pun kepada salah satu Pihak; dan/atau",
                    "Pihak Pertama memutuskan untuk mengakhiri Perjanjian ini karena Pihak Kedua gagal memenuhi persyaratan berdasarkan Peninjauan yang dilakukan oleh Pihak Pertama sebagaimana diatur dalam Ketentuan Umum Pasal 2."
                };

                list.Item().PaddingLeft(25).Column(subList =>
                {
                    subList.Spacing(5);
                    for (int i = 0; i < terminationReasons.Length; i++)
                    {
                        subList.Item().PaddingLeft(35).Row(row =>
                        {
                            row.ConstantItem(35).ShowOnce().Text($"15.2.{i + 1}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span(terminationReasons[i]).Style(bodyStyle);
                            });
                        });
                    }
                });

                AddItem("15.3", text =>
                {
                    text.Justify();
                    text.Span("Pengakhiran Perjanjian ini tidak akan membebaskan suatu Pihak dari kewajiban yang telah ada sebelum pengakhiran Perjanjian ini.").Style(bodyStyle);
                });
            });
        }


        void ComposeCorrespondence(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // BAGIAN 16 - KORESPONDENSI
            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("16").Style(bodyStyle);
                row.RelativeItem().Text("KORESPONDENSI").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).Column(list =>
            {
                list.Spacing(5);

                void AddItem(string number, Action<TextDescriptor> contentBuilder)
                {
                    list.Item().PaddingLeft(25).Row(row =>
                    {
                        row.ConstantItem(35).ShowOnce().AlignTop().Text(number).Style(bodyStyle);
                        row.RelativeItem().Text(contentBuilder);
                    });
                }

                AddItem("16.1", text =>
                {
                    text.Justify();
                    text.Span("Seluruh pemberitahuan (termasuk persetujuan dan permintaan persetujuan atau komunikasi lain) sehubungan dengan Perjanjian ini wajib dibuat secara tertulis dan dikirimkan melalui surat elektronik (email) kepada atau surat tercatat yang dikirimkan melalui kurir atau layanan kurir dengan mendapatkan tanda terima yang ditandatangani oleh penerima pada alamat korespondensi yang terdapat pada Ketentuan Khusus.").Style(bodyStyle);
                });

                AddItem("16.2", text =>
                {
                    text.Justify();
                    text.Span("Semua pemberitahuan akan berlaku efektif segera setelah diterimanya, sebagaimana dibuktikan dengan sebuah pernyataan tanda terima yang ditandatangani oleh seorang dewasa pada alamat penerima yang diberikan berdasarkan Perjanjian ini, namun, dalam hal pemberitahuan dikirim melalui surat elektronik (email), segera setelah penerima oleh pengirim atas sebuah laporan tanda terima atau pengiriman yang dihasilkan oleh mesin pengirim pesan dan yang mengindikasikan bahwa pesan telah seluruhnya berhasil dikirimkan kepada penerima. Pihak mana pun dapat mengubah alamatnya dengan memberikan pemberitahuan tertulis kepada Pihak yang lain, dengan ketentuan bahwa masing-masing Pihak diwajibkan untuk, pada setiap saat, memiliki suatu alamat ke mana pemberitahuan dapat dikirimkan.").Style(bodyStyle);
                });
            });
        }


        void ComposeGoverningLaw(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // BAGIAN 17 - HUKUM YANG BERLAKU DAN PENYELESAIAN SENGKETA
            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("17").Style(bodyStyle);
                row.RelativeItem().Text("HUKUM YANG BERLAKU DAN PENYELESAIAN SENGKETA").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).ShowOnce().Column(list =>
            {
                list.Spacing(5);

                void AddItem(string number, Action<TextDescriptor> contentBuilder)
                {
                    list.Item().PaddingLeft(25).Row(row =>
                    {
                        row.ConstantItem(35).ShowOnce().AlignTop().Text(number).Style(bodyStyle);
                        row.RelativeItem().Text(contentBuilder);
                    });
                }

                AddItem("17.1", text =>
                {
                    text.Justify();
                    text.Span("Perjanjian ini diatur dan ditafsirkan berdasarkan hukum negara Republik Indonesia.").Style(bodyStyle);
                });

                AddItem("17.2", text =>
                {
                    text.Justify();
                    text.Span("Setiap sengketa yang timbul dari atau sehubungan dengan Perjanjian ini, termasuk setiap pertanyaan mengenai keberadaan, keabsahan atau pengakhiran Perjanjian ini, atau setiap klaim sehubungan dengan pelanggaran Perjanjian ini ").Style(bodyStyle);
                    text.Span("(“Sengketa”)").Style(boldBodyStyle);
                    text.Span(" akan diselesaikan secara damai melalui musyawarah mufakat antara Para Pihak.").Style(bodyStyle);
                });

                AddItem("17.3", text =>
                {
                    text.Justify();
                    text.Span("Apabila Sengketa tidak dapat diselesaikan melalui musyawarah mufakat dalam waktu 30 (tiga puluh) Hari Kalender sejak tanggal pemberitahuan oleh salah satu Pihak yang mengadakan musyawarah tersebut, maka setiap Pihak dapat merujuk penyelesaian Sengketa tersebut pada pengadilan negeri.").Style(bodyStyle);
                });

                AddItem("17.4", text =>
                {
                    text.Justify();
                    text.Span("Untuk keperluan Perjanjian ini berikut akibat-akibat dan pelaksanaan atasnya, Para Pihak telah memilih domisili hukum mereka yang umum dan tetap di Kantor Panitera Pengadilan Negeri Jakarta Selatan.").Style(bodyStyle);
                });
            });
        }


        void ComposeMiscellaneous(ColumnDescriptor column)
        {
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();

            // BAGIAN 18 - LAIN - LAIN
            column.Item().PaddingTop(10).ShowOnce().Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("18").Style(bodyStyle);
                row.RelativeItem().Text("LAIN - LAIN").Style(boldBodyStyle);
            });

            column.Item().PaddingTop(5).ShowOnce().Column(list =>
            {
                list.Spacing(5);

                void AddItem(string number, string content)
                {
                    list.Item().PaddingLeft(25).ShowOnce().Row(row =>
                    {
                        row.ConstantItem(35).AlignTop().ShowOnce().Text(number).Style(bodyStyle);
                        row.RelativeItem().Text(text =>
                        {
                            text.Justify();
                            text.Span(content).Style(bodyStyle);
                        });
                    });
                }

                AddItem("18.1", "Pelaksanaan Perjanjian ini termasuk tetapi tidak terbatas pada korespondensi sehubungan dengan Perjanjian ini wajib dilakukan dalam Bahasa Indonesia.");
                AddItem("18.2", "Perjanjian ini merupakan keseluruhan perjanjian antara Para Pihak dalam Perjanjian ini dan menggantikan seluruh perjanjian dan pemahaman sebelumnya, baik lisan maupun tertulis, di antara seluruh Para Pihak dalam Perjanjian ini sehubungan dengan pokok materi Perjanjian ini. Tidak ada pernyataan, tawaran, janji, pemahaman, kondisi atau jaminan yang tidak diatur di dalam Perjanjian ini telah dibuat atau dipercaya oleh Pihak mana pun.");
                AddItem("18.3", "Perjanjian ini harus bertujuan untuk kepentingan dan mengikat bagi Para Pihak dalam Perjanjian ini dan penerus, perwakilan hukum dan pihak yang ditunjuk dari masing-masing Pihak. Tidak ada dalam Perjanjian ini, baik secara tersurat maupun tersirat yang bermaksud untuk memberikan kepada orang lain selain Para Pihak dalam Perjanjian ini, dan penerus, perwakilan hukum dan pihak yang ditunjuk dari masing-masing Pihak, segala hak, pemulihan, kewajiban atau tanggung jawab dalam Perjanjian ini.");
                AddItem("18.4", "Para Pihak dengan ini sepakat untuk, masing-masing, menanggung setiap biaya dan ongkos sehubungan dengan pelaksanaan Perjanjian ini.");
                AddItem("18.5", "Tidak ada Pihak yang dapat mengalihkan atau menyerahkan setiap hak atau kewajibannya berdasarkan Perjanjian ini kepada pihak lain mana pun, kecuali karena hukum atau perintah pengadilan, tanpa persetujuan tertulis sebelumnya dari seluruh Pihak lainnya.");
                AddItem("18.6", "Setiap perubahan atau pengesampingan atas, atau persetujuan yang diberikan berdasarkan, ketentuan dalam Perjanjian ini harus dibuat secara tertulis dan, dalam hal terdapat perubahan, harus ditandatangani oleh seluruh Pihak dalam Perjanjian ini atau dalam hal pengesampingan, harus ditandatangani oleh Pihak lainnya selain yang melakukan pengesampingan tersebut.");
                AddItem("18.7", "Apabila salah satu atau sebagian dari ketentuan atau janji dalam Perjanjian ini menjadi tidak berlaku, tidak sah atau tidak dapat diberlakukan karena alasan apa pun, maka ketentuan atau janji lainnya tetap akan berlaku, sah dan dapat diberlakukan. Para Pihak akan berupaya sebaik mungkin untuk mencapai tujuan dari ketentuan atau janji yang menjadi tidak berlaku, tidak sah atau tidak dapat diberlakukan tersebut dengan menggantinya dengan ketentuan atau janji yang berlaku dan sah secara hukum.");
                AddItem("18.8", "Perjanjian ini dapat ditandatangani dalam beberapa rangkap, masing-masing akan dianggap sebagai dokumen asli dan memiliki kekuatan hukum yang sama, serta tanda tangan tersebut akan dianggap memiliki kekuatan hukum seolah-olah dicantumkan pada satu instrumen yang sama.");

                list.Item().PaddingTop(10).Text(text =>
                {
                    text.AlignCenter();
                    text.Span("Ketentuan Khusus Perjanjian pada Halaman Berikutnya").Style(bodyStyle.Italic());
                });
            });
        }


        void ComposeAllDefinitions(ColumnDescriptor column)
        {


            var titleStyle = TextStyle.Default.FontSize(12).Bold();
            var bodyStyle = TextStyle.Default.FontSize(10);
            var boldBodyStyle = TextStyle.Default.FontSize(10).Bold();
            var italicBodyStyle = bodyStyle.Italic();

            var definitions = GetDefinitions();
            var point1Definitions = definitions.Where(d => d.Term1.Length > 1 && !d.Term1.StartsWith("2.")).ToList();
            var point2Definitions = definitions.Where(d => d.Term1.StartsWith("2.")).ToList();

            // Section I. DEFINISI DAN INTERPRETASI
            column.Item().ShowOnce().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("I.").Style(boldBodyStyle);
                row.RelativeItem().Text("DEFINISI DAN INTERPRETASI").Style(titleStyle);
            });

            // Section 1. Definisi
            column.Item().ShowOnce().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("1.").Style(bodyStyle);
                row.RelativeItem().Text(text =>
                {
                    text.Justify();
                    text.Span("Dalam Perjanjian ini, kecuali dalam konteksnya memerlukan pengertian lain, ungkapan-ungkapan dan kata-kata sebagai berikut mempunyai arti sebagaimana disebutkan di bawah ini :").Style(bodyStyle);
                });
            });

            column.Item().ShowOnce().PaddingTop(10).PaddingLeft(25).Column(defs =>
            {
                defs.Spacing(10);
                int itemNumber = 1;
                foreach (var definition in point1Definitions)
                {
                    if (definition.Term1 == "Provider")
                    {
                        defs.Item().Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text($"1.{itemNumber++}").Style(bodyStyle);
                            row.RelativeItem().Column(subColumn =>
                            {
                                subColumn.Item().Text(text =>
                                {
                                    text.Justify();
                                    text.Span("Provider").Style(boldBodyStyle);
                                    text.Span(" adalah sarana/fasilitas pelayanan kesehatan yang bekerja sama dengan Pihak Pertama untuk memberikan pelayanan kesehatan bagi Peserta berdasarkan tingkat pelayanan yang diberikan. Provider terbagi atas:").Style(bodyStyle);
                                });
                                subColumn.Item().PaddingTop(5).PaddingLeft(2).Row(subRow =>
                                {
                                    subRow.ConstantItem(20).AlignTop().ShowOnce().Text("1)").Style(bodyStyle);
                                    subRow.RelativeItem().Text(text =>
                                    {
                                        text.Justify();
                                        text.Span("Provider Tingkat Pertama").Style(boldBodyStyle);
                                        text.Span(": sarana pelayanan kesehatan yang memberikan pelayanan kesehatan dasar/umum non spesialistik dan mengutamakan pelayanan promotif dan preventif seperti balai pengobatan, dokter keluarga, klinik dan puskesmas yang bekerjasama dengan Pihak Pertama dengan Pihak Pertama atau sekaligus bekerjasama dengan BPJS Kesehatan; dan").Style(bodyStyle);
                                    });
                                });
                                subColumn.Item().PaddingTop(5).PaddingLeft(2).Row(subRow =>
                                {
                                    subRow.ConstantItem(20).AlignTop().ShowOnce().Text("2)").Style(bodyStyle);
                                    subRow.RelativeItem().Text(text =>
                                    {
                                        text.Justify();
                                        text.Span("Provider Tingkat Lanjutan").Style(boldBodyStyle);
                                        text.Span(": sarana pelayanan kesehatan yang memberikan pelayanan kesehatan spesialis dan subspesialis untuk keperluan observasi, diagnosis, pengobatan, Rehabilitasi Medis, dan/atau pelayanan medis lainnya baik pelayanan rawat jalan maupun rawat inap yang meliputi praktik dokter spesialis, klinik spesialis, dan Rumah Sakit yang bekerjasama dengan Pihak Pertama dengan Pihak Pertama atau sekaligus bekerjasama dengan BPJS Kesehatan.").Style(bodyStyle);
                                    });
                                });
                            });
                        });
                    }
                    else if (definition.Term1 == "Data Pribadi")
                    {
                        defs.Item().Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text($"1.{itemNumber++}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span($"{definition.Term1}").Style(boldBodyStyle);
                                text.Span(" adalah setiap data atau informasi (baik tertulis, oral, digital atau dalam bentuk apapun) yang benar dan nyata yang melekat dan dapat diidentifikasi berdasarkan dokumen identitas Peserta, baik langsung maupun tidak langsung, pada masing-masing Peserta, termasuk namun tidak terbatas pada nama, tempat lahir, tanggal lahir, alamat, nomor kontak, nama ibu kandung, data simpanan, data kartu kredit, data pinjaman pada setiap institusi keuangan, dan/atau data-data lain yang disimpan, dirawat dan dijaga kebenarannya serta dilindungi kerahasiaannya berdasarkan ketentuan peraturan perundang-undangan. Untuk menghindari keragu-raguan, Data Pribadi meliputi pula data-data yang dipersyaratkan peraturan perundang-undangan untuk ditelaah guna keperluan penerapan prinsip ").Style(bodyStyle);
                                text.Span("know your customer").Style(italicBodyStyle);
                                text.Span(" atau prinsip ").Style(bodyStyle);
                                text.Span("mengenal nasabah").Style(bodyStyle);
                                text.Span(".").Style(bodyStyle);
                            });
                        });
                    }
                    else if (definition.Term1 == "Surat Jaminan")
                    {
                        defs.Item().Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text($"1.{itemNumber++}").Style(bodyStyle);
                            row.RelativeItem().Column(subColumn =>
                            {
                                subColumn.Item().Text(text =>
                                {
                                    text.Justify();
                                    text.Span("Surat Jaminan").Style(boldBodyStyle);
                                    text.Span(" adalah dokumen keabsahan Peserta yang diterbitkan oleh Pihak Kedua sebagai syarat untuk memperoleh pelayanan kesehatan di Pihak Kedua berupa:").Style(bodyStyle);
                                });
                                subColumn.Item().PaddingTop(5).PaddingLeft(2).Row(subRow =>
                                {
                                    subRow.ConstantItem(20).ShowOnce().AlignTop().Text("1)").Style(bodyStyle);
                                    subRow.RelativeItem().Text(text =>
                                    {
                                        text.Justify();
                                        text.Span("Letter of Acceptance").Style(boldBodyStyle);
                                        text.Span(" atau ").Style(bodyStyle);
                                        text.Span("LOA").Style(boldBodyStyle);
                                        text.Span(" adalah surat konfirmasi resmi luaran sistem aplikasi untuk melakukan validasi dan otorisasi jaminan kesehatan. LOA merupakan korespondensi format yang dikirim untuk mengkonfirmasi penjaminan/penundaan/penolakan jaminan layanan kesehatan dan dicetak untuk dilanjutkan konfirmasi kepada Peserta sebelum dilakukan layanan kesehatan.").Style(bodyStyle);
                                    });
                                });
                                subColumn.Item().PaddingTop(5).PaddingLeft(2).Row(subRow =>
                                {
                                    subRow.ConstantItem(20).AlignTop().Text("2)").Style(bodyStyle);
                                    subRow.RelativeItem().Text(text =>
                                    {
                                        text.Justify();
                                        text.Span("Letter of Confirmation").Style(boldBodyStyle);
                                        text.Span(" atau ").Style(bodyStyle);
                                        text.Span("LOC").Style(boldBodyStyle);
                                        text.Span(" adalah surat konfirmasi resmi luaran sistem aplikasi yang dilakukan validasi rincian biaya yang timbul, jaminan kesehatan. LOC merupakan korespondensi format yang dikirim untuk mengkonfirmasi atau melakukan validasi biaya yang timbul setelah dilakukan jaminan termasuk informasi iur biaya Peserta dan dicetak untuk konfirmasi kepada Peserta setelah dilakukan layanan kesehatan.").Style(bodyStyle);
                                    });
                                });

                                subColumn.Item().PaddingTop(5).PaddingLeft(2).Row(subRow =>
                                {
                                    subRow.ConstantItem(20).AlignTop().Text("3)").Style(bodyStyle);
                                    subRow.RelativeItem().Text(text =>
                                    {
                                        text.Justify();
                                        text.Span("Surat Eligibilitas Peserta").Style(boldBodyStyle);
                                        text.Span(" atau ").Style(bodyStyle);
                                        text.Span(" SEP").Style(boldBodyStyle);
                                        text.Span("adalah surat keabsahan Pelanggan dan/atau Peserta yang diterbitkan oleh fasilitas kesehatan BPJS Kesehatan sebagai syarat untuk mendapatkan pelayanan kesehatan bagi peserta program Jaminan Kesehatan Nasional.").Style(bodyStyle);
                                    });
                                });
                            });
                        });
                    }
                    else if (definition.Term1 == "Hari Kalender")
                    {
                        defs.Item().Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text($"1.{itemNumber++}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                // Mulai dengan istilah definisi yang ditebalkan
                                text.Span("Hari Kalender").Style(boldBodyStyle);

                                // Pecah penjelasannya menjadi tiga bagian untuk format khusus
                                // Bagian 1: Teks sebelum kata "libur"
                                text.Span(" adalah setiap hari dalam 1 (satu) tahun sesuai dengan kalender gregorius (berdasarkan kalender masehi) tanpa terkecuali, termasuk hari sabtu, minggu, dan hari ").Style(bodyStyle);

                                // Bagian 2: Kata "libur" yang ditebalkan
                                text.Span("libur").Style(boldBodyStyle);

                                // Bagian 3: Teks setelah kata "libur"
                                text.Span(" nasional yang ditetapkan oleh pemerintah dari waktu ke waktu.").Style(bodyStyle);
                            });
                        });
                    }
                    else if (definition.Explanation1 == "atau" && definition.Term2 != null)
                    {
                        defs.Item().Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().Text($"1.{itemNumber++}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span($"{definition.Term1}").Style(boldBodyStyle);
                                text.Span($" {definition.Explanation1}").Style(bodyStyle);
                                text.Span($" {definition.Term2}").Style(boldBodyStyle);
                                text.Span($" {definition.Explanation2}").Style(bodyStyle);
                            });
                        });
                    }
                    else
                    {
                        defs.Item().Row(row =>
                        {
                            row.ConstantItem(35).AlignTop().ShowOnce().Text($"1.{itemNumber++}").Style(bodyStyle);
                            row.RelativeItem().Text(text =>
                            {
                                text.Justify();
                                text.Span($"{definition.Term1}").Style(boldBodyStyle);
                                text.Span($" {definition.Explanation1}").Style(bodyStyle);
                                text.Span($"{definition.Term2}").Style(boldBodyStyle);
                                text.Span($" {definition.Explanation2}").Style(bodyStyle);

                            });
                        });
                    }
                }
            });

            // Section 2. Interpretasi
            column.Item().PaddingTop(10).Row(row =>
            {
                row.ConstantItem(25).AlignTop().Text("2.").Style(bodyStyle);
                row.RelativeItem().Text(text =>
                {
                    text.Justify();
                    text.Span("Dalam Perjanjian ini, kecuali ditentukan lain, rujukan ke:").Style(bodyStyle);
                });
            });

            column.Item().PaddingTop(10).PaddingLeft(25).Column(defs =>
            {
                defs.Spacing(10);
                foreach (var definition in point2Definitions)
                {
                    defs.Item().Row(row =>
                    {
                        row.ConstantItem(35).ShowOnce().AlignTop().Text($"{definition.Term1}").Style(bodyStyle);
                        row.RelativeItem().Text(text =>
                        {
                            text.Justify();
                            text.Span($"{definition.Explanation1}").Style(bodyStyle);
                        });
                    });
                }
            });
        }
        private List<DefinitionItem> GetDefinitions()
        {
            return new List<DefinitionItem>
            {
                new DefinitionItem { Term1 = "Alat Kesehatan", Explanation1 = "adalah instrumen, aparatus, mesin dan/atau implan yang tidak mengandung obat yang digunakan untuk mencegah, mendiagnosis, menyembuhkan dan meringankan penyakit, merawat orang sakit, memulihkan kesehatan pada manusia, dan/atau membentuk struktur dan memperbaiki fungsi tubuh." },
                new DefinitionItem { Term1 = "Aplikasi Kesehatan", Explanation1 = "adalah aplikasi yang disediakan oleh Pihak Pertama, yang dapat diunduh oleh Peserta melalui Playstore dan/atau Appstore yang berisi tentang informasi kesehatan dan layanan asuransi seperti Info Provider, Dokter Online, E-Resep, Info Sehat, Tombol Konfirmasi, Info Benefit/manfaat, E-Claim, dan E-Card." },
                new DefinitionItem { Term1 = "Aplikasi ", Explanation1 = "Pihak Pertama adalah sistem informasi milik Pihak Pertama yang diakses dan digunakan oleh Pihak Kedua dalam memberikan pelayanan kesehatan, mulai dari tahap awal pelayanan hingga tahap penagihan Klaim pelayanan kesehatan." },
                new DefinitionItem { Term1 = "Aplikasi Sistem Informasi Manajemen Obat", Explanation1 = "atau", Term2 = "Aplikasi SIMO", Explanation2 = " adalah sistem informasi yang digunakan dalam hal manajemen obat mulai dari tahap awal penyusunan hingga tahap transaksi obat di Provider." },
                new DefinitionItem { Term1 = "Badan Penyelenggara Jaminan Sosial Kesehatan", Explanation1 = "atau", Term2 = "BPJS Kesehatan", Explanation2 = "adalah Badan hukum yang dibentuk untuk menyelenggarakan program Jaminan Kesehatan Nasional (JKN)." },
                new DefinitionItem { Term1 = "Bahan dan Alat Kesehatan Habis Pakai", Explanation1 = "atau", Term2 = "BAHP", Explanation2 = "adalah bahan dan Alat Kesehatan yang digunakan oleh Pihak Kedua dalam rangka melakukan diagnosa, pengobatan, dan perawatan yang disediakan oleh Pihak Kedua." },
                new DefinitionItem { Term1 = "Berkas Elektronik", Explanation1 = "adalah berkas/dokumen penagihan Klaim berupa hardcopy/berkas fisik yang dialih mediakan kedalam bentuk elektronik (softcopy) yang diajukan oleh Pihak Kedua melalui Aplikasi Pihama." },
                new DefinitionItem { Term1 = "Data Pribadi", Explanation1 = "adalah setiap data atau informasi (baik tertulis, oral, digital atau dalam bentuk apapun) yang benar dan nyata yang melekat dan dapat diidentifikasi berdasarkan dokumen identitas Peserta, baik langsung maupun tidak langsung, pada masing-masing Peserta, termasuk namun tidak terbatas pada nama, tempat lahir, tanggal lahir, alamat, nomor kontak, nama ibu kandung, data simpanan, data kartu kredit, data pinjaman pada setiap institusi keuangan, dan/atau data-data lain yang disimpan, dirawat dan dijaga kebenarannya serta dilindungi kerahasiaannya berdasarkan ketentuan peraturan perundang-undangan. Untuk menghindari keragu-raguan, Data Pribadi meliputi pula data-data yang dipersyaratkan peraturan perundang-undangan untuk ditelaah guna keperluan penerapan prinsip know your customer atau prinsip mengenal nasabah." },
                new DefinitionItem { Term1 = "Distributor", Explanation1 = "adalah perusahaan yang telah memenuhi regulasi di bidang perdagangan atau nasional terkait lainnya sesuai dengan ketentuan peraturan perundang-undangan yang berlaku di Indonesia yang mendistribusikan atau menyalurkan obat-obatan dan/atau Alat Kesehatan yang diproduksi dan/atau dipasarkan oleh Pihak Kedua yang tercantum dalam Formularium Obat Inhealth dan bekerjasama dengan Pihak Pertama." },
                new DefinitionItem { Term1 = "Formularium Obat Inhealth", Explanation1 = "atau", Term2 = "FOI", Explanation2 = "adalah daftar obat dan/atau Alat Kesehatan yang digunakan oleh Pihak Pertama, yang disusun berdasarkan item obat dan/atau Alat Kesehatan berdasarkan kesepakatan kerjasama dengan Pihak Kedua untuk memenuhi kebutuhan pemberian resep bagi Peserta dengan prinsip Evidence Based Medicine (EBM), Patient Safety dan indikasi yang disetujui oleh Badan Pengawas Obat dan Makanan (BPOM)." },
                new DefinitionItem { Term1 = "Formularium Rumah Sakit", Explanation1 = "adalah obat yang pengadaannya dilakukan oleh Pihak Kedua dan diresepkan oleh dokter Pihak Kedua kepada Peserta." },
                new DefinitionItem { Term1 = "Formulir Pengajuan Klaim", Explanation1 = "atau", Term2 = "FPK", Explanation2 = "adalah formulir standar Pihak Pertama, yang wajib diisi oleh Pihak Kedua dan disertakan pada saat pengajuan Klaim kepada Pihak Pertama." },
                new DefinitionItem { Term1 = "Hari Kalender", Explanation1 = "adalah setiap hari dalam 1 (satu) tahun sesuai dengan kalender gregorius (berdasarkan kalender masehi) tanpa terkecuali, termasuk hari sabtu, minggu, dan hari libur nasional yang ditetapkan oleh pemerintah dari waktu ke waktu." },
                new DefinitionItem { Term1 = "Hari Kerja", Explanation1 = "adalah hari di mana di mana Pihak Pertama beroperasi pada jam kerja yaitu pukul 08.00 - 17.00 waktu setempat, tidak termasuk hari sabtu, minggu, atau hari libur nasional yang ditetapkan oleh pemerintah dari waktu ke waktu." },
                new DefinitionItem { Term1 = "Hari Rawat", Explanation1 = "adalah perhitungan jumlah Hari Rawat Inap Peserta pada layanan kesehatan milik Pihak Kedua yang dihitung berdasarkan tanggal keluar dikurangi tanggal", Term2 = " masuk", Explanation2 = "dengan ketentuan apabila tanggal keluar sama dengan tanggal masuk, maka dihitung sebagai 1 (satu) Hari Rawat Inap." },
                new DefinitionItem { Term1 = "Indonesian-Case Based Group", Explanation1 = "atau", Term2 = "INA-CBG", Explanation2 = "adalah tarif klaim yang ditetapkan oleh BPJS Kesehatan kepada fasilitas kesehatan rujukan tingkat lanjutan atas paket pelayanan kesehatan yang didasarkan kepada pengelompokan diagnosis penyakit dan prosedur." },
                new DefinitionItem { Term1 = "Kartu Asuransi", Explanation1 = "adalah kartu identitas peserta yang diterbitkan oleh Pihak Pertama sebagai bukti kepesertaan pelayanan kesehatan Pihak Pertama. Kartu Asuransi dapat diterbitkan secara fisik maupun elektronik." },
                new DefinitionItem { Term1 = "Kasus Gawat Darurat (emergency", Explanation1 = ") adalah suatu kasus/gangguan mendadak yang harus mendapatkan pelayanan kesehatan secepatnya untuk mencegah kematian, keparahan, dan/atau kecacatan sesuai dengan peraturan perundang-undangan." },
                new DefinitionItem { Term1 = "Klaim", Explanation1 = "adalah uang penggantian yang dibayar oleh Pihak Pertama kepada Pihak Kedua atas pelayanan kesehatan dan/atau obat yang diberikan kepada Peserta." },
                new DefinitionItem { Term1 = "Manfaat Pelayanan", Explanation1 = "adalah pelayanan kesehatan yang diperjanjikan dalam Perjanjian ini atau ketentuan lain yang berlaku pada Pihak Pertama dengan Pemegang Polis." },
                new DefinitionItem { Term1 = "Manfaat Top Up", Explanation1 = "adalah penjaminan yang diberikan oleh Pihak Pertama kepada Pihak Kedua atas biaya pelayanan kesehatan Peserta berdasarkan ketentuan Pihak Pertama." },
                new DefinitionItem { Term1 = "Pelayanan Gawat Darurat (emergency )", Explanation1 = "adalah pelayanan yang harus diberikan secepatnya pada Kasus Gawat Darurat untuk mengurangi risiko kematian atau cacat." },
                new DefinitionItem { Term1 = "Pelayanan One Day Care", Explanation1 = "adalah pelayanan atau tindakan pembedahan atau Pelayanan Gawat Darurat yang dilakukan di Instalasi Gawat Darurat Pihak Kedua terhadap kondisi penyakit tertentu yang dilaksanakan oleh tenaga ahli kesehatan dengan atau tanpa anestesi paling sedikit enam jam serta Peserta tanpa harus rawat inap." },
                new DefinitionItem { Term1 = "Pemegang Polis", Explanation1 = "adalah pihak yang mengikatkan diri berdasarkan perjanjian dengan Pihak Pertama untuk mendapatkan perlindungan atau pengelolaan atas risiko bagi pengguna Produk Asuransi." },
                new DefinitionItem { Term1 = "Pemeriksaan Penunjang Diagnostik", Explanation1 = "adalah kegiatan pemeriksaan yang dilakukan untuk membantu menegakkan diagnosa sesuai indikasi medis." },
                new DefinitionItem { Term1 = "Peninjauan", Explanation1 = "adalah kegiatan peninjauan oleh Pihak Pertama terhadap kesiapan Pihak Kedua untuk menjalankan tugas dan kewajiban sebagai Provider untuk menyediakan pelayanan kesehatan kepada Peserta." },
                new DefinitionItem { Term1 = "Persalinan", Explanation1 = "adalah proses lahirnya bayi cukup bulan atau hampir cukup bulan baik secara spontan maupun lahir dengan memerlukan tindakan medis baik operatif dan non operatif." },
                new DefinitionItem { Term1 = "Peserta", Explanation1 = "adalah orang-perorangan yang terdaftar sebagai peserta asuransi kesehatan yang diselenggarakan oleh Pihak Pertama." },
                new DefinitionItem { Term1 = "Pelayanan Kesehatan Tingkat Lanjutan", Explanation1 = "adalah upaya pelayanan kesehatan perorangan yang bersifat spesialisik atau subspesialisik yang meliputi Rawat Jalan Lanjutan, Rawat Inap Lanjutan, dan Rawat Inap di ruang perawatan khusus." },
                new DefinitionItem { Term1 = "Pelayanan Rawat Jalan Eksekutif", Explanation1 = "adalah pemberian pelayanan kesehatan Rawat Jalan nonreguler di Rumah Sakit yang diselenggarakan melalui pelayanan dokter spesialis atau subspesialis dalam satu fasilitas ruangan terpadu secara khusus, tanpa menginap di Rumah Sakit, dengan sarana dan prasarana di atas standar." },
                new DefinitionItem { Term1 = "Provider", Explanation1 = "adalah sarana/fasilitas pelayanan kesehatan yang bekerja sama dengan Pihak Pertama untuk memberikan pelayanan kesehatan bagi Peserta berdasarkan tingkat pelayanan yang diberikan. Provider terbagi atas:" },
                new DefinitionItem { Term1 = "Provider Irisan", Explanation1 = "adalah Provider mitra dari Pihak Pertama yang terdaftar sebagai sarana pelayanan kesehatan BPJS Kesehatan." },
                new DefinitionItem { Term1 = "Rekam Medis", Explanation1 = "adalah berkas yang berisikan catatan dan dokumen tentang identitas pasien, pemeriksaan, pengobatan, tindakan dan pelayanan lain yang telah diberikan oleh Provider kepada Peserta." },
                new DefinitionItem { Term1 = "Rawat Inap Tingkat Lanjutan", Explanation1 = "atau", Term2 = "RI", Explanation2 = "adalah pelayanan yang bersifat spesialisik/subspesialisik untuk keperluan observasi, perawatan, diagnosis, pengobatan, Rehabilitasi Medis dan atau pelayanan medis lainnya, pada Provider Tingkat Lanjutan yang mana Peserta dirawat inap di ruang perawatan/perawatan khusus paling sedikit 1 (satu) Hari Kalender." },
                new DefinitionItem { Term1 = "Rawat Jalan Tingkat Lanjutan", Explanation1 = "atau", Term2 = "RJTL", Explanation2 = "adalah pelayanan kesehatan yang bersifat spesialisik atau subspesialisik tanpa menginap di ruang perawatan dan dilaksanakan pada Provider Tingkat Lanjutan sebagai Rujukan dari Provider Tingkat Pertama." },
                new DefinitionItem { Term1 = "Rehabilitasi Medis", Explanation1 = "adalah pelayanan yang diberikan oleh instalasi rehabilitasi medis dalam bentuk fisioterapi, terapi okupasional, terapi wicara, dan bimbingan sosial medik." },
                new DefinitionItem { Term1 = "Rujukan", Explanation1 = "adalah sistem aturan pelimpahan tugas dan tanggung jawab pelayanan kesehatan secara timbal balik antar Provider Tingkat Pertama dengan Provider Tingkat Lanjutan." },
                new DefinitionItem { Term1 = "Rujukan balik", Explanation1 = "adalah layanan kesehatan yang diberikan kepada pasien penderita penyakit kronis setelah menyelesaikan pengobatan di rumah sakit dan masih memerlukan pengobatan jangka panjang." },
                new DefinitionItem { Term1 = "Rumah Sakit", Explanation1 = "adalah sarana/fasilitas upaya kesehatan yang menyelenggarakan kegiatan pelayanan kesehatan secara komprehensif serta dapat dimanfaatkan untuk pendidikan tenaga kesehatan dan penelitian." },
                new DefinitionItem { Term1 = "Surat Jaminan", Explanation1 = "adalah dokumen keabsahan Peserta yang diterbitkan oleh Pihak Kedua sebagai syarat untuk memperoleh pelayanan kesehatan di Pihak Kedua berupa:" },
                new DefinitionItem { Term1 = "Telemedicine", Explanation1 = "adalah layanan kesehatan berbasis teknologi yang memungkinkan para pengguna berkonsultasi dengan dokter tanpa bertatap muka secara langsung atau secara jarak jauh dalam rangka memberikan konsultasi diagnostik dan tata laksana perawatan pasien." },
                new DefinitionItem { Term1 = "Tindakan Medis", Explanation1 = "adalah tindakan yang bersifat operatif dan non operatif yang dilaksanakan baik untuk tujuan diagnostik maupun pengobatan." },
                new DefinitionItem { Term1 = "Verifikasi Klaim", Explanation1 = "adalah upaya pemeriksaan kelengkapan/kebenaran berkas kelayakan Klaim yang diajukan kepada Pihak Pertama oleh Pihak Kedua/Peserta." },
                new DefinitionItem { Term1 = "2.1", Explanation1 = "\"perusahaan\" wajib ditafsirkan sedemikian rupa sehingga mencakup perusahaan, korporasi, atau badan hukum lainnya, di mana pun dan bagaimana pun didirikan;" },
                new DefinitionItem { Term1 = "2.2", Explanation1 = "rujukan kepada suatu pihak dalam dokumen apa pun mencakup para penerus dan penerima pengalihan yang diizinkan dari pihak tersebut;" },
                new DefinitionItem { Term1 = "2.3", Explanation1 = "\"orang\" mencakup individu, firma, perusahaan, korporasi, badan pemerintah atau setiap asosiasi, trust, usaha patungan, konsorsium, atau kemitraan apa pun (baik yang memiliki personalitas hukum terpisah maupun tidak);" },
                new DefinitionItem { Term1 = "2.4", Explanation1 = "kecuali konteks menentukan lain, rujukan apa pun ke bagian, pasal, ayat, atau lampiran adalah bagian, pasal, ayat, atau lampiran, tergantung pada kasusnya, dari atau pada Perjanjian ini;" },
                new DefinitionItem { Term1 = "2.5", Explanation1 = "lampiran-lampiran akan menjadi bagian dari Perjanjian ini dan akan memiliki kekuatan dan efek yang sama seolah-olah secara tegas diatur dalam tubuh Perjanjian ini;" },
                new DefinitionItem { Term1 = "2.6", Explanation1 = "seluruh kata yang digunakan akan dianggap memiliki jenis dan jumlah seperti yang dibutuhkan oleh keadaan;" },
                new DefinitionItem { Term1 = "2.7", Explanation1 = "ketentuan apa pun dari undang-undang wajib ditafsirkan sebagai rujukan untuk ketentuan tersebut sebagaimana diubah, dimodifikasi, diberlakukan kembali atau diperpanjang dari waktu ke waktu;" },
                new DefinitionItem { Term1 = "2.8", Explanation1 = "judul adalah untuk kenyamanan semata dan tidak mempengaruhi penafsiran dari Perjanjian ini;" },
                new DefinitionItem { Term1 = "2.9", Explanation1 = "kata-kata yang bermakna tunggal mencakup makna jamak dan juga sebaliknya;" },
                new DefinitionItem { Term1 = "2.10", Explanation1 = "kecuali dinyatakan sebaliknya, kata \"termasuk\" tidak membatasi kata atau istilah sebelumnya; dan" },
                new DefinitionItem { Term1 = "2.11", Explanation1 = "ketentuan tidak akan ditafsirkan terhadap salah satu pihak hanya berdasarkan penulis dan ketentaun tersebut"}
            };
        }

        public class DefinitionItem
        {
            public required string Term1 { get; set; }
            public required string Explanation1 { get; set; }
            public string? Term2 { get; set; }
            public string? Explanation2 { get; set; }
        }

        #region Main Content Methods
        void ComposeContent(ColumnDescriptor column)
        {
            // Memulai bagian "KETENTUAN KHUSUS" di halaman baru. Ini tetap diperlukan.
            column.Item().PageBreak();
            column.Item().AlignLeft().Text("III. KETENTUAN KHUSUS").Bold().FontSize(12);

            // Mengurutkan bab untuk memastikan urutan tampil benar
            var orderedBabs = _model.KetentuanKhusus.OrderBy(b => b.UrutanTampil).ToList();
            foreach (var (bab, babIndex) in _model.KetentuanKhusus.OrderBy(b => b.UrutanTampil).Select((value, i) => (value, i)))
            {
                // Logika PageBreak tetap sama untuk memisahkan halaman
                if (bab.UrutanTampil == 7)
                {
                    column.Item().PageBreak();
                }

                column.Item().PaddingTop(24).Row(row =>
                {
                    row.Spacing(0);
                    row.ConstantItem(120)
                        .ShowOnce() // Perintah ini sekarang akan bekerja dengan benar di dalam Row
                        .AlignTop()
                        .Text($"{babIndex + 1}. {bab.JudulTeks}")
                        .Bold();
                    row.ConstantItem(10)
                        .ShowOnce() // Perintah ini juga penting
                        .AlignTop()
                        .AlignCenter()
                        .Text(":");

                    row.RelativeItem().Column(contentColumn =>
                    {
                        if (bab.UrutanTampil == 7) RenderRekeningBankFromModel(contentColumn);
                        else if (bab.UrutanTampil == 8) RenderAlamatKorespondensiFromModel(contentColumn);
                        else
                        {
                            if (bab.SubBab != null && bab.SubBab.Any())
                            {
                                var orderedSubBabs = bab.SubBab.OrderBy(s => s.UrutanTampil).ToList();
                                foreach (var (subBab, subBabIndex) in orderedSubBabs.Select((value, i) => (value, i)))
                                {
                                    if (!string.IsNullOrWhiteSpace(subBab.Konten))
                                    {
                                        var subBabLabel = (orderedSubBabs.Count > 1 || (subBab.Poin != null && subBab.Poin.Any()))
                                            ? $"{babIndex + 1}.{subBabIndex + 1}."
                                            : "";

                                        contentColumn.Item().PaddingBottom(8).Row(innerRow =>
                                        {
                                            innerRow.Spacing(5);
                                            if (!string.IsNullOrEmpty(subBabLabel))
                                            {
                                                innerRow.ConstantItem(30).AlignTop().Text(subBabLabel);
                                            }
                                            innerRow.RelativeItem().Text(subBab.Konten).Justify();
                                        });
                                    }

                                    if (subBab.Poin != null && subBab.Poin.Any())
                                    {
                                        RenderHierarchicalPoin(contentColumn, subBab.Poin, null, 0, 35f);
                                    }
                                }
                            }
                        }
                    });
                });
            }

            column.Item().PaddingTop(30).Text(text =>
            {
                text.Justify();
                text.Span("DEMIKIANLAH,").Bold();
                text.Span(" Para Pihak telah menandatangani Perjanjian ini pada hari dan tanggal yang disebutkan di atas.");
            });

            column.Item().PaddingTop(30).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(5);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(5);
                });
                table.Cell().Column(col =>
                {
                    col.Item().Height(40).AlignBottom().Column(inner =>
                    {
                        inner.Item().Text("PT ASURANSI JIWA INHEALTH INDONESIA").Bold();
                    });
                    col.Item().Height(80);
                    col.Item().LineHorizontal(1).LineColor(Colors.Black);
                    col.Item().Table(innerTable =>
                    {
                        innerTable.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);
                            columns.RelativeColumn();
                        });
                        innerTable.Cell().Text("Nama");
                        innerTable.Cell().Text($": {_model.PihakPertama?.NamaPerwakilanPihakPertama ?? "[•]"}");
                        innerTable.Cell().Text("Jabatan");
                        innerTable.Cell().Text($": {_model.PihakPertama?.JabatanPerwakilanPihakPertama ?? "[•]"}");
                    });
                });
                table.Cell();
                table.Cell().Column(col =>
                {
                    col.Item().Height(40).AlignBottom().Text(_model.PihakKedua.NamaEntitasCalonProvider ?? "[nama entitas calon provider]").Bold();
                    col.Item().Height(80);
                    col.Item().LineHorizontal(1).LineColor(Colors.Black);
                    col.Item().Table(innerTable =>
                    {
                        innerTable.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);
                            columns.RelativeColumn();
                        });
                        innerTable.Cell().Text("Nama");
                        innerTable.Cell().Text($": {_model.PihakKedua.NamaPerwakilan ?? "[•]"}");
                        innerTable.Cell().Text("Jabatan");
                        innerTable.Cell().Text($": {_model.PihakKedua.JabatanPerwakilan ?? "[•]"}");
                    });
                });
            });
        }

        void RenderHierarchicalPoin(ColumnDescriptor column, List<PoinModel> allPoinForSubBab, int? parentId, int depth, float indent)
        {
            // Mengambil semua anak langsung dari parentId saat ini
            var children = allPoinForSubBab
                .Where(p => p.ParentId == parentId)
                .OrderBy(p => p.UrutanTampil)
                .ToList();

            if (!children.Any()) return;

            // Loop melalui anak-anak untuk dirender
            for (int i = 0; i < children.Count; i++)
            {
                var poin = children[i];

                string numberLabel = GetNumberingLabel(depth, i);

                // Render baris
                column.Item().PaddingLeft(indent).PaddingBottom(4).Row(row =>
                {
                    row.Spacing(5);
                    row.ConstantItem(30).AlignTop().ShowOnce().Text(numberLabel);
                    row.RelativeItem().ShowOnce().Text(poin.TeksPoin ?? string.Empty).Justify();
                });

                RenderHierarchicalPoin(column, allPoinForSubBab, poin.Id, depth + 1, indent + 35f);
            }
        }

        string GetNumberingLabel(int depth, int index)
        {
            switch (depth % 4) // Menggunakan modulo untuk siklus penomoran
            {
                case 0: // Level 1 -> a, b, c
                    return $"{(char)('a' + index)}.";
                case 1: // Level 2 -> i, ii, iii
                    return $"{ToRoman(index + 1).ToLower()}.";
                case 2: // Level 3 -> 1), 2), 3)
                    return $"{index + 1})";
                case 3: // Level 4 -> (a), (b), (c)
                    return $"({(char)('a' + index)})";
                default:
                    return $"{index + 1}.";
            }
        }


        string ToRoman(int number)
        {
            if (number < 1) return string.Empty;
            if (number >= 4000) return number.ToString();
            var romanNumerals = new[]
            {
                new { Value = 1000, Symbol = "M" }, new { Value = 900, Symbol = "CM" },
                new { Value = 500, Symbol = "D" }, new { Value = 400, Symbol = "CD" },
                new { Value = 100, Symbol = "C" }, new { Value = 90, Symbol = "XC" },
                new { Value = 50, Symbol = "L" }, new { Value = 40, Symbol = "XL" },
                new { Value = 10, Symbol = "X" }, new { Value = 9, Symbol = "IX" },
                new { Value = 5, Symbol = "V" }, new { Value = 4, Symbol = "IV" },
                new { Value = 1, Symbol = "I" }
            };

            var result = new System.Text.StringBuilder();
            foreach (var numeral in romanNumerals)
            {
                while (number >= numeral.Value)
                {
                    result.Append(numeral.Symbol);
                    number -= numeral.Value;
                }
            }
            return result.ToString();
        }


        void RenderRekeningBankFromModel(ColumnDescriptor column)
        {
            column.Item().PaddingBottom(5).Text("REKENING BANK PIHAK KEDUA").Bold();
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.ConstantColumn(10);
                    columns.RelativeColumn(7);
                });
                void AddRow(string key, string value)
                {
                    table.Cell().PaddingBottom(2).Text(key);
                    table.Cell().PaddingBottom(2).AlignCenter().Text(":");
                    table.Cell().PaddingBottom(2).Text(value ?? "[•]");
                }
                AddRow("NAMA BANK", _model.PihakKedua.NamaBank);
                AddRow("CABANG", _model.PihakKedua.NamaCabangBank ?? "[•]");
                AddRow("NOMOR REKENING", _model.PihakKedua.NomorRekening);
                AddRow("PEMILIK REKENING", _model.PihakKedua.PemilikRekening);
                AddRow("NAMA PEMILIK NPWP", _model.PihakKedua.NamaPemilikNpwp);
                AddRow("NO NPWP", _model.PihakKedua.NoNpwp);
                AddRow("JENIS NPWP", _model.PihakKedua.JenisNpwp);
            });
        }

        void RenderAlamatKorespondensiFromModel(ColumnDescriptor column)
        {
            // BENAR:
            column.Item().PaddingBottom(2).Text("Pihak Pertama:");
            column.Item().PaddingLeft(15).Text("PT Asuransi Jiwa Inhealth Indonesia").Bold();
            column.Item().PaddingLeft(15).Text("Jl.Prof.Dr.Satrio Kav.E-IV No.6, Mega Kuningan");
            column.Item().PaddingLeft(15).Text("Jakarta Selatan 12940, Lt.8,9,10.");
            column.Item().PaddingLeft(15).Text("Divisi Pelayanan Kesehatan");
            column.Item().PaddingLeft(15).Text(text =>
            {
                text.Span("U.p. : ");
                text.Span(_model.PihakPertama?.NamaPerwakilanPihakPertama ?? "[•]").Bold();
            });
            column.Item().PaddingLeft(15).Text("Email: " + (_model.PihakPertama?.Email ?? "[•]"));
            column.Item().PaddingTop(10).PaddingBottom(2).Text("Pihak Kedua:");
            column.Item().PaddingLeft(15).Text(_model.PihakKedua.NamaEntitasCalonProvider ?? "PT [•]").Bold();
            column.Item().PaddingLeft(15).Text(_model.PihakKedua.AlamatPemegangPolis ?? "[Alamat]");
            column.Item().PaddingLeft(15).Text($"Telp: {_model.PihakKedua.Telepon ?? "[•]"}");
            column.Item().PaddingLeft(15).Text($"U.p.: {_model.PihakKedua.NamaPerwakilan ?? "[•]"}");
            column.Item().PaddingLeft(15).Text("Email: " + (_model.PihakKedua?.Email ?? "[•]"));
        }
        #endregion

        #region Appendix Rendering Methods

        void RenderListOtomatis(ColumnDescriptor column, List<PoinModel> items)
        {
            var orderedItems = items.OrderBy(p => p.UrutanTampil).ToList();
            
            for (int i = 0; i < orderedItems.Count; i++)
            {
                var poin = orderedItems[i];
                
                string numberLabel = $"{(char)('a' + i)}."; 
                
                column.Item().PaddingLeft(20f).PaddingBottom(4).Row(row =>
                {
                    row.Spacing(5); // Jarak antara nomor dan teks
                    row.ConstantItem(25).AlignTop().Text(numberLabel);
                    row.RelativeItem().Text(poin.TeksPoin).Justify();
                });
            }
        }

void RenderPoinDanTabel(
    ColumnDescriptor column,
    List<PoinModel> allPoin,
    int? parentId,
    int depth,
    bool forceSimpleNumbering = false)
{
    var children = allPoin
        .Where(p => p.ParentId == parentId)
        .OrderBy(p => p.UrutanTampil)
        .ToList();

    if (!children.Any()) return;

    int itemCounter = 0;

    for (int i = 0; i < children.Count; i++)
    {
        var poin = children[i];
        var teks = poin.TeksPoin?.Trim() ?? string.Empty;

        if (forceSimpleNumbering)
        {
            column.Item().PaddingLeft(25f).PaddingBottom(5).Row(row =>
            {
                row.Spacing(5);
                row.ConstantItem(35).AlignTop().Text($"{i + 1}.");
                row.RelativeItem().Text(teks).Justify();
            });
            itemCounter++;
        }
        else
        {
            float indent = 0;
            switch(depth)
            {
                case 0: indent = 0; break;      
                case 1: indent = 0; break;      
                case 2: indent = 21f; break;     
                case 3: indent = 45f; break;     
                default: indent = 65f; break;
            }
            
            if (teks.StartsWith("[TABLE]"))
            {
                RenderTabelBiasa(column, teks);
                itemCounter++;
            }
            else if (teks.StartsWith("[HEADER]"))
            {
                var headerText = teks.Substring(8);
                column.Item().PaddingBottom(5).Text(headerText).SemiBold();
            }
            else if (teks.StartsWith("[NOTE]"))
            {
                var noteText = teks.Substring(6); 
                column.Item().PaddingTop(10).Text(noteText); 
            }
            else if (teks.StartsWith("-"))
            {
                column.Item().PaddingLeft(indent + 35f).PaddingBottom(5).Text(teks);
            }
            else
            {
                string numberLabel = "";
                switch (depth)
                {
                    case 0: numberLabel = ""; break;
                    case 1: numberLabel = $"{itemCounter + 1}."; break;
                    case 2: numberLabel = $"{itemCounter + 1}."; break; 
                    case 3: numberLabel = $"{(char)('a' + itemCounter)}."; break;
                    default: numberLabel = ""; break;
                }
                
                column.Item().PaddingLeft(indent).PaddingBottom(5).Row(row =>
                {
                    row.Spacing(2); 
                    if (!string.IsNullOrEmpty(numberLabel))
                    {
                        row.ConstantItem(20).AlignTop().Text(numberLabel);
                    }
                    row.RelativeItem().Text(teks).Justify();
                });
                
                itemCounter++;
            }
        }
        
        RenderPoinDanTabel(column, allPoin, poin.Id, depth + 1, false);
    }
}
        void RenderTabelBiasa(ColumnDescriptor column, string teks)
        {
            column.Item().PaddingTop(10).PaddingBottom(10).Table(table =>
            {
                var headers = teks.Replace("[TABLE]", "").Replace("[/TABLE]", "").Split('|');

                table.ColumnsDefinition(columns =>
                {
                    if (teks.Contains("Jenis Tindakan Medis"))
                    {
                        columns.ConstantColumn(40);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2.5f);
                    }
                    else
                    {
                        foreach (var _ in headers)
                        {
                            columns.RelativeColumn();
                        }
                    }
                });

                foreach (var header in headers)
                {
                    table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text(header).Bold();
                }

                if (teks.Contains("Jenis PIC"))
                {
                    if (_model.LampiranPic != null && _model.LampiranPic.Any())
                    {
                        foreach (var pic in _model.LampiranPic)
                        {
                            table.Cell().Border(1).Padding(5).Text(pic.JenisPIC);
                            table.Cell().Border(1).Padding(5).Text(pic.NamaPIC);
                            table.Cell().Border(1).Padding(5).Text(pic.NomorTelepon);
                            table.Cell().Border(1).Padding(5).Text(pic.AlamatEmail);
                        }
                    }
                }
                else if (teks.Contains("Jenis Tindakan Medis"))
                {
                    if (_model.LampiranTindakanMedis != null && _model.LampiranTindakanMedis.Any())
                    {
                        int nomor = 1;
                        foreach (var tindakan in _model.LampiranTindakanMedis)
                        {
                            table.Cell().Border(1).Padding(5).AlignCenter().Text((nomor++).ToString());
                            table.Cell().Border(1).Padding(5).Text(tindakan.JenisTindakanMedis);
                            table.Cell().Border(1).Padding(5).AlignRight().Text($"Rp. {tindakan.Tarif?.ToString("N0", new System.Globalization.CultureInfo("id-ID")) ?? "0"}");
                            table.Cell().Border(1).Padding(5).Text(tindakan.Keterangan);
                        }
                    }
                }
            });
        }

        void RenderTabelKhususMenyatu(ColumnDescriptor column, List<string> tabelRows)
        {
            column.Item().PaddingTop(10).Border(1).Column(tableColumn =>
            {
                foreach (var (tabelTeks, index) in tabelRows.Select((value, i) => (value, i)))
                {
                    var parts = tabelTeks.Split(new[] { '|' }, 2);

                    if (parts.Length == 2)
                    {
                        tableColumn.Item().Row(row =>
                        {
                            // Kolom Kiri (Judul)
                            row.ConstantItem(150)
                                .BorderRight(1)
                                .BorderBottom(index < tabelRows.Count - 1 ? 1 : 0)
                                .Padding(5)
                                .ShowOnce()
                                .Text(parts[0]).Bold();

                            // Kolom Kanan (Konten)
                            row.RelativeItem()
                                .BorderBottom(index < tabelRows.Count - 1 ? 1 : 0)
                                .Padding(5)
                                .Column(col =>
                                {
                                    var lines = parts[1].Split(new[] { "\\n" }, StringSplitOptions.None)
                                                    .Where(l => !string.IsNullOrWhiteSpace(l))
                                                    .ToList();

                                    if (lines.Count == 1)
                                    {
                                        col.Item().Text(lines[0].Trim()).Justify();
                                    }
                                    else
                                    {
                                        int generatedNumberCounter = 1;

                                        foreach (var line in lines)
                                        {
                                            var match = Regex.Match(line.Trim(), @"^([a-zA-Z0-9]+\.|\d+\)|\([a-zA-Z0-9]+\))\s*");

                                            string numberLabel;
                                            string textContent;

                                            if (match.Success)
                                            {
                                                numberLabel = match.Value;
                                                textContent = line.Trim().Substring(match.Length);
                                            }
                                            else
                                            {
                                                numberLabel = $"{generatedNumberCounter}.";
                                                textContent = line.Trim();
                                                generatedNumberCounter++;
                                            }

                                            float indent = (!string.IsNullOrWhiteSpace(numberLabel) && char.IsLetter(numberLabel.Trim(), 0))
                                                            ? 22f // Indentasi untuk sub-list (a, b, c)
                                                            : 0;  // Tidak ada indentasi untuk list utama (1, 2, 3)

                                            col.Item().PaddingLeft(indent).PaddingBottom(2).Row(innerRow =>
                                            {
                                                innerRow.Spacing(2); // Jarak antara nomor dan teks
                                                innerRow.ConstantItem(20).AlignLeft().Text(numberLabel);
                                                innerRow.RelativeItem().Text(textContent).Justify();
                                            });
                                        }
                                    }
                                });
                        });
                    }
                }
            });
        }

       private string NormalisasiNamaLayanan(string teks)
        {
            if (string.IsNullOrWhiteSpace(teks))
                return string.Empty;

            var cleanedText = teks.Replace('\u00A0', ' ');

            cleanedText = Regex.Replace(cleanedText, @"^([a-zA-Z0-9]+\.|\d+\))\s*", "").Trim();
            cleanedText = Regex.Replace(cleanedText, @"\s*\([^)]*\)", "").Trim();
            if (cleanedText.EndsWith(" dan", StringComparison.OrdinalIgnoreCase))
            {
                cleanedText = cleanedText.Substring(0, cleanedText.Length - 4).Trim();
            }

            if (cleanedText.StartsWith("PELAYANAN ", StringComparison.OrdinalIgnoreCase))
            {
                cleanedText = cleanedText.Substring(10).Trim();
            }
            cleanedText = Regex.Replace(cleanedText, @"[^a-zA-Z0-9]", "");
            
            return cleanedText.ToUpper();
        }


void ComposeLampiran(ColumnDescriptor column)
{
    foreach (var lampiran in _model.Lampiran.OrderBy(l => l.UrutanTampil))
    {
        column.Item().PageBreak();
        column.Item().AlignLeft().Text(lampiran.JudulTeks).Bold().FontSize(14);
        column.Item().PaddingBottom(20);

        foreach (var subBab in lampiran.SubBab.OrderBy(s => s.UrutanTampil))
        {
            if (!string.IsNullOrWhiteSpace(subBab.Konten))
            {
                column.Item().AlignLeft().Text(subBab.Konten).SemiBold();
                column.Item().PaddingBottom(10);
            }

            if (!subBab.Konten.Contains("RUANG LINGKUP DAN MEKANISME"))
            {
                if (subBab.Poin != null && subBab.Poin.Any())
                {
                     RenderPoinDanTabel(column, subBab.Poin, null, 0);
                }
                continue; 
            }
            
            var semuaPoin = subBab.Poin?.OrderBy(p => p.UrutanTampil)?.ToList() ?? new List<PoinModel>();
            var poinRuangLingkup = new List<PoinModel>();
            var poinPenjelasan = new List<PoinModel>();
            bool isPenjelasanSection = false;

            foreach (var poin in semuaPoin)
            {
                if (poin.TeksPoin.Contains("Penjelasan mekanisme")) { isPenjelasanSection = true; }
                if (isPenjelasanSection) { poinPenjelasan.Add(poin); }
                else { poinRuangLingkup.Add(poin); }
            }

            var daftarLayananAktif = new HashSet<string>(
                poinRuangLingkup.Skip(1).Select(p => NormalisasiNamaLayanan(p.TeksPoin))
            );

            if (poinRuangLingkup.Any())
            {
                var judulRuangLingkup = poinRuangLingkup.First();
                column.Item().AlignLeft().Text(judulRuangLingkup.TeksPoin).SemiBold(); // Dibuat SemiBold agar konsisten
                column.Item().PaddingBottom(10);

                var daftarItem = poinRuangLingkup.Skip(1).ToList();
                for (int i = 0; i < daftarItem.Count; i++)
                {
                    var poin = daftarItem[i];
                    string numberLabel = $"{(char)('a' + i)}."; 
                    
                    column.Item().PaddingLeft(20f).PaddingBottom(4).Row(row =>
                    {
                        row.Spacing(5);
                        row.ConstantItem(25).AlignTop().Text(numberLabel);
                        row.RelativeItem().Text(poin.TeksPoin).Justify();
                    });
                }
            }
            
            if (poinPenjelasan.Any())
            {
                column.Item().Height(20); 
                
                var judulPenjelasan = poinPenjelasan.First();
                column.Item().AlignLeft().Text(judulPenjelasan.TeksPoin).SemiBold(); 
                column.Item().PaddingBottom(10);
                
                var tabelUntukDirender = new List<string>();
                int penjelasanCounter = 1;
                
                foreach (var poin in poinPenjelasan.Skip(1))
                {
                    var match = Regex.Match(poin.TeksPoin, @"\[TABLE_SPECIAL\](.*?)\[/TABLE_SPECIAL\]", RegexOptions.Singleline);
                    if (match.Success)
                    {
                        string kontenTabel = match.Groups[1].Value.Trim();
                        var parts = kontenTabel.Split(new[] { '|' }, 2);
                        if (parts.Length == 2)
                        {
                            string judulTabel = NormalisasiNamaLayanan(parts[0]);
                            if (daftarLayananAktif.Contains(judulTabel))
                            {
                                string kontenTabelBaru = $"{penjelasanCounter}. {parts[0].Trim()}|{parts[1]}";
                                tabelUntukDirender.Add(kontenTabelBaru);
                                penjelasanCounter++;
                            }
                        }
                    }
                }
                
                if (tabelUntukDirender.Any())
                {
                    RenderTabelKhususMenyatu(column, tabelUntukDirender);
                }
            }
        }
    }
}  
   
void ComposeLampiranTarif(ColumnDescriptor column, BabModel lampiran)
{
    var semuaPoin = lampiran.SubBab.SelectMany(sb => sb.Poin).ToList();
    RenderPoinTarifHierarkis(column, semuaPoin, null, 0, 0);
}

void RenderPoinTarifHierarkis(ColumnDescriptor column, List<PoinModel> allPoin, int? parentId, int depth, float indent)
{
    var children = allPoin
        .Where(p => p.ParentId == parentId)
        .OrderBy(p => p.UrutanTampil)
        .ToList();

    if (!children.Any()) return;

    for (int i = 0; i < children.Count; i++)
    {
        var poin = children[i];
        string numberLabel = "";

        switch (depth)
        {
            case 0: // Level 1 -> 1., 2.
                numberLabel = $"{i + 1}.";
                break;
            case 1: // Level 2 -> a., b.
                numberLabel = $"{(char)('a' + i)}.";
                break;
        }
        
        column.Item().PaddingLeft(indent).PaddingBottom(5).Row(row =>
        {
            row.Spacing(5);
            row.ConstantItem(35).AlignTop().Text(numberLabel);
            row.RelativeItem().Text(poin.TeksPoin);
        });

        RenderPoinTarifHierarkis(column, allPoin, poin.Id, depth + 1, indent + 20f);
    }
}
        #endregion
    }
    
}