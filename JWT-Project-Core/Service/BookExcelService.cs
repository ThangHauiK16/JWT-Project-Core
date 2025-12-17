using ClosedXML.Excel;
using JWT_Project_Core.Interface;

namespace JWT_Project_Core.Service
{
    public class BookExcelService : IBookExcelService
    {
        public byte[] GenerateImportTemplate()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("BookImportTemplate");

            worksheet.Cell(1, 1).Value = "MaSach";
            worksheet.Cell(1, 2).Value = "TenSach";
            worksheet.Cell(1, 3).Value = "TheLoai";
            worksheet.Cell(1, 4).Value = "GiaNhap";
            worksheet.Cell(1, 5).Value = "GiaBan";
            worksheet.Cell(1, 6).Value = "TenTacGia";
            worksheet.Cell(1, 7).Value = "NoiDungSach";
            worksheet.Cell(1, 8).Value = "SoLuong";

            var header = worksheet.Range(1, 1, 1, 8);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(2, 1).Value = "MS001";
            worksheet.Cell(2, 2).Value = "Lập trình C# cơ bản";
            worksheet.Cell(2, 3).Value = "CNTT";
            worksheet.Cell(2, 4).Value = 50000;
            worksheet.Cell(2, 5).Value = 80000;
            worksheet.Cell(2, 6).Value = "Nguyễn Văn A";
            worksheet.Cell(2, 7).Value = "Sách dành cho người mới học";
            worksheet.Cell(2, 8).Value = 100;

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
