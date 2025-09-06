// Part 3: Hành trình khám phá, side quests NPC

=== start ===
"Bạn có thể gặp gỡ từng người dân và khám phá câu chuyện riêng của họ."

+ [Gặp Josh – săn bắn] -> meet_josh
+ [Gặp Lyria – thảo dược] -> meet_lyria
+ [Gặp Manu – quản lý gia súc] -> meet_manu
+ [Gặp Tori – chế tạo máy móc] -> meet_tori
+ [Gặp Bill – thợ rèn, chiến binh] -> meet_bill

=== meet_josh ===
Josh: "Có việc săn bắt cần giúp đỡ không?"
+ [Giúp Josh săn bắn] -> side_josh
+ [Hẹn lần khác] -> start

=== side_josh ===
Josh dẫn bạn vào rừng và dạy cách săn thú.
+ [Cảm ơn và học hỏi] -> start

=== meet_lyria ===
Lyria: "Tôi đang nghiên cứu loại thảo dược mới, bạn có muốn giúp?"
+ [Giúp Lyria thu thập thảo dược] -> side_lyria
+ [Từ chối nhẹ nhàng] -> start

=== side_lyria ===
Lyria dẫn bạn vào khu rừng ma thuật.
+ [Thu thập thảo dược thành công] -> start

=== meet_manu ===
Manu: "Cần giúp quản lý gia súc không?"
+ [Giúp Manu chăm sóc động vật] -> side_manu
+ [Từ chối] -> start

=== side_manu ===
Bạn giúp Manu cho gia súc ăn và kiểm tra sức khỏe.
+ [Hoàn thành công việc] -> start

=== meet_tori ===
Tori: "Bạn có muốn xem dự án máy móc mới không?"
+ [Xem và học hỏi] -> side_tori
+ [Không quan tâm] -> start

=== side_tori ===
Tori cho bạn xem cỗ máy và giải thích cách hoạt động.
+ [Rất thú vị] -> start

=== meet_bill ===
Bill: "Tôi cần luyện thêm vũ khí. Bạn có muốn giúp?"
+ [Giúp luyện vũ khí] -> side_bill
+ [Không tiện] -> start

=== side_bill ===
Bạn cùng Bill luyện tập và rèn vũ khí mới.
+ [Hoàn thành] -> start

