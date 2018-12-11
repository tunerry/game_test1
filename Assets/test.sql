SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for ranklist
-- ----------------------------
DROP TABLE IF EXISTS `ranklist`;
CREATE TABLE `ranklist`  (
  `rank` int(10) NOT NULL,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `point` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`rank`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Records of ranklist
-- ----------------------------
INSERT INTO `ranklist` VALUES (1, 'qwe1', '109');
INSERT INTO `ranklist` VALUES (2, 'qwe1', '108');
INSERT INTO `ranklist` VALUES (3, 'asd', '106');
INSERT INTO `ranklist` VALUES (4, 'user01', '100');
INSERT INTO `ranklist` VALUES (5, 'user02', '90');
INSERT INTO `ranklist` VALUES (6, 'user03', '80');
INSERT INTO `ranklist` VALUES (7, 'user04', '70');
INSERT INTO `ranklist` VALUES (8, 'user01', '60');
INSERT INTO `ranklist` VALUES (9, 'user03', '50');
INSERT INTO `ranklist` VALUES (10, 'user04', '40');

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `passwd` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Records of user
-- ----------------------------
INSERT INTO `user` VALUES (1, 'qwe1', 'qwe11');
INSERT INTO `user` VALUES (2, 'asd', '1111');

SET FOREIGN_KEY_CHECKS = 1;
