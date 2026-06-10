using System.Text;
using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class ReportExportService
{
    private readonly ReportService _reportService;

    public ReportExportService(ReportService reportService) => _reportService = reportService;

    public async Task<byte[]> GenerateCsvAsync()
    {
        var report = await _reportService.GetSummaryAsync();
        var sb = new StringBuilder();

        sb.AppendLine("Dom Zdravlja - Izvjestaj");
        sb.AppendLine($"Datum,{DateTime.Now:dd.MM.yyyy HH:mm}");
        sb.AppendLine();
        sb.AppendLine("Pregled");
        sb.AppendLine($"Ukupno lijekova,{report.TotalMedicines}");
        sb.AppendLine($"Niska zaliha,{report.LowStockCount}");
        sb.AppendLine($"Istekli lijekovi,{report.ExpiredCount}");
        sb.AppendLine($"Istice uskoro,{report.ExpiringSoonCount}");
        sb.AppendLine($"Zahtjevi na cekanju,{report.PendingRequests}");
        sb.AppendLine($"Odobreni zahtjevi,{report.ApprovedRequests}");
        sb.AppendLine($"Izdati zahtjevi,{report.DeliveredRequests}");
        sb.AppendLine($"Odbijeni zahtjevi,{report.RejectedRequests}");
        sb.AppendLine();
        sb.AppendLine("Stanje zaliha");
        sb.AppendLine("Lijek,Kolicina,Jedinica,Min. zaliha,Rok trajanja,Status");

        foreach (var item in report.MedicineStock)
        {
            var status = item.IsExpired ? "Istekao" : item.IsExpiringSoon ? "Istice uskoro" : item.IsLowStock ? "Niska zaliha" : "OK";
            var expiry = item.ExpiryDate?.ToString("dd.MM.yyyy") ?? "-";
            sb.AppendLine($"{Escape(item.Name)},{item.Quantity},{Escape(item.Unit)},{item.MinimumStock},{expiry},{status}");
        }

        sb.AppendLine();
        sb.AppendLine("Zahtjevi po ambulantama");
        sb.AppendLine("Ambulanta,Ukupno,Izdato");

        foreach (var item in report.RequestsByAmbulance)
            sb.AppendLine($"{Escape(item.AmbulanceName)},{item.TotalRequests},{item.DeliveredCount}");

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    public async Task<string> GenerateHtmlReportAsync()
    {
        var report = await _reportService.GetSummaryAsync();
        var rows = string.Join("", report.MedicineStock.Select(m =>
        {
            var status = m.IsExpired ? "Istekao" : m.IsExpiringSoon ? "Istice uskoro" : m.IsLowStock ? "Niska zaliha" : "OK";
            var expiry = m.ExpiryDate?.ToString("dd.MM.yyyy") ?? "-";
            return $"<tr><td>{m.Name}</td><td>{m.Quantity} {m.Unit}</td><td>{expiry}</td><td>{status}</td></tr>";
        }));

        var ambulanceRows = string.Join("", report.RequestsByAmbulance.Select(a =>
            $"<tr><td>{a.AmbulanceName}</td><td>{a.TotalRequests}</td><td>{a.DeliveredCount}</td></tr>"));

        var date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        return "<!DOCTYPE html><html lang=\"bs\"><head><meta charset=\"UTF-8\">" +
               "<title>Izvjestaj - Dom Zdravlja</title>" +
               "<style>body{font-family:Arial,sans-serif;margin:2rem;color:#134e4a}" +
               "h1{color:#0f766e}table{width:100%;border-collapse:collapse;margin:1rem 0}" +
               "th,td{border:1px solid #99f6e4;padding:0.5rem;text-align:left}th{background:#ccfbf1}" +
               ".stats{display:grid;grid-template-columns:repeat(4,1fr);gap:1rem;margin:1rem 0}" +
               ".stat{border:1px solid #99f6e4;padding:1rem;border-radius:8px;text-align:center}" +
               ".stat strong{display:block;font-size:1.5rem}</style></head><body>" +
               "<h1>Dom Zdravlja — Izvještaj</h1>" +
               $"<p>Datum: {date}</p>" +
               "<div class=\"stats\">" +
               $"<div class=\"stat\"><strong>{report.TotalMedicines}</strong>Lijekova</div>" +
               $"<div class=\"stat\"><strong>{report.LowStockCount}</strong>Niska zaliha</div>" +
               $"<div class=\"stat\"><strong>{report.ExpiredCount}</strong>Istekli</div>" +
               $"<div class=\"stat\"><strong>{report.DeliveredRequests}</strong>Izdato</div></div>" +
               "<h2>Stanje zaliha</h2><table><thead><tr><th>Lijek</th><th>Količina</th><th>Rok</th><th>Status</th></tr></thead>" +
               $"<tbody>{rows}</tbody></table>" +
               "<h2>Zahtjevi po ambulantama</h2><table><thead><tr><th>Ambulanta</th><th>Ukupno</th><th>Izdato</th></tr></thead>" +
               $"<tbody>{ambulanceRows}</tbody></table>" +
               "<p><em>Za PDF: Ctrl+P → Sačuvaj kao PDF</em></p></body></html>";
    }

    private static string Escape(string value) =>
        value.Contains(',') || value.Contains('"') ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
}
