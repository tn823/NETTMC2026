# Hướng dẫn khắc phục lỗi ứng dụng bị treo (phải End Task để tiếp tục F5)

Trong quá trình phát triển và gỡ lỗi (debug) bằng Visual Studio, đôi khi ứng dụng (app) có thể bị treo (freeze/hang). Khi điều này xảy ra, bạn không thể dừng quá trình gỡ lỗi (Stop Debugging) hoặc chạy lại (F5) một cách bình thường do tiến trình (process) của ứng dụng vẫn còn đang ngầm chạy và bị kẹt.

## Nguyên nhân
- Thực thi vòng lặp vô hạn (Infinite loop) hoặc xử lý tác vụ nặng làm nghẽn luồng giao diện chính (UI Thread) gây ra deadlock.
- Ứng dụng thoát không đúng cách hoặc đóng form mà không dọn dẹp (dispose) các luồng liên quan.
- Các tiến trình (process / background thread) chạy dưới nền không được dọn dẹp khi ứng dụng chính bị tắt.

## Cách khắc phục

### 1. Xử lý tạm thời khi bị treo (Bước bạn đang gặp)
Khi gặp phải tình trạng ứng dụng bị treo và kết thúc phiên debug không dọn dẹp được tiến trình, bạn cần bắt buộc đóng rác tiến trình qua **Task Manager** để có thể nhấn lệnh F5 lại:
1. Nhấn tổ hợp phím **`Ctrl + Shift + Esc`** để mở **Task Manager**.
2. Tìm tiến trình của ứng dụng đang chạy (thường sẽ có tên giống tên file thực thi của dự án, ví dụ: `NETTMC.exe`).
3. Chuột phải vào tiến trình đó và chọn **End task** (hoặc chọn tiến trình và nhấn phím `Delete` trên bàn phím).
4. Quay lại Visual Studio, tiến trình đã kết thúc và bạn có thể tiếp tục nhấn `F5` hoặc `Ctrl + F5` để chạy lại ứng dụng.

### 2. Khắc phục triệt để trong code
Để ngăn chặn hoàn toàn việc xuất hiện những tiến trình chạy ngầm (zombie process) bị lỗi treo này:

- **Bảo đảm đóng hẳn ứng dụng khi tắt Form:**
  Khi đang tắt ứng dụng qua một Form chính, bạn có thể gọi thẳng lệnh sau trong Event `FormClosed` để ứng dụng kết thúc toàn bộ tất cả các luồng đang chạy:
  ```csharp
  Environment.Exit(0);
  ```

- **Set Thread.IsBackground = true:**
  Nếu dự án gọi các Thread hoặc Task chạy ngầm mới, hãy đảm bảo set biến thuộc tính `IsBackground = true`. Các background thread sẽ tự động kết thúc khi main process (ứng dụng) bị đóng lại.
  ```csharp
  Thread t = new Thread(DoWorkMethod);
  t.IsBackground = true; // Rất quan trọng để tránh treo app khi main thread tắt
  t.Start();
  ```

- **Dọn dẹp cẩn thận tài nguyên với `using` hay `Dispose()`:**
  Các đối tượng như kết nối cơ sở dữ liệu (`SqlConnection`, `OracleConnection`), thiết bị ngoại vi hoặc File kết nối cần được giải phóng ngay sau khi dùng xong để tránh leak (rò rỉ).
