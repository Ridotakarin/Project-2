=== part2_start ===

"Tôi bắt đầu cảm nhận được sự khác biệt kỳ lạ trong làng Eloria."

+ [Hỏi Lyria về những loại cây trồng phát sáng] 
    -> ask_lyria_about_plants

+ [Đi thăm các cánh đồng để tự kiểm chứng] 
    -> explore_fields

+ [Tâm sự với Josh về cảm xúc của mọi người trong làng]
    -> talk_to_josh

=== ask_lyria_about_plants ===
Lyria: "Đúng vậy, những cây trồng ở đây có khả năng phản ứng với cảm xúc tích cực của người dân. Nó làm chúng phát sáng và có năng lực đặc biệt."

+ [Hỏi tiếp về nguồn gốc năng lượng này] 
    -> ask_spirit_root

+ [Hỏi về bóng tối đang rình rập]
    -> ask_about_darkness

+ [Cảm ơn Lyria và hứa sẽ chú ý hơn]
    -> part2_continue

=== ask_spirit_root ===
Lyria: "Làng Eloria nằm trên một Tâm Linh Địa – một trung tâm năng lượng thiên nhiên từ rất lâu rồi. Nhưng năng lượng đó cũng thu hút những thế lực bóng tối."

+ [Hỏi về các sinh vật bóng tối] 
    -> ask_about_darkness

+ [Nói rằng sẽ cẩn thận hơn]
    -> part2_continue

=== ask_about_darkness ===
Lyria: "Vùng Đất Bóng Tối, nơi bị lãng quên, giờ đang thức tỉnh. Những sinh vật đó xuất hiện vào ban đêm và hút năng lượng từ nông sản."

+ [Hỏi cách bảo vệ làng] 
    -> ask_how_to_protect

+ [Thở dài lo lắng và quyết tâm hơn]
    -> part2_continue

=== ask_how_to_protect ===
Lyria: "Chúng ta cần củng cố phép thuật bảo vệ, đặc biệt là vào ban đêm. Bạn cũng có thể giúp tôi thu thập thảo dược để tạo bùa hộ mệnh."

+ [Đồng ý giúp Lyria]
    // help_lyria = 1
    "Tôi sẽ giúp cô ấy bằng mọi cách."
    -> part2_continue

+ [Ngần ngại nhưng sẽ suy nghĩ thêm]
    // help_lyria = 0
    "Tôi cần thời gian để suy nghĩ kỹ hơn."
    -> part2_continue

=== explore_fields ===
"Tôi ra đồng để xem xét các cây trồng đang phát sáng và cảm nhận sự kỳ diệu."

+ [Thu thập mẫu cây để hỏi Lyria] 
    -> ask_lyria_about_plants

+ [Đi thăm các hộ gia đình trong làng]
    -> visit_villagers

+ [Quay lại trò chuyện với Josh]
    -> talk_to_josh

=== talk_to_josh ===
Josh: "Mày cảm nhận thấy gì trong làng không? Tao thấy bầu không khí có chút khác lạ."

+ [Chia sẻ sự lo lắng về bóng tối]
    -> share_concern_with_josh

+ [Nói về năng lượng tích cực của làng]
    -> share_hope_with_josh

+ [Chỉ nói chuyện xã giao]
    -> part2_continue

=== share_concern_with_josh ===
Josh: "Ừ, tao cũng nghe về những chuyện quái dị ban đêm. Cần phải cảnh giác."

+ [Đề nghị cùng nhau tuần tra ban đêm]
    // night_patrol = 1
    "Tôi muốn cùng Josh bảo vệ làng vào ban đêm."
    -> part2_continue

+ [Chỉ theo dõi và chuẩn bị]
    // night_patrol = 0
    "Tôi sẽ tự mình quan sát tình hình."
    -> part2_continue

=== share_hope_with_josh ===
Josh: "Đúng vậy, sức mạnh của làng là sự gắn kết của mọi người. Cùng cố gắng nhé!"

-> part2_continue

=== visit_villagers ===
"Tôi đi thăm các cư dân khác trong làng để hiểu hơn về cuộc sống và phép màu nơi đây."

+ [Gặp Manu – người chăm sóc gia súc]
    -> meet_manu

+ [Gặp Tori – cô gái đam mê máy móc]
    -> meet_tori

+ [Gặp Bill – thợ rèn làng]
    -> meet_bill

+ [Quay về làng]
    -> part2_continue

=== meet_manu ===
Manu: "Alex, tôi nghe bạn đang dần hiểu ra phép màu nơi đây. Nếu cần giúp đỡ với gia súc, cứ nói tôi."

+ [Cảm ơn Manu và hứa sẽ hỏi khi cần]
    -> visit_villagers

+ [Hỏi Manu về những điều kỳ lạ ở làng]
    -> manu_talk_mystery

=== manu_talk_mystery ===
Manu: "Có những đêm gia súc hoảng loạn, như cảm nhận được điều gì đó không bình thường."

+ [Hỏi thêm về bóng tối]
    -> ask_about_darkness

+ [Cảm ơn Manu và rời đi]
    -> visit_villagers

=== meet_tori ===
Tori: "Alex! Tôi đang nghiên cứu máy móc để giúp trang trại hoạt động hiệu quả hơn."

+ [Hỏi Tori về máy móc và phép màu]
    -> tori_talk_magic

+ [Chào Tori và tiếp tục đi]
    -> visit_villagers

=== tori_talk_magic ===
Tori: "Tôi không tin vào phép màu nhiều, nhưng có thứ gì đó ở đây khiến tôi phải suy nghĩ lại."

-> visit_villagers

=== meet_bill ===
Bill: "Tôi từng là chiến binh bảo vệ vùng đất này. Bóng tối không chỉ là chuyện hoang đường."

+ [Hỏi Bill về cách chiến đấu chống bóng tối]
    -> bill_talk_combat

+ [Cảm ơn Bill và tiếp tục]
    -> visit_villagers

=== bill_talk_combat ===
Bill: "Tôi sẽ giúp bạn rèn vũ khí và dạy cách chiến đấu nếu bạn muốn."

+ [Đồng ý học chiến đấu]
    // learn_combat = 1
    "Tôi sẵn sàng học để bảo vệ làng."
    -> DONE

+ [Chưa muốn học ngay]
    // learn_combat = 0
    "Tôi cần thời gian suy nghĩ."

-> visit_villagers

=== part2_continue ===
"Tôi tiếp tục cuộc sống ở Eloria, chuẩn bị cho những thử thách phía trước."

-> DONE
