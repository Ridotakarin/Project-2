// Part 4: Định mệnh của Eloria - Trận chiến cuối

=== start ===
"Umboros đang chuẩn bị phá vỡ phong ấn Cội Linh."

+ [Chuẩn bị đối mặt] -> prepare_fight
+ [Tập hợp dân làng] -> rally_villagers

=== rally_villagers ===
Alex: "Mọi người, ta phải đứng cùng nhau để bảo vệ ngôi làng!"
+ [Mọi người đồng ý hỗ trợ] -> prepare_fight
+ [Mọi người còn nghi ngờ] -> doubt_villagers

=== doubt_villagers ===
Một số người còn nghi ngờ nhưng vẫn quyết định cố gắng.
+ [Khích lệ họ] -> prepare_fight

=== prepare_fight ===
Alex chuẩn bị mọi vũ khí, phép thuật và sức mạnh tình bạn để chiến đấu.
+ [Bắt đầu trận chiến] -> battle

=== battle ===
(Đoạn hội thoại trận chiến có thể được phát triển riêng hoặc gọi vào hệ thống combat.)
+ [Chiến thắng] -> victory
+ [Thất bại] -> defeat

=== victory ===
"Umboros bị phong ấn lại, Eloria hồi sinh rực rỡ."
-> DONE

=== defeat ===
"Alex thất bại, bóng tối lan rộng..."
-> DONE
