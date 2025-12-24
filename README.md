# Tetris Game - Trò chơi xếp hình Tetris

## Giới thiệu

Đây là phiên bản game Tetris cổ điển được phát triển bằng WPF và .NET 8, có tích hợp hệ thống lưu điểm cao trên cloud database. Game hỗ trợ đăng ký tài khoản, lưu lịch sử chơi và bảng xếp hạng.

## Tính năng chính

- Gameplay Tetris cổ điển với đầy đủ các khối hình
- Hệ thống tài khoản người chơi (đăng ký, đăng nhập)
- Lưu điểm cao, cấp độ cao nhất và thời gian chơi tốt nhất
- Lịch sử chơi chi tiết
- Âm thanh và hiệu ứng
- Hệ thống tạm dừng
- Chế độ giữ khối (Hold)
- Hiển thị khối tiếp theo
- Cloud database - không cần cài đặt MySQL local

## Yêu cầu hệ thống

### Để chạy từ source code:
- .NET SDK 8.0 trở lên
- Windows 10/11 (64-bit)
- Kết nối internet (để kết nối cloud database)

### Để chạy file .exe đã build:
- Windows 10/11 (64-bit)
- Kết nối internet (để lưu điểm)
- KHÔNG cần cài .NET SDK
- KHÔNG cần cài MySQL

## Cài đặt và chạy

### Cách 1: Chạy từ source code

```powershell
# Clone hoặc download project
cd Tetris_Game

# Build project
dotnet build

# Chạy game
dotnet run --project Tetris_Game.csproj
```

### Cách 2: Chạy file .exe đã build

```powershell
# Build file .exe
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish

# Chạy file .exe
cd publish
.\Tetris_Game.exe
```

### Cách 3: Sử dụng file zip đã đóng gói

1. Giải nén file `Tetris_Game_Cloud.zip`
2. Chạy file `Tetris_Game.exe`
3. Chơi game

## Hướng dẫn chơi

### Điều khiển cơ bản:

- **Mũi tên trái/phải**: Di chuyển khối sang trái/phải
- **Mũi tên xuống**: Rơi nhanh (soft drop)
- **Mũi tên lên**: Xoay khối theo chiều kim đồng hồ
- **Phím Z**: Xoay khối ngược chiều kim đồng hồ
- **Phím C**: Giữ khối hiện tại (Hold)
- **Phím Space**: Thả khối xuống ngay lập tức (hard drop)
- **Phím P**: Tạm dừng game
- **Phím M**: Tắt/Bật âm thanh

### Cách chơi:

1. Khởi động game và đăng ký/đăng nhập tài khoản
2. Các khối Tetris sẽ rơi từ trên xuống
3. Sử dụng phím điều khiển để di chuyển và xoay khối
4. Xếp các khối để tạo thành hàng ngang hoàn chỉnh
5. Hàng hoàn chỉnh sẽ biến mất và bạn được cộng điểm
6. Game kết thúc khi các khối chạm đến đỉnh màn hình
7. Điểm cao nhất của bạn sẽ được lưu tự động

### Hệ thống điểm:

- Xóa 1 hàng: 100 điểm
- Xóa 2 hàng cùng lúc: 300 điểm
- Xóa 3 hàng cùng lúc: 500 điểm
- Xóa 4 hàng cùng lúc (Tetris): 800 điểm
- Tốc độ rơi tăng theo cấp độ

## Cấu trúc project

```
Tetris_Game/
├── models/              # Các model logic game
│   ├── Block.cs        # Class cơ sở cho các khối
│   ├── GameGrid.cs     # Lưới game
│   ├── GameState.cs    # Trạng thái game
│   ├── BlockQueue.cs   # Hàng đợi khối
│   ├── Position.cs     # Vị trí
│   └── [các khối cụ thể: IBlock, JBlock, LBlock, OBlock, SBlock, TBlock, ZBlock]
│
├── views/              # Các view WPF
│   ├── MainWindow.xaml     # Màn hình chơi game chính
│   ├── Login.xaml          # Màn hình đăng nhập
│   ├── Register.xaml       # Màn hình đăng ký
│   ├── Player.xaml         # Màn hình chọn người chơi
│   ├── Pause.xaml          # Màn hình tạm dừng
│   └── [các file .xaml.cs tương ứng]
│
├── resources/          # Tài nguyên
│   ├── assets/        # Hình ảnh (khối, background, v.v.)
│   ├── audio/         # File âm thanh
│   └── button/        # Hình ảnh nút bấm
│
├── DatabaseManager.cs  # Quản lý kết nối database
├── AudioManager.cs     # Quản lý âm thanh
├── App.config         # Cấu hình database
├── DBTetris.sql       # Schema database
└── README.md          # File này
```

## Database

### Cấu hình

Game sử dụng **Railway Cloud Database** (MySQL) để lưu trữ dữ liệu người chơi. Thông tin kết nối được cấu hình trong file `App.config`.

### Schema database

Database gồm 3 bảng chính:

**1. InfoPlayer** - Thông tin người chơi
- UserName (VARCHAR 50) - Tên đăng nhập (Primary Key)
- PassWord (VARCHAR 50) - Mật khẩu
- Gender (VARCHAR 10) - Giới tính

**2. Goal** - Điểm cao nhất
- UserName (VARCHAR 50) - Tên người chơi (Primary Key, Foreign Key)
- BestScore (INT) - Điểm cao nhất
- BestLevel (INT) - Cấp độ cao nhất
- BestTime (VARCHAR 5) - Thời gian chơi lâu nhất

**3. History** - Lịch sử chơi
- HistoryID (INT) - ID lịch sử (Primary Key, Auto Increment)
- UserName (VARCHAR 50) - Tên người chơi (Foreign Key)
- Date (DATE) - Ngày chơi
- LoginTime (VARCHAR 8) - Thời gian đăng nhập
- TimesPlayed (INT) - Số lần chơi
- Score (INT) - Điểm số
- Level (INT) - Cấp độ đạt được
- PlayTime (VARCHAR 5) - Thời gian chơi

### Chuyển đổi giữa Cloud và Local Database

**Sử dụng Cloud Database (Mặc định):**
File `App.config` đã được cấu hình sẵn để kết nối Railway cloud database.

**Chuyển về Local Database:**
1. Mở file `App.config`
2. Comment (chú thích) phần Railway connection
3. Uncomment (bỏ chú thích) phần localhost connection
4. Cài đặt MySQL local và import file `DBTetris.sql`
5. Build lại project

## Quản lý Cloud Database

### Truy cập Railway Dashboard:
- URL: https://railway.app
- Project: Tetris_Game_DB
- Service: MySQL

### Xem dữ liệu:
1. Đăng nhập Railway
2. Mở project Tetris_Game_DB
3. Click vào MySQL service
4. Chọn tab "Data" để xem các bảng

### Backup dữ liệu:
Sử dụng MySQL Workbench hoặc Railway UI để export dữ liệu.

Chi tiết xem file `CLOUD_DATABASE_SETUP.md`.

## Build và Publish

### Build bản Debug:
```powershell
dotnet build
```

### Build bản Release:
```powershell
dotnet build -c Release
```

### Publish file .exe standalone:
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

Tham số:
- `-c Release`: Build bản release
- `-r win-x64`: Target Windows 64-bit
- `--self-contained true`: Bao gồm .NET runtime (không cần cài .NET)
- `-p:PublishSingleFile=true`: Tạo file .exe đơn
- `-o publish`: Output vào folder publish

### Tạo file zip để phân phối:
```powershell
Compress-Archive -Path publish\* -DestinationPath Tetris_Game_Cloud.zip -Force
```

## Phân phối cho người dùng

### File cần thiết:
- `Tetris_Game.exe` - File thực thi chính
- `App.config` hoặc `Tetris_Game.dll.config` - File cấu hình
- Folder `resources/` - Tài nguyên game (hình ảnh, âm thanh)

### Hướng dẫn cho người dùng cuối:
1. Giải nén file zip
2. Chạy file `Tetris_Game.exe`
3. Không cần cài đặt gì thêm

## Xử lý sự cố

### Game không khởi động được:
- Kiểm tra Windows Defender hoặc Antivirus có chặn file .exe không
- Click chuột phải vào file .exe, chọn Properties, tick "Unblock" nếu có

### Không kết nối được database:
- Kiểm tra kết nối internet
- Kiểm tra firewall có chặn kết nối không
- Xem log lỗi trong game

### Lỗi âm thanh không phát:
- Kiểm tra file audio trong folder `resources/audio/`
- Kiểm tra volume hệ thống
- Thử nhấn phím M để bật/tắt âm thanh

### Build bị lỗi:
- Đảm bảo đã cài .NET SDK 8.0 trở lên
- Chạy `dotnet clean` rồi `dotnet build` lại
- Kiểm tra các package NuGet đã được restore chưa

## Công nghệ sử dụng

- **Framework**: .NET 8.0
- **UI**: WPF (Windows Presentation Foundation)
- **Database**: MySQL (Railway Cloud)
- **Audio**: NAudio library
- **Database Client**: MySql.Data

## Tác giả và License

Project này được phát triển cho mục đích học tập và giải trí.

Assets (hình ảnh, âm thanh) được sử dụng cho mục đích demo.

## Liên hệ và Đóng góp

Nếu bạn muốn đóng góp hoặc báo lỗi, vui lòng tạo issue hoặc pull request trên repository.

## Changelog

### Version 1.0 (2025-12-20)
- Phát hành phiên bản đầu tiên
- Chuyển từ local database sang Railway cloud database
- Hỗ trợ build standalone .exe
- Thêm hệ thống tài khoản và lưu điểm
- Thêm âm thanh và hiệu ứng

## Tài liệu tham khảo

- `CLOUD_DATABASE_SETUP.md` - Hướng dẫn quản lý cloud database
- `DONE.md` - Tóm tắt quá trình setup
- `publish/README.txt` - Hướng dẫn cho người dùng cuối

## Lưu ý quan trọng

- Game cần kết nối internet để lưu điểm và đăng nhập
- Thông tin database trong `App.config` là public, nên cân nhắc bảo mật nếu deploy production
- Railway free tier có giới hạn $5 credit/tháng
- Backup dữ liệu định kỳ để tránh mất mát

Chúc bạn chơi game vui vẻ!
