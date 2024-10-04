using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace App.Models
{
    [Table("RentalProperty")]
    public class RentalProperty
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên tài sản là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên tài sản không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên Tài Sản", Prompt = "Nhập tên tài sản...")]
        public string PropertyName { get; set; }

        [Required(ErrorMessage = "Tên chủ sở hữu là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên chủ sở hữu không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên Chủ Sở Hữu", Prompt = "Nhập tên chủ sở hữu...")]
        public string OwnerName { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(512, ErrorMessage = "Địa chỉ không được vượt quá 512 ký tự.")]
        [Display(Name = "Địa Chỉ", Prompt = "Nhập địa chỉ...")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Số phòng là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số phòng phải lớn hơn 0.")]
        [Display(Name = "Số Phòng", Prompt = "Nhập số phòng...")]
        public int NumberOfRooms { get; set; }

        [Required(ErrorMessage = "Diện tích tổng là bắt buộc.")]
        [Range(1, double.MaxValue, ErrorMessage = "Diện tích phải lớn hơn 0.")]
        [Display(Name = "Diện Tích Tổng", Prompt = "Nhập diện tích tổng?...")]
        public double TotalArea { get; set; }

        [Required(ErrorMessage = "Loại tài sản là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Loại tài sản không được vượt quá 50 ký tự.")]
        [Display(Name = "Loại Tài Sản", Prompt = "Nhập loại tài sản...")]
        public string PropertyType { get; set; }

        [Display(Name = "Tiện Nghi", Prompt = "Nhập các tiện nghi...")]
        public string Facilities { get; set; }

        [Required(ErrorMessage = "Giá điện là bắt buộc.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Giá Điện", Prompt = "Nhập giá điện...")]
        public decimal ElectricityPrice { get; set; }

        [Required(ErrorMessage = "Giá nước là bắt buộc.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Giá Nước", Prompt = "Nhập giá nước...")]
        public decimal WaterPrice { get; set; }

        [Required(ErrorMessage = "Số điện thoại chủ sở hữu là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Số Điện Thoại Chủ Sở Hữu", Prompt = "Nhập số điện thoại chủ sở hữu...")]
        public string OwnerPhoneNumber { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày Bắt Đầu", Prompt = "Nhập ngày bắt đầu...")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Hình Ảnh Tài Sản", Prompt = "Chọn đường dẫn đến hình ảnh tài sản...")]
        public string? PropertyImage { get; set; }

        [Display(Name = "Trạng Thái Hoạt Động")]
        public bool IsActive { get; set; }

        [Display(Name = "Người Dùng Ứng Dụng")]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        [Display(Name = "Danh Sách Phòng")]
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
