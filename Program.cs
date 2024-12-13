using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DinhDanhDienTu
{
    // Lớp Tài Khoản Định Danh
    public class TaiKhoanDinhDanh
    {
        // Thông tin định danh cơ bản
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string SoCanCuoc { get; set; }

        // Thông tin cá nhân
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }

        // Thông tin xác thực và trạng thái
        public DateTime NgayTao { get; set; }
        public DateTime NgayHetHan { get; set; }
        public bool DangHoatDong { get; set; }
        public int SoLanDangNhapSai { get; set; }
        public List<DateTime> LichSuDangNhap { get; set; }

        public TaiKhoanDinhDanh()
        {
            LichSuDangNhap = new List<DateTime>();
        }

        public override string ToString() => $"{TenDangNhap} - {HoTen} - {SoCanCuoc}";

        public bool DaHetHan() => NgayHetHan < DateTime.Now;
        public bool BiKhoa() => SoLanDangNhapSai >= 5;
        public bool CoTheDangNhap() => DangHoatDong && !DaHetHan() && !BiKhoa();
    }

    // Lớp Nút Cây Nhị Phân
    public class NutCayNhiPhan
    {
        public TaiKhoanDinhDanh TaiKhoan { get; set; }
        public NutCayNhiPhan Trai { get; set; }
        public NutCayNhiPhan Phai { get; set; }

        public NutCayNhiPhan(TaiKhoanDinhDanh taiKhoan)
        {
            TaiKhoan = taiKhoan;
        }
    }

    // Lớp Cây Nhị Phân Định Danh
    public class CayNhiPhanDinhDanh
    {
        public NutCayNhiPhan Goc { get; private set; }

        // Thêm tài khoản mới
        public void Them(TaiKhoanDinhDanh taiKhoan)
        {
            Goc = ThemDeQuy(Goc, taiKhoan);
        }

        private NutCayNhiPhan ThemDeQuy(NutCayNhiPhan nut, TaiKhoanDinhDanh taiKhoan)
        {
            if (nut == null) return new NutCayNhiPhan(taiKhoan);

            int soSanh = string.Compare(taiKhoan.TenDangNhap, nut.TaiKhoan.TenDangNhap, StringComparison.OrdinalIgnoreCase);

            if (soSanh < 0)
                nut.Trai = ThemDeQuy(nut.Trai, taiKhoan);
            else if (soSanh > 0)
                nut.Phai = ThemDeQuy(nut.Phai, taiKhoan);

            return nut;
        }

        // Tìm kiếm tài khoản theo tên đăng nhập
        public TaiKhoanDinhDanh TimKiem(string tenDangNhap)
        {
            return TimKiemDeQuy(Goc, tenDangNhap);
        }

        private TaiKhoanDinhDanh TimKiemDeQuy(NutCayNhiPhan nut, string tenDangNhap)
        {
            if (nut == null) return null;

            int soSanh = string.Compare(tenDangNhap, nut.TaiKhoan.TenDangNhap, StringComparison.OrdinalIgnoreCase);

            if (soSanh == 0) return nut.TaiKhoan;
            if (soSanh < 0) return TimKiemDeQuy(nut.Trai, tenDangNhap);
            return TimKiemDeQuy(nut.Phai, tenDangNhap);
        }

        // Xác thực tài khoản
        public bool XacThucTaiKhoan(string tenDangNhap, string matKhau)
        {
            var taiKhoan = TimKiem(tenDangNhap);
            if (taiKhoan == null) return false;

            if (!taiKhoan.CoTheDangNhap())
                return false;

            if (taiKhoan.MatKhau != matKhau)
            {
                taiKhoan.SoLanDangNhapSai++;
                return false;
            }

            taiKhoan.SoLanDangNhapSai = 0;
            taiKhoan.LichSuDangNhap.Add(DateTime.Now);
            return true;
        }

        // Tìm kiếm theo CCCD
        public TaiKhoanDinhDanh TimKiemTheoCCCD(string soCanCuoc)
        {
            var danhSach = DuyetTheoThuTu();
            return danhSach.FirstOrDefault(x => x.SoCanCuoc == soCanCuoc);
        }

        // Duyệt cây theo thứ tự
        public List<TaiKhoanDinhDanh> DuyetTheoThuTu()
        {
            List<TaiKhoanDinhDanh> danhSach = new List<TaiKhoanDinhDanh>();
            DuyetTheoThuTuDeQuy(Goc, danhSach);
            return danhSach;
        }

        private void DuyetTheoThuTuDeQuy(NutCayNhiPhan nut, List<TaiKhoanDinhDanh> danhSach)
        {
            if (nut == null) return;

            DuyetTheoThuTuDeQuy(nut.Trai, danhSach);
            danhSach.Add(nut.TaiKhoan);
            DuyetTheoThuTuDeQuy(nut.Phai, danhSach);
        }

        // Tìm kiếm theo tên
        public List<TaiKhoanDinhDanh> TimKiemTheoTen(string ten)
        {
            List<TaiKhoanDinhDanh> ketQua = new List<TaiKhoanDinhDanh>();
            TimKiemTheoTenDeQuy(Goc, ten, ketQua);
            return ketQua;
        }

        private void TimKiemTheoTenDeQuy(NutCayNhiPhan nut, string ten, List<TaiKhoanDinhDanh> ketQua)
        {
            if (nut == null) return;

            TimKiemTheoTenDeQuy(nut.Trai, ten, ketQua);

            if (nut.TaiKhoan.HoTen.ToLower().Contains(ten.ToLower()))
            {
                ketQua.Add(nut.TaiKhoan);
            }

            TimKiemTheoTenDeQuy(nut.Phai, ten, ketQua);
        }

        // Tìm kiếm theo khoảng thời gian tạo
        public List<TaiKhoanDinhDanh> TimKiemTheoKhoangThoiGian(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            List<TaiKhoanDinhDanh> ketQua = new List<TaiKhoanDinhDanh>();
            TimKiemTheoKhoangThoiGianDeQuy(Goc, ngayBatDau, ngayKetThuc, ketQua);
            return ketQua;
        }

        private void TimKiemTheoKhoangThoiGianDeQuy(NutCayNhiPhan nut, DateTime ngayBatDau, DateTime ngayKetThuc, List<TaiKhoanDinhDanh> ketQua)
        {
            if (nut == null) return;

            TimKiemTheoKhoangThoiGianDeQuy(nut.Trai, ngayBatDau, ngayKetThuc, ketQua);

            if (nut.TaiKhoan.NgayTao.Date >= ngayBatDau.Date &&
                nut.TaiKhoan.NgayTao.Date <= ngayKetThuc.Date)
            {
                ketQua.Add(nut.TaiKhoan);
            }

            TimKiemTheoKhoangThoiGianDeQuy(nut.Phai, ngayBatDau, ngayKetThuc, ketQua);
        }
    }
}
namespace DinhDanhDienTu
{
    public partial class FormDangNhap : Form
    {
        private readonly CayNhiPhanDinhDanh _cayDinhDanh;
        private TextBox txtTenDangNhap;
        private TextBox txtMatKhau;
        private Button btnDangNhap;
        private Label lblTenDangNhap;
        private Label lblMatKhau;
        private Label lblThongBao;

        public FormDangNhap(CayNhiPhanDinhDanh cayDinhDanh)
        {
            _cayDinhDanh = cayDinhDanh;
            KhoiTaoThanhPhan();
        }

        private void KhoiTaoThanhPhan()
        {
            this.lblTenDangNhap = new Label
            {
                Text = "Tên đăng nhập:",
                Location = new Point(50, 30),
                AutoSize = true
            };

            this.lblMatKhau = new Label
            {
                Text = "Mật khẩu:",
                Location = new Point(50, 80),
                AutoSize = true
            };

            this.txtTenDangNhap = new TextBox
            {
                Location = new Point(50, 50),
                Width = 200
            };

            this.txtMatKhau = new TextBox
            {
                Location = new Point(50, 100),
                Width = 200,
                UseSystemPasswordChar = true
            };

            this.btnDangNhap = new Button
            {
                Text = "Đăng nhập",
                Location = new Point(50, 150),
                Width = 100
            };

            this.lblThongBao = new Label
            {
                Location = new Point(50, 190),
                AutoSize = true,
                ForeColor = Color.Red
            };

            this.btnDangNhap.Click += btnDangNhap_Click;

            this.Controls.Add(lblTenDangNhap);
            this.Controls.Add(lblMatKhau);
            this.Controls.Add(txtTenDangNhap);
            this.Controls.Add(txtMatKhau);
            this.Controls.Add(btnDangNhap);
            this.Controls.Add(lblThongBao);

            this.Text = "Hệ thống Định danh điện tử - Đăng nhập";
            this.Size = new Size(350, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CaiDatPlaceholder(txtTenDangNhap, "Nhập tên đăng nhập");
            CaiDatPlaceholder(txtMatKhau, "Nhập mật khẩu");
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtTenDangNhap.Text;
            string matKhau = txtMatKhau.Text;

            var taiKhoan = _cayDinhDanh.TimKiem(tenDangNhap);

            if (taiKhoan == null)
            {
                lblThongBao.Text = "Tài khoản không tồn tại!";
                return;
            }

            if (taiKhoan.DaHetHan())
            {
                lblThongBao.Text = "Tài khoản đã hết hạn sử dụng!";
                return;
            }

            if (taiKhoan.BiKhoa())
            {
                lblThongBao.Text = "Tài khoản đã bị khóa do đăng nhập sai nhiều lần!";
                return;
            }

            if (!taiKhoan.DangHoatDong)
            {
                lblThongBao.Text = "Tài khoản đã bị vô hiệu hóa!";
                return;
            }

            if (_cayDinhDanh.XacThucTaiKhoan(tenDangNhap, matKhau))
            {
                MessageBox.Show($"Xác thực thành công!\nXin chào {taiKhoan.HoTen}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Hide();
                new FormQuanLyDinhDanh(_cayDinhDanh).ShowDialog();
                Close();
            }
            else
            {
                int lanConLai = 5 - taiKhoan.SoLanDangNhapSai;
                if (lanConLai > 0)
                    lblThongBao.Text = $"Sai mật khẩu! Còn {lanConLai} lần thử";
                else
                    lblThongBao.Text = "Tài khoản đã bị khóa do đăng nhập sai nhiều lần!";
            }
        }

        private void CaiDatPlaceholder(TextBox textBox, string giaTriMacDinh)
        {
            textBox.Text = giaTriMacDinh;
            textBox.ForeColor = Color.Gray;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == giaTriMacDinh)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = giaTriMacDinh;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }
    }

    // Form Quản lý Định danh
    public partial class FormQuanLyDinhDanh : Form
    {
        private readonly CayNhiPhanDinhDanh _cayDinhDanh;
        private DataGridView gridDanhSach;
        private Panel panelHienThiCay;
        private Button btnHienThiCay;

        // Controls thêm tài khoản mới
        private TextBox txtThemTenDangNhap;
        private TextBox txtThemMatKhau;
        private TextBox txtThemCCCD;
        private TextBox txtThemHoTen;
        private DateTimePicker dtpThemNgaySinh;
        private TextBox txtThemDiaChi;
        private TextBox txtThemEmail;
        private DateTimePicker dtpThemNgayHetHan;
        private Button btnThemTaiKhoan;

        // Controls tìm kiếm
        private TextBox txtTimKiemCCCD;
        private Button btnTimKiemCCCD;
        private TextBox txtTimKiemTheoTen;
        private Button btnTimKiemTheoTen;
        private DateTimePicker dtpNgayBatDau;
        private DateTimePicker dtpNgayKetThuc;
        private Button btnTimKiemTheoNgay;
        private Button btnLamMoi;
        private Button btnKhoaTaiKhoan;
        private Button btnMoKhoaTaiKhoan;
        private Button btnXemLichSu;

        public FormQuanLyDinhDanh(CayNhiPhanDinhDanh cayDinhDanh)
        {
            _cayDinhDanh = cayDinhDanh;
            KhoiTaoThanhPhan();
        }

        private void KhoiTaoThanhPhan()
        {
            // Khởi tạo DataGridView
            this.gridDanhSach = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(800, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.gridDanhSach.ColumnCount = 9;
            this.gridDanhSach.Columns[0].HeaderText = "Tên đăng nhập";
            this.gridDanhSach.Columns[1].HeaderText = "Số CCCD";
            this.gridDanhSach.Columns[2].HeaderText = "Họ tên";
            this.gridDanhSach.Columns[3].HeaderText = "Ngày sinh";
            this.gridDanhSach.Columns[4].HeaderText = "Địa chỉ";
            this.gridDanhSach.Columns[5].HeaderText = "Email";
            this.gridDanhSach.Columns[6].HeaderText = "Ngày tạo";
            this.gridDanhSach.Columns[7].HeaderText = "Ngày hết hạn";
            this.gridDanhSach.Columns[8].HeaderText = "Trạng thái";

            // Khởi tạo Panel hiển thị cây
            this.panelHienThiCay = new Panel
            {
                Location = new Point(20, 250),
                Size = new Size(800, 300),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.btnHienThiCay = new Button
            {
                Text = "Hiển thị cấu trúc cây",
                Location = new Point(20, 560),
                Width = 150
            };
            this.btnHienThiCay.Click += btnHienThiCay_Click;

            // Khởi tạo controls thêm tài khoản mới
            int yPos = 600;

            this.txtThemTenDangNhap = new TextBox { Location = new Point(20, yPos), Width = 120, PlaceholderText = "Tên đăng nhập" };
            this.txtThemMatKhau = new TextBox { Location = new Point(150, yPos), Width = 120, PlaceholderText = "Mật khẩu", UseSystemPasswordChar = true };
            this.txtThemCCCD = new TextBox { Location = new Point(280, yPos), Width = 120, PlaceholderText = "Số CCCD" };
            this.txtThemHoTen = new TextBox { Location = new Point(410, yPos), Width = 120, PlaceholderText = "Họ tên" };

            yPos += 30;

            this.dtpThemNgaySinh = new DateTimePicker { Location = new Point(20, yPos), Width = 120, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
            this.txtThemDiaChi = new TextBox { Location = new Point(150, yPos), Width = 250, PlaceholderText = "Địa chỉ" };
            this.txtThemEmail = new TextBox { Location = new Point(410, yPos), Width = 120, PlaceholderText = "Email" };
            this.dtpThemNgayHetHan = new DateTimePicker { Location = new Point(540, yPos), Width = 120, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };

            this.btnThemTaiKhoan = new Button { Text = "Thêm tài khoản", Location = new Point(670, yPos), Width = 120 };
            this.btnThemTaiKhoan.Click += btnThemTaiKhoan_Click;

            // Khởi tạo controls tìm kiếm
            yPos += 50;

            this.txtTimKiemCCCD = new TextBox { Location = new Point(20, yPos), Width = 150, PlaceholderText = "Tìm theo CCCD" };
            this.btnTimKiemCCCD = new Button { Text = "Tìm CCCD", Location = new Point(180, yPos), Width = 100 };
            this.btnTimKiemCCCD.Click += btnTimKiemCCCD_Click;

            this.txtTimKiemTheoTen = new TextBox { Location = new Point(290, yPos), Width = 150, PlaceholderText = "Tìm theo tên" };
            this.btnTimKiemTheoTen = new Button { Text = "Tìm tên", Location = new Point(450, yPos), Width = 100 };
            this.btnTimKiemTheoTen.Click += btnTimKiemTheoTen_Click;

            yPos += 30;

            this.dtpNgayBatDau = new DateTimePicker
            {
                Location = new Point(20, yPos),
                Width = 150,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };
            this.dtpNgayKetThuc = new DateTimePicker
            {
                Location = new Point(180, yPos),
                Width = 150,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };
            this.btnTimKiemTheoNgay = new Button { Text = "Tìm theo ngày", Location = new Point(340, yPos), Width = 120 };
            this.btnTimKiemTheoNgay.Click += btnTimKiemTheoNgay_Click;

            // Các nút chức năng khác
            this.btnLamMoi = new Button { Text = "Làm mới", Location = new Point(470, yPos), Width = 100 };
            this.btnLamMoi.Click += btnLamMoi_Click;

            this.btnKhoaTaiKhoan = new Button { Text = "Khóa TK", Location = new Point(580, yPos), Width = 100 };
            this.btnKhoaTaiKhoan.Click += btnKhoaTaiKhoan_Click;

            this.btnMoKhoaTaiKhoan = new Button { Text = "Mở khóa TK", Location = new Point(690, yPos), Width = 100 };
            this.btnMoKhoaTaiKhoan.Click += btnMoKhoaTaiKhoan_Click;

            yPos += 30;
            this.btnXemLichSu = new Button { Text = "Xem lịch sử đăng nhập", Location = new Point(20, yPos), Width = 150 };
            this.btnXemLichSu.Click += btnXemLichSu_Click;

            // Thêm tất cả controls vào form
            this.Controls.AddRange(new Control[] {
                gridDanhSach, panelHienThiCay, btnHienThiCay,
                txtThemTenDangNhap, txtThemMatKhau, txtThemCCCD, txtThemHoTen,
                dtpThemNgaySinh, txtThemDiaChi, txtThemEmail, dtpThemNgayHetHan, btnThemTaiKhoan,
                txtTimKiemCCCD, btnTimKiemCCCD, txtTimKiemTheoTen, btnTimKiemTheoTen,
                dtpNgayBatDau, dtpNgayKetThuc, btnTimKiemTheoNgay,
                btnLamMoi, btnKhoaTaiKhoan, btnMoKhoaTaiKhoan, btnXemLichSu
            });

            this.Text = "Quản lý Định danh điện tử";
            this.Size = new Size(850, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            HienThiDanhSach();
        }

        // Tiếp tục với các phương thức xử lý sự kiện
        private void btnHienThiCay_Click(object sender, EventArgs e)
        {
            // Xóa và thiết lập lại panel
            panelHienThiCay.Controls.Clear();
            panelHienThiCay.Refresh();

            // Tạo bitmap có kích thước bằng với panel để vẽ
            using (Bitmap bmp = new Bitmap(panelHienThiCay.Width, panelHienThiCay.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);

                    // Tính toán số nút để điều chỉnh khoảng cách phù hợp
                    int soNut = _cayDinhDanh.DuyetTheoThuTu().Count;
                    int khoangCachNgang = panelHienThiCay.Width / (soNut + 1);

                    // Bắt đầu vẽ từ giữa panel phía trên
                    VeCay(g, _cayDinhDanh.Goc, panelHienThiCay.Width / 2, 30, khoangCachNgang);
                }

                // Vẽ bitmap lên panel
                panelHienThiCay.CreateGraphics().DrawImage(bmp, 0, 0);
            }
        }

        private void VeCay(Graphics g, NutCayNhiPhan nut, int x, int y, int khoangCach)
        {
            if (nut == null) return;

            // Tăng kích thước nút
            int nutWidth = 80;
            int nutHeight = 40;

            // Vẽ nút hiện tại
            g.FillEllipse(Brushes.LightBlue, x - nutWidth / 2, y - nutHeight / 2, nutWidth, nutHeight);
            g.DrawEllipse(Pens.Black, x - nutWidth / 2, y - nutHeight / 2, nutWidth, nutHeight);

            // Vẽ text trong nút
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(nut.TaiKhoan.TenDangNhap,
                    new Font("Arial", 8),
                    Brushes.Black,
                    new RectangleF(x - nutWidth / 2, y - nutHeight / 2, nutWidth, nutHeight),
                    sf);
            }

            // Điều chỉnh khoảng cách giữa các tầng của cây
            int yOffset = 80;

            // Vẽ nút con bên trái
            if (nut.Trai != null)
            {
                g.DrawLine(Pens.Black,
                    x, y + nutHeight / 2,
                    x - khoangCach, y + yOffset - nutHeight / 2);
                VeCay(g, nut.Trai, x - khoangCach, y + yOffset, khoangCach / 2);
            }

            // Vẽ nút con bên phải
            if (nut.Phai != null)
            {
                g.DrawLine(Pens.Black,
                    x, y + nutHeight / 2,
                    x + khoangCach, y + yOffset - nutHeight / 2);
                VeCay(g, nut.Phai, x + khoangCach, y + yOffset, khoangCach / 2);
            }
        }

        private void btnThemTaiKhoan_Click(object sender, EventArgs e)
        {
            if (KiemTraDuLieuNhap())
            {
                TaiKhoanDinhDanh taiKhoanMoi = new TaiKhoanDinhDanh
                {
                    TenDangNhap = txtThemTenDangNhap.Text,
                    MatKhau = txtThemMatKhau.Text,
                    SoCanCuoc = txtThemCCCD.Text,
                    HoTen = txtThemHoTen.Text,
                    NgaySinh = dtpThemNgaySinh.Value,
                    DiaChi = txtThemDiaChi.Text,
                    Email = txtThemEmail.Text,
                    NgayTao = DateTime.Now,
                    NgayHetHan = dtpThemNgayHetHan.Value,
                    DangHoatDong = true,
                    SoLanDangNhapSai = 0
                };

                _cayDinhDanh.Them(taiKhoanMoi);
                HienThiDanhSach();
                XoaDuLieuNhap();
                MessageBox.Show("Thêm tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool KiemTraDuLieuNhap()
        {
            if (string.IsNullOrWhiteSpace(txtThemTenDangNhap.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtThemMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtThemCCCD.Text))
            {
                MessageBox.Show("Vui lòng nhập số CCCD!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtThemHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (dtpThemNgayHetHan.Value <= DateTime.Now)
            {
                MessageBox.Show("Ngày hết hạn phải lớn hơn ngày hiện tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra tài khoản đã tồn tại
            if (_cayDinhDanh.TimKiem(txtThemTenDangNhap.Text) != null)
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra CCCD đã tồn tại
            if (_cayDinhDanh.TimKiemTheoCCCD(txtThemCCCD.Text) != null)
            {
                MessageBox.Show("Số CCCD đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void XoaDuLieuNhap()
        {
            txtThemTenDangNhap.Clear();
            txtThemMatKhau.Clear();
            txtThemCCCD.Clear();
            txtThemHoTen.Clear();
            txtThemDiaChi.Clear();
            txtThemEmail.Clear();
            dtpThemNgaySinh.Value = DateTime.Now;
            dtpThemNgayHetHan.Value = DateTime.Now.AddYears(1);
        }

        private void btnTimKiemCCCD_Click(object sender, EventArgs e)
        {
            string cccd = txtTimKiemCCCD.Text.Trim();
            if (string.IsNullOrEmpty(cccd))
            {
                MessageBox.Show("Vui lòng nhập số CCCD cần tìm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var taiKhoan = _cayDinhDanh.TimKiemTheoCCCD(cccd);
            if (taiKhoan != null)
            {
                List<TaiKhoanDinhDanh> ketQua = new List<TaiKhoanDinhDanh> { taiKhoan };
                CapNhatDataGridView(ketQua);
            }
            else
            {
                MessageBox.Show("Không tìm thấy tài khoản với số CCCD này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnTimKiemTheoTen_Click(object sender, EventArgs e)
        {
            string ten = txtTimKiemTheoTen.Text.Trim();
            if (string.IsNullOrEmpty(ten))
            {
                MessageBox.Show("Vui lòng nhập tên cần tìm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<TaiKhoanDinhDanh> ketQua = _cayDinhDanh.TimKiemTheoTen(ten);
            CapNhatDataGridView(ketQua);
        }

        private void btnTimKiemTheoNgay_Click(object sender, EventArgs e)
        {
            DateTime ngayBatDau = dtpNgayBatDau.Value;
            DateTime ngayKetThuc = dtpNgayKetThuc.Value;

            if (ngayBatDau > ngayKetThuc)
            {
                MessageBox.Show("Ngày bắt đầu phải trước hoặc bằng ngày kết thúc!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<TaiKhoanDinhDanh> ketQua = _cayDinhDanh.TimKiemTheoKhoangThoiGian(ngayBatDau, ngayKetThuc);
            CapNhatDataGridView(ketQua);
        }

        private void btnKhoaTaiKhoan_Click(object sender, EventArgs e)
        {
            if (gridDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần khóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tenDangNhap = gridDanhSach.SelectedRows[0].Cells[0].Value.ToString();
            var taiKhoan = _cayDinhDanh.TimKiem(tenDangNhap);
            if (taiKhoan != null)
            {
                taiKhoan.DangHoatDong = false;
                HienThiDanhSach();
                MessageBox.Show("Đã khóa tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnMoKhoaTaiKhoan_Click(object sender, EventArgs e)
        {
            if (gridDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần mở khóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tenDangNhap = gridDanhSach.SelectedRows[0].Cells[0].Value.ToString();
            var taiKhoan = _cayDinhDanh.TimKiem(tenDangNhap);
            if (taiKhoan != null)
            {
                taiKhoan.DangHoatDong = true;
                taiKhoan.SoLanDangNhapSai = 0;
                HienThiDanhSach();
                MessageBox.Show("Đã mở khóa tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnXemLichSu_Click(object sender, EventArgs e)
        {
            if (gridDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xem lịch sử!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tenDangNhap = gridDanhSach.SelectedRows[0].Cells[0].Value.ToString();
            var taiKhoan = _cayDinhDanh.TimKiem(tenDangNhap);
            if (taiKhoan != null)
            {
                string lichSu = "Lịch sử đăng nhập:\n\n";
                if (taiKhoan.LichSuDangNhap.Count == 0)
                {
                    lichSu += "Chưa có lịch sử đăng nhập.";
                }
                else
                {
                    foreach (var ngayDangNhap in taiKhoan.LichSuDangNhap)
                    {
                        lichSu += ngayDangNhap.ToString("dd/MM/yyyy HH:mm:ss") + "\n";
                    }
                }
                MessageBox.Show(lichSu, $"Lịch sử đăng nhập - {taiKhoan.HoTen}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            HienThiDanhSach();
            txtTimKiemCCCD.Clear();
            txtTimKiemTheoTen.Clear();
            dtpNgayBatDau.Value = DateTime.Now;
            dtpNgayKetThuc.Value = DateTime.Now;
        }

        private void CapNhatDataGridView(List<TaiKhoanDinhDanh> danhSachTaiKhoan)
        {
            gridDanhSach.Rows.Clear();
            foreach (var taiKhoan in danhSachTaiKhoan)
            {
                string trangThai = taiKhoan.DangHoatDong ?
                    (taiKhoan.BiKhoa() ? "Đã khóa" :
                    (taiKhoan.DaHetHan() ? "Hết hạn" : "Hoạt động"))
                    : "Vô hiệu";

                gridDanhSach.Rows.Add(
                    taiKhoan.TenDangNhap,
                    taiKhoan.SoCanCuoc,
                    taiKhoan.HoTen,
                    taiKhoan.NgaySinh.ToString("dd/MM/yyyy"),
                    taiKhoan.DiaChi,
                    taiKhoan.Email,
                    taiKhoan.NgayTao.ToString("dd/MM/yyyy"),
                    taiKhoan.NgayHetHan.ToString("dd/MM/yyyy"),
                    trangThai
                );
            }

            if (danhSachTaiKhoan.Count == 0)
            {
                MessageBox.Show("Không tìm thấy tài khoản nào!", "Kết quả tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void HienThiDanhSach()
        {
            List<TaiKhoanDinhDanh> danhSachTaiKhoan = _cayDinhDanh.DuyetTheoThuTu();
            CapNhatDataGridView(danhSachTaiKhoan);
        }
    }

    // Lớp Program
    static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var cayDinhDanh = new CayNhiPhanDinhDanh();
            KhoiTaoDuLieuMau(cayDinhDanh);

            System.Windows.Forms.Application.Run(new FormDangNhap(cayDinhDanh));
        }

        static void KhoiTaoDuLieuMau(CayNhiPhanDinhDanh cayDinhDanh)
        {
            // Tạo tài khoản admin
            cayDinhDanh.Them(new TaiKhoanDinhDanh
            {
                TenDangNhap = "admin",
                MatKhau = "admin123",
                SoCanCuoc = "001099000001",
                HoTen = "Quản Trị Viên",
                NgaySinh = new DateTime(1990, 1, 1),
                DiaChi = "Hà Nội",
                Email = "admin@example.com",
                NgayTao = DateTime.Now.AddDays(-30),
                NgayHetHan = DateTime.Now.AddYears(5),
                DangHoatDong = true,
                SoLanDangNhapSai = 0
            });

            // Tạo các tài khoản người dùng mẫu
            cayDinhDanh.Them(new TaiKhoanDinhDanh
            {
                TenDangNhap = "user1",
                MatKhau = "pass1",
                SoCanCuoc = "001099000002",
                HoTen = "Nguyễn Văn A",
                NgaySinh = new DateTime(1995, 5, 15),
                DiaChi = "TP. Hồ Chí Minh",
                Email = "user1@example.com",
                NgayTao = DateTime.Now.AddDays(-15),
                NgayHetHan = DateTime.Now.AddYears(2),
                DangHoatDong = true,
                SoLanDangNhapSai = 0
            });

            cayDinhDanh.Them(new TaiKhoanDinhDanh
            {
                TenDangNhap = "user2",
                MatKhau = "pass2",
                SoCanCuoc = "001099000003",
                HoTen = "Trần Thị B",
                NgaySinh = new DateTime(1992, 8, 20),
                DiaChi = "Đà Nẵng",
                Email = "user2@example.com",
                NgayTao = DateTime.Now.AddDays(-45),
                NgayHetHan = DateTime.Now.AddYears(1),
                DangHoatDong = true,
                SoLanDangNhapSai = 0
            });

            cayDinhDanh.Them(new TaiKhoanDinhDanh
            {
                TenDangNhap = "user3",
                MatKhau = "pass3",
                SoCanCuoc = "001099000004",
                HoTen = "Lê Văn C",
                NgaySinh = new DateTime(1988, 12, 10),
                DiaChi = "Hải Phòng",
                Email = "user3@example.com",
                NgayTao = DateTime.Now.AddDays(-60),
                NgayHetHan = DateTime.Now.AddDays(-1), // Tài khoản đã hết hạn
                DangHoatDong = true,
                SoLanDangNhapSai = 0
            });

            cayDinhDanh.Them(new TaiKhoanDinhDanh
            {
                TenDangNhap = "user4",
                MatKhau = "pass4",
                SoCanCuoc = "001099000005",
                HoTen = "Phạm Thị D",
                NgaySinh = new DateTime(1998, 3, 25),
                DiaChi = "Cần Thơ",
                Email = "user4@example.com",
                NgayTao = DateTime.Now.AddDays(-20),
                NgayHetHan = DateTime.Now.AddYears(3),
                DangHoatDong = false, // Tài khoản bị vô hiệu hóa
                SoLanDangNhapSai = 0
            });

            cayDinhDanh.Them(new TaiKhoanDinhDanh
            {
                TenDangNhap = "user5",
                MatKhau = "pass5",
                SoCanCuoc = "001099000006",
                HoTen = "Hoàng Văn E",
                NgaySinh = new DateTime(1993, 7, 5),
                DiaChi = "Huế",
                Email = "user5@example.com",
                NgayTao = DateTime.Now.AddDays(-10),
                NgayHetHan = DateTime.Now.AddYears(2),
                DangHoatDong = true,
                SoLanDangNhapSai = 5 // Tài khoản bị khóa do đăng nhập sai nhiều lần
            });
        }
    }
}