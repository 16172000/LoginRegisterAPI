using ClosedXML.Excel;

namespace TicketProjectWEB.Models
{
    public class ExcelFileHandling
    {
        public MemoryStream CreateExcelFile(List<Employee> employees)
        {
            //Create an Instance of Workbook, i.e., Creates a new Excel workbook
            var workbook = new XLWorkbook();
            //Add a Worksheets with the workbook
            //Worksheets name is Employees
            IXLWorksheet worksheet = workbook.Worksheets.Add("Employees");
            //Create the Cell
            //First Row is going to be Header Row
            worksheet.Cell(1, 1).Value = "ID"; //First Row and First Column
            worksheet.Cell(1, 2).Value = "Name"; //First Row and Second Column
            worksheet.Cell(1, 3).Value = "Designation"; //First Row and Third Column
            worksheet.Cell(1, 4).Value = "Age"; //First Row and Fourth Column
            worksheet.Cell(1, 5).Value = "City"; //First Row and Fifth Column
            worksheet.Cell(1, 6).Value = "Salary"; //First Row and Sixth Column
            worksheet.Cell(1, 7).Value = "File"; //First Row and Sixth Column
            //Data is going to stored from Row 2
            int row = 2;
            //Loop Through Each Employees and Populate the worksheet
            //For Each Employee increase row by 1
            foreach (var emp in employees)
            {
                worksheet.Cell(row, 1).Value = emp.Id;
                worksheet.Cell(row, 2).Value = emp.Name;
                worksheet.Cell(row, 3).Value = emp.Designation;
                worksheet.Cell(row, 4).Value = emp.Age;
                worksheet.Cell(row, 5).Value = emp.City;
                worksheet.Cell(row, 6).Value = emp.Salary;
                worksheet.Cell(row, 7).Value = emp.File;
                //worksheet.Cell(row, 7).Value = "Attachment: " + (emp.File != null ? emp.File.FileName : "No file attached");
                row++; //Increasing the Data Row by 1
            }
            //Create an Memory Stream Object
            var stream = new MemoryStream();
            //Saves the current workbook to the Memory Stream Object.
            workbook.SaveAs(stream);
            //The Position property gets or sets the current position within the stream.
            //This is the next position a read, write, or seek operation will occur from.
            stream.Position = 0;
            return stream;
        }
    }
}
