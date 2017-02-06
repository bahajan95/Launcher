/*
Navicat MySQL Data Transfer

Source Server         : WoW
Source Server Version : 50525
Source Host           : localhost:3306
Source Database       : news

Target Server Type    : MYSQL
Target Server Version : 50525
File Encoding         : 65001

Date: 2015-10-02 00:48:37
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `news_launcher`
-- ----------------------------
DROP TABLE IF EXISTS `news_launcher`;
CREATE TABLE `news_launcher` (
  `id` int(11) unsigned NOT NULL,
  `news_head` varchar(64) NOT NULL DEFAULT '',
  `news_body` text NOT NULL,
  `news_link` varchar(64) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='News System';

-- ----------------------------
-- Records of news_launcher
-- ----------------------------
INSERT INTO `news_launcher` VALUES ('1', 'HEAD NEWS', 'BODY NEWS', 'http://127.0.0.1/');
INSERT INTO `news_launcher` VALUES ('2', 'HEAD NEWS', 'BODY NEWS', 'http://127.0.0.1/');
INSERT INTO `news_launcher` VALUES ('3', 'HEAD NEWS', 'BODY NEWS', 'http://127.0.0.1/');
INSERT INTO `news_launcher` VALUES ('4', 'HEAD NEWS', 'BODY NEWS', 'http://127.0.0.1/');
