using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using tastemam.Models;

namespace tastemam.Services
{
    public class PdfService
    {
        public byte[] GenerateOrderReceipt(Order order, List<OrderItem> items)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("tastemam Catering").FontSize(22).Bold().FontColor("#C0392B");
                        col.Item().Text("Sipariş Fişi").FontSize(14).FontColor("#888888");
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor("#C9A84C");
                    });

                    page.Content().PaddingTop(20).Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Sipariş No: #{order.ID}").Bold();
                                c.Item().Text($"Tarih: {order.Date:dd.MM.yyyy HH:mm}");
                                c.Item().Text($"Müşteri: {order.CustomerName}");
                                c.Item().Text($"Durum: {order.State}");
                            });
                        });

                        col.Item().PaddingTop(20).Text("Sipariş Detayları").FontSize(13).Bold();
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor("#dddddd");

                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Ürün").Bold();
                                header.Cell().Text("Adet").Bold();
                                header.Cell().Text("Birim Fiyat").Bold();
                                header.Cell().Text("Toplam").Bold();
                            });

                            foreach (var item in items)
                            {
                                table.Cell().Text(item.Menu?.Name ?? "-");
                                table.Cell().Text(item.Pieces.ToString());
                                table.Cell().Text($"{item.UnitPrice}₺");
                                table.Cell().Text($"{item.UnitPrice * item.Pieces}₺");
                            }
                        });

                        col.Item().PaddingTop(20).AlignRight()
                            .Text($"Genel Toplam: {order.TotalPrice}₺")
                            .FontSize(14).Bold().FontColor("#C0392B");
                    });

                    page.Footer().AlignCenter()
                        .Text("tastemam Catering — tastemam.noreply@gmail.com")
                        .FontSize(9).FontColor("#aaaaaa");
                });
            }).GeneratePdf();
        }

        public byte[] GenerateCaretakerAgreement(string caretakerEmail, string caretakerName, DateTime date)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10.5f));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("tastemam Catering").FontSize(20).Bold().FontColor("#C0392B");
                        col.Item().Text("Caretaker Hizmet Sözleşmesi").FontSize(13).FontColor("#888888");
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor("#C9A84C");
                    });

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text(txt =>
                        {
                            txt.Span("Sözleşme Tarihi: ").Bold();
                            txt.Span(date.ToString("dd.MM.yyyy"));
                        });

                        col.Item().PaddingBottom(10).Text(txt =>
                        {
                            txt.Span("Caretaker: ").Bold();
                            txt.Span(caretakerEmail);
                        });

                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor("#dddddd");
                        col.Item().PaddingTop(10).Text("MADDE 1 — TARAFLAR").Bold();
                        col.Item().Text("İşbu sözleşme, tastemam Catering platformu (bundan böyle 'Platform' olarak anılacaktır) ile platforma kayıtlı caretaker (bundan böyle 'Hizmet Sağlayıcı' olarak anılacaktır) arasında akdedilmiştir.");

                        col.Item().PaddingTop(8).Text("MADDE 2 — KONU").Bold();
                        col.Item().Text("İşbu sözleşmenin konusu, Hizmet Sağlayıcı'nın Platform üzerinden sunacağı catering hizmetlerine ilişkin karşılıklı hak ve yükümlülüklerin belirlenmesidir.");

                        col.Item().PaddingTop(8).Text("MADDE 3 — HİZMET TANIMI").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, Platform üzerinde menü oluşturma, fiyat belirleme ve sipariş kabul etme yetkisine sahiptir. Sunulan hizmetlerin kalitesi ve güvenliği Hizmet Sağlayıcı'nın sorumluluğundadır.");

                        col.Item().PaddingTop(8).Text("MADDE 4 — ÖDEME KOŞULLARI").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, onaylanan siparişler için belirlenen ücretin tamamını Platform komisyonu düşüldükten sonra 3 (üç) iş günü içinde almayı kabul eder. Ödeme anlaşmazlıklarında Platform'un kayıtları esas alınır.");

                        col.Item().PaddingTop(8).Text("MADDE 5 — ÖDEME YAPILMAMASI HALİNDE UYGULANACAK YAPTIRIMLAR").Bold();
                        col.Item().Text("Hizmet Sağlayıcı'nın herhangi bir nedenle ödeme yükümlülüğünü yerine getirmemesi halinde; gecikilen her gün için aylık %5 oranında gecikme faizi uygulanır, hesabı askıya alınır ve yasal yollara başvurulabilir.");

                        col.Item().PaddingTop(8).Text("MADDE 6 — SİPARİŞ İPTALİ").Bold();
                        col.Item().Text("Onaylanan bir siparişin Hizmet Sağlayıcı tarafından haksız yere iptal edilmesi halinde, sipariş bedelinin %20'si oranında cezai şart uygulanır ve müşteriye tam iade yapılır.");

                        col.Item().PaddingTop(8).Text("MADDE 7 — KALİTE STANDARTLARI").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, sunduğu hizmetlerin gıda güvenliği standartlarına uygun olduğunu taahhüt eder. Aksi halde doğacak tüm hukuki ve mali sorumluluk Hizmet Sağlayıcı'ya aittir.");

                        col.Item().PaddingTop(8).Text("MADDE 8 — GİZLİLİK").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, Platform aracılığıyla edindiği müşteri bilgilerini üçüncü şahıslarla paylaşmamayı, ticari amaçla kullanmamayı kabul ve taahhüt eder.");

                        col.Item().PaddingTop(8).Text("MADDE 9 — REKABET YASAĞI").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, Platform üzerinden tanıştığı müşterilerle Platform dışında doğrudan ticari ilişki kurmamayı taahhüt eder. Aksinin tespiti halinde 3 aylık ortalama geliri tutarında cezai şart uygulanır.");

                        col.Item().PaddingTop(8).Text("MADDE 10 — PLATFORMUN SORUMLULUKLARI").Bold();
                        col.Item().Text("Platform, sipariş yönetimi, ödeme altyapısı ve müşteri iletişimi konularında Hizmet Sağlayıcı'ya destek sağlar. Teknik aksaklıklardan doğan gecikmeler Platform'un sorumluluğundadır.");

                        col.Item().PaddingTop(8).Text("MADDE 11 — SÖZLEŞMENİN SÜRESİ").Bold();
                        col.Item().Text("İşbu sözleşme imzalandığı tarihten itibaren 1 (bir) yıl süreyle geçerlidir. Taraflardan birinin sözleşmeyi feshetmemesi halinde aynı koşullarla 1 yıl daha uzatılmış sayılır.");

                        col.Item().PaddingTop(8).Text("MADDE 12 — FESİH").Bold();
                        col.Item().Text("Her iki taraf da 30 gün önceden yazılı bildirimde bulunmak kaydıyla sözleşmeyi feshedebilir. Hizmet Sağlayıcı'nın sözleşme ihlali halinde Platform tek taraflı fesih hakkına sahiptir.");

                        col.Item().PaddingTop(8).Text("MADDE 13 — UYUŞMAZLIK ÇÖZÜMÜ").Bold();
                        col.Item().Text("İşbu sözleşmeden doğan uyuşmazlıklarda öncelikle arabuluculuk yoluna başvurulur. Çözüme kavuşturulamazsa Ankara mahkemeleri ve icra daireleri yetkilidir.");

                        col.Item().PaddingTop(8).Text("MADDE 14 — DEĞİŞİKLİK").Bold();
                        col.Item().Text("Platform, sözleşme koşullarını 15 gün önceden Hizmet Sağlayıcı'ya bildirmek kaydıyla değiştirme hakkını saklı tutar. Hizmet Sağlayıcı'nın platformu kullanmaya devam etmesi değişiklikleri kabul ettiği anlamına gelir.");

                        col.Item().PaddingTop(8).Text("MADDE 15 — MÜCBİR SEBEPLER").Bold();
                        col.Item().Text("Doğal afet, salgın hastalık, savaş ve benzeri mücbir sebeplerden kaynaklanan aksaklıklar nedeniyle taraflardan herhangi biri yükümlülüklerini yerine getiremezse, bu durum sözleşme ihlali sayılmaz.");

                        col.Item().PaddingTop(8).Text("MADDE 16 — DEVİR YASAĞI").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, işbu sözleşmeden doğan hak ve yükümlülüklerini Platform'un yazılı onayı olmaksızın üçüncü şahıslara devredemez.");

                        col.Item().PaddingTop(8).Text("MADDE 17 — BAĞIMSIZ ÇALIŞMA").Bold();
                        col.Item().Text("Hizmet Sağlayıcı bağımsız bir yüklenici olarak faaliyet gösterir; Platform ile arasında işçi-işveren ilişkisi yoktur. Vergi ve SGK yükümlülükleri Hizmet Sağlayıcı'ya aittir.");

                        col.Item().PaddingTop(8).Text("MADDE 18 — HASAR VE KAYIPLAR").Bold();
                        col.Item().Text("Hizmet sunumu sırasında müşteri mülkünde oluşan hasar ve kayıplardan Hizmet Sağlayıcı sorumludur. Platform bu kapsamdaki tazminat taleplerinden muaftır.");

                        col.Item().PaddingTop(8).Text("MADDE 19 — SİGORTA").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, sunduğu hizmetler için gerekli mesleki sorumluluk sigortasını yaptırmayı ve geçerli tutmayı taahhüt eder.");

                        col.Item().PaddingTop(8).Text("MADDE 20 — REKLAM VE TANITIM").Bold();
                        col.Item().Text("Platform, Hizmet Sağlayıcı'nın adını ve hizmetlerini tanıtım amaçlı kullanabilir. Hizmet Sağlayıcı bu kullanıma onay verdiğini kabul eder.");

                        col.Item().PaddingTop(8).Text("MADDE 21 — MÜŞTERİ ŞİKAYETLERİ").Bold();
                        col.Item().Text("Müşteri şikayetleri önce Platform aracılığıyla iletilir. Hizmet Sağlayıcı, kendisine iletilen şikayetlere 48 saat içinde yanıt vermekle yükümlüdür.");

                        col.Item().PaddingTop(8).Text("MADDE 22 — MENÜ DOĞRULUĞU").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, Platform'da yayınlanan menü bilgilerinin (içerik, alerjen, fiyat) doğru ve güncel olduğunu taahhüt eder. Yanlış bilgiden doğan zararlar Hizmet Sağlayıcı'ya aittir.");

                        col.Item().PaddingTop(8).Text("MADDE 23 — HİJYEN VE SAĞLIK").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, gıda hazırlama ve sunum süreçlerinde yürürlükteki hijyen ve sağlık mevzuatına uymayı taahhüt eder. Denetim belgelerini talep halinde ibraz etmekle yükümlüdür.");

                        col.Item().PaddingTop(8).Text("MADDE 24 — VERİ GÜVENLİĞİ").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, Platform aracılığıyla işlenen kişisel verilerin güvenliğine ilişkin gerekli teknik ve idari tedbirleri almayı kabul eder.");

                        col.Item().PaddingTop(8).Text("MADDE 25 — PLATFORM KURALLARINA UYUM").Bold();
                        col.Item().Text("Hizmet Sağlayıcı, Platform'un yayımladığı kullanım kurallarına ve güncellemelerine uymayı kabul eder. Kural ihlali hesap askıya alınması veya silinmesiyle sonuçlanabilir.");

                        col.Item().PaddingTop(8).Text("MADDE 26 — TEBLİGAT ADRESİ").Bold();
                        col.Item().Text("Taraflar arasındaki resmi yazışmalar kayıtlı e-posta adresleri üzerinden yapılır. E-posta değişiklikleri 3 gün içinde karşı tarafa bildirilmelidir.");

                        col.Item().PaddingTop(8).Text("MADDE 27 — BÖLÜNEBILIRLIK").Bold();
                        col.Item().Text("İşbu sözleşmenin herhangi bir maddesinin geçersiz sayılması, diğer maddelerin geçerliliğini etkilemez.");

                        col.Item().PaddingTop(8).Text("MADDE 28 — FERAGAT").Bold();
                        col.Item().Text("Taraflardan birinin sözleşmeden doğan haklarını kullanmaması, bu haklardan feragat ettiği anlamına gelmez.");

                        col.Item().PaddingTop(8).Text("MADDE 29 — YÜRÜRLÜK").Bold();
                        col.Item().Text("İşbu sözleşme, Hizmet Sağlayıcı'nın onay vermesiyle birlikte yürürlüğe girer ve her iki taraf için bağlayıcı nitelik kazanır.");

                        col.Item().PaddingTop(8).Text("MADDE 30 — TARAFLARIN BEYANI").Bold();
                        col.Item().Text("Her iki taraf da işbu sözleşmeyi özgür iradeleriyle, tüm maddelerini okuyup anlayarak kabul ettiklerini beyan eder.");

                        col.Item().PaddingTop(20).LineHorizontal(1).LineColor("#dddddd");
                        col.Item().PaddingTop(15).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("tastemam Catering").Bold();
                                c.Item().Text("Platform Yetkilisi");
                                c.Item().PaddingTop(20).Text("İmza: _______________");
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Hizmet Sağlayıcı").Bold();
                                c.Item().Text(caretakerEmail);
                                c.Item().PaddingTop(20).Text("İmza: _______________");
                                c.Item().Text($"Tarih: {date:dd.MM.yyyy}");
                            });
                        });
                    });

                    page.Footer().AlignCenter()
                        .Text("tastemam Catering — tastemam.noreply@gmail.com")
                        .FontSize(9).FontColor("#aaaaaa");
                });
            }).GeneratePdf();
        }
    }
}