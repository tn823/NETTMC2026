# Danh sách Test Voice Whisper (Sát thực tế A14)

Dưới đây là 20 câu lệnh test sát với thực tế sản xuất nhất (công nhân thường đọc liền một hơi: **Bộ phận + Mã lỗi/Tên lỗi + Hành động**).
Hãy chạy `VoiceTestForm`, đọc các câu lệnh dưới đây và paste log vào cột "Log Kết Quả Thực Tế" để tôi phân tích độ chính xác, tốc độ nhận diện và bổ sung thêm các cách đọc số (như "một một" thay vì "mười một").

| STT | Câu nói (Voice Input) | Kỳ vọng (Expected Output) | Log Kết Quả Thực Tế | Ghi chú |
|---|---|---|---|---|
| 1 | A một một không đạt | Part: A, Err: 11, Act: fail | | Đọc tách số "một một" (11) |
| 2 | B mười tám không đạt | Part: B, Err: 18, Act: fail | | Đọc gộp số "mười tám" |
| 3 | A một tám hở keo không đạt | Part: A, Err: 18, Act: fail | | Đọc tách số "một tám" + tên lỗi |
| 4 | C mười bảy lem keo không đạt | Part: C, Err: 17, Act: fail | | Đọc số + tên lỗi |
| 5 | A năm khác màu không đạt | Part: A, Err: 5, Act: fail | | Lỗi 1 chữ số |
| 6 | đạt | Act: pass | | Lệnh Pass tiếng Việt |
| 7 | pass | Act: pass | | Lệnh Pass tiếng Anh |
| 8 | B hai lăm kim loại không đạt | Part: B, Err: 25, Act: fail | | Nuốt chữ "hai mươi lăm" -> "hai lăm" |
| 9 | A mười hai nhăn không đạt | Part: A, Err: 12, Act: fail | | |
| 10 | B mười đứt chỉ không đạt | Part: B, Err: 10, Act: fail | | Chữ "mười" (Whisper hay nhầm thành "mùi") |
| 11 | C bốn mươi chỉ thừa không đạt | Part: C, Err: 40, Act: fail | | Số tròn chục |
| 12 | A ba tư hở keo nhỏ không đạt | Part: A, Err: 34, Act: fail | | Nuốt chữ "ba tư" (34) |
| 13 | B ba lăm hở keo lớn không đạt | Part: B, Err: 35, Act: fail | | Nuốt chữ "ba lăm" (35) |
| 14 | C hai mốt vệ sinh dơ không đạt | Part: C, Err: 21, Act: fail | | Nuốt chữ "hai mốt" (21) |
| 15 | A bốn mốt đứt chỉ không đạt | Part: A, Err: 41, Act: fail | | Nuốt chữ "bốn mốt" (41) |
| 16 | B bốn hai nhăn lót không đạt | Part: B, Err: 42, Act: fail | | Nuốt chữ "bốn hai" (42) |
| 17 | đạt lại | Act: re-pass | | Lệnh Repass |
| 18 | lỗi lại | Act: re-fail | | Lệnh Refail |
| 19 | xóa | Act: clear | | Lệnh Clear / Hủy thao tác |
| 20 | C tám hai lỗi khác không đạt | Part: C, Err: 82, Act: fail | | Test số lớn nhất (82) |
