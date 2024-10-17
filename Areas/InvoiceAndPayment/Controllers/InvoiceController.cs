using App.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Payment
{
    [Area("InvoiceAndPayment")]
    public class InvoiceController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public InvoiceController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [Route("/invoice")]
        public IActionResult Index()
        {
            return View();
        }

        // Get list invoice
        [Route("/get-list-invoice/{homeId:int?}")]
        public IActionResult GetListInvoice(int? homeId)
        {
            if (homeId == null)
            {
                return NotFound("Mã nhà không hợp lệ.");
            }

            // Kiểm tra xem nhà có tồn tại không
            var homeExist = _appDbContext.RentalProperties.FirstOrDefault(r => r.Id == homeId);
            if (homeExist == null)
            {
                return NotFound("Nhà không tồn tại.");
            }

            // Lấy danh sách hóa đơn dựa trên homeId
            var invoices = _appDbContext.Invoices
                .Where(i => i.RoomId == homeId)
                .Join(_appDbContext.Rooms, // Thực hiện join với bảng Rooms
                      invoice => invoice.RoomId, // Khóa ngoại từ bảng Invoices (RoomId)
                      room => room.Id, // Khóa chính từ bảng Rooms (Id)
                      (invoice, room) => new // Kết quả truy vấn bao gồm dữ liệu từ cả hai bảng
                      {
                          invoice.Id, // Mã hóa đơn
                          invoice.RoomId, // Mã nhà
                          invoice.InvoiceDate, // Ngày hóa đơn
                          invoice.PaymentDate,
                          invoice.AdditionalServiceFee, // Phí dịch vụ thêm
                          invoice.WaterUsage, // Sử dụng nước
                          RoomName = room.RoomName // Tên phòng từ bảng Rooms
                      })
                .ToList();

            // Nếu không có hóa đơn nào
            if (!invoices.Any())
            {
                return NotFound("Không có hóa đơn nào cho nhà này.");
            }

            // Trả về danh sách hóa đơn dưới dạng JSON
            return Ok(invoices);

        }
    }
}