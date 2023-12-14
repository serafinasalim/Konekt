CREATE DATABASE  IF NOT EXISTS `konekt` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `konekt`;
-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: localhost    Database: konekt
-- ------------------------------------------------------
-- Server version	8.0.35

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20231109132319_InitialCreate','7.0.12'),('20231109164052_CobaDulu','7.0.12'),('20231109164455_PerbaikiNama','7.0.12'),('20231112074826_UpdateSQL','7.0.12'),('20231112143649_UpdateSQL2','7.0.12'),('20231116164541_UpdateTAcc','7.0.12'),('20231116165753_TestRole','7.0.12'),('20231119090343_Test','7.0.12'),('20231119090525_addcolumn','7.0.12'),('20231124140558_active','7.0.12'),('20231125142329_addcol','7.0.12'),('20231128153048_TambahTabel','7.0.12'),('20231128154023_drop','7.0.12'),('20231128154157_drop2','7.0.12'),('20231128154438_del','7.0.12'),('20231129163314_delcol','7.0.12'),('20231129172352_clan','7.0.12'),('20231129172627_droptabl','7.0.12'),('20231129172858_add','7.0.12'),('20231129173527_rollabck','7.0.12'),('20231129181102_testestes','7.0.12'),('20231129181307_hello','7.0.12'),('20231129181535_cape','7.0.12'),('20231203090153_addtable','7.0.12'),('20231203090310_attTable','7.0.12'),('20231205115305_addCOOLLL','7.0.12');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_account`
--

DROP TABLE IF EXISTS `t_account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `t_account` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `password` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `role` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `inactive` tinyint(1) NOT NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `IX_t_account_username` (`username`),
  CONSTRAINT `t_account_ibfk_1` FOREIGN KEY (`username`) REFERENCES `t_employee` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_account`
--

LOCK TABLES `t_account` WRITE;
/*!40000 ALTER TABLE `t_account` DISABLE KEYS */;
INSERT INTO `t_account` VALUES (1,'03082220004','serafina123','Human Resources',0),(2,'03082220013','vyo123','Employee',0),(3,'00000000999','testes','Human Resources',0),(4,'01234567890','123123','Employee',1),(5,'12345678901','12345','Human Resources',0);
/*!40000 ALTER TABLE `t_account` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_attendance`
--

DROP TABLE IF EXISTS `t_attendance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `t_attendance` (
  `attendance_id` int NOT NULL AUTO_INCREMENT,
  `employee_id` int NOT NULL,
  `clock_in` datetime(6) NOT NULL,
  `clock_out` datetime(6) DEFAULT NULL,
  `status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `tgl` date NOT NULL DEFAULT '0001-01-01',
  PRIMARY KEY (`attendance_id`),
  KEY `IX_t_attendance_employee_id` (`employee_id`),
  CONSTRAINT `FK_t_attendance_t_employee_employee_id` FOREIGN KEY (`employee_id`) REFERENCES `t_employee` (`employee_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=256 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_attendance`
--

LOCK TABLES `t_attendance` WRITE;
/*!40000 ALTER TABLE `t_attendance` DISABLE KEYS */;
INSERT INTO `t_attendance` VALUES (1,1,'2023-12-04 08:13:48.000000','2023-12-04 17:13:48.000000','H','2023-12-04'),(2,1,'2023-12-05 23:16:55.435996','0001-01-01 00:00:00.000000','H','2023-12-05'),(4,1,'2023-12-06 00:23:10.139206','2023-12-06 01:10:11.077743','C','2023-12-06'),(205,2,'2023-11-01 08:58:00.000000','2023-11-01 17:03:00.000000','H','2023-11-01'),(206,2,'2023-11-02 08:58:00.000000','2023-11-02 17:03:00.000000','H','2023-11-02'),(207,2,'2023-11-03 08:58:00.000000','2023-11-03 17:03:00.000000','H','2023-11-03'),(208,2,'2023-11-06 08:58:00.000000','2023-11-06 17:03:00.000000','H','2023-11-06'),(209,2,'2023-11-07 09:03:00.000000','2023-11-07 17:03:00.000000','T','2023-11-07'),(210,2,'2023-11-08 08:58:00.000000','2023-11-08 17:03:00.000000','H','2023-11-08'),(211,2,'2023-11-09 08:58:00.000000','2023-11-09 17:03:00.000000','H','2023-11-09'),(212,2,'2023-11-10 08:58:00.000000','2023-11-10 17:03:00.000000','H','2023-11-10'),(213,2,'2023-11-13 08:58:00.000000','2023-11-13 17:03:00.000000','H','2023-11-13'),(214,2,'2023-11-14 08:58:00.000000','2023-11-14 17:03:00.000000','H','2023-11-14'),(215,2,'2023-11-15 08:58:00.000000','2023-11-15 17:03:00.000000','H','2023-11-15'),(217,2,'2023-11-17 08:58:00.000000','2023-11-17 16:23:00.000000','C','2023-11-17'),(218,2,'2023-11-20 08:58:00.000000','2023-11-20 17:03:00.000000','H','2023-11-20'),(219,2,'2023-11-21 08:58:00.000000','2023-11-21 17:03:00.000000','H','2023-11-21'),(220,2,'2023-11-22 08:58:00.000000','2023-11-22 17:03:00.000000','H','2023-11-22'),(221,2,'2023-11-23 08:58:00.000000','2023-11-23 17:03:00.000000','H','2023-11-23'),(222,2,'2023-11-24 08:58:00.000000','2023-11-24 17:03:00.000000','H','2023-11-24'),(223,2,'2023-11-27 08:58:00.000000','2023-11-27 17:03:00.000000','H','2023-11-27'),(224,2,'2023-11-28 08:58:00.000000','2023-11-28 17:03:00.000000','H','2023-11-28'),(225,2,'2023-11-29 08:58:00.000000','2023-11-29 17:03:00.000000','H','2023-11-29'),(226,2,'2023-11-30 08:58:00.000000','2023-11-30 17:03:00.000000','H','2023-11-30'),(227,1,'2023-11-01 09:58:00.000000','2023-11-01 17:03:00.000000','T','2023-11-01'),(228,1,'2023-11-02 10:58:00.000000','2023-11-02 17:03:00.000000','T','2023-11-02'),(229,1,'2023-11-03 09:58:00.000000','2023-11-03 17:03:00.000000','T','2023-11-03'),(230,1,'2023-11-06 08:58:00.000000','2023-11-06 17:03:00.000000','H','2023-11-06'),(231,1,'2023-11-07 08:58:00.000000','2023-11-07 17:03:00.000000','H','2023-11-07'),(232,1,'2023-11-08 08:58:00.000000','2023-11-08 17:03:00.000000','H','2023-11-08'),(233,1,'2023-11-09 08:58:00.000000','2023-11-09 17:03:00.000000','H','2023-11-09'),(235,1,'2023-11-13 08:58:00.000000','2023-11-13 17:03:00.000000','H','2023-11-13'),(236,1,'2023-11-14 08:58:00.000000','2023-11-14 17:03:00.000000','H','2023-11-14'),(237,1,'2023-11-15 08:58:00.000000','2023-11-15 17:03:00.000000','H','2023-11-15'),(238,1,'2023-11-16 08:58:00.000000','2023-11-16 17:03:00.000000','H','2023-11-16'),(240,1,'2023-11-20 08:58:00.000000','2023-11-20 17:03:00.000000','H','2023-11-20'),(241,1,'2023-11-21 08:58:00.000000','2023-11-21 17:03:00.000000','H','2023-11-21'),(242,1,'2023-11-22 08:58:00.000000','2023-11-22 17:03:00.000000','H','2023-11-22'),(243,1,'2023-11-23 08:58:00.000000','2023-11-23 17:03:00.000000','H','2023-11-23'),(244,1,'2023-11-24 08:58:00.000000','2023-11-24 17:03:00.000000','H','2023-11-24'),(245,1,'2023-11-27 08:58:00.000000','2023-11-27 17:03:00.000000','H','2023-11-27'),(246,1,'2023-11-28 08:58:00.000000','2023-11-28 17:03:00.000000','H','2023-11-28'),(247,1,'2023-11-29 08:58:00.000000','2023-11-29 17:03:00.000000','H','2023-11-29'),(248,1,'2023-11-30 08:58:00.000000','2023-11-30 17:03:00.000000','H','2023-11-30'),(254,2,'2023-11-16 08:58:00.000000','2023-11-16 17:03:00.000000','H','2023-11-16');
/*!40000 ALTER TABLE `t_attendance` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_employee`
--

DROP TABLE IF EXISTS `t_employee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `t_employee` (
  `employee_id` int NOT NULL AUTO_INCREMENT,
  `employee_name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `base_salary` decimal(10,0) NOT NULL DEFAULT '0',
  `deleted` tinyint(1) NOT NULL DEFAULT '0',
  `username` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `position_id` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`employee_id`),
  UNIQUE KEY `AK_t_employee_username` (`username`),
  KEY `IX_t_employee_position_id` (`position_id`),
  CONSTRAINT `t_employee_ibfk_1` FOREIGN KEY (`position_id`) REFERENCES `t_position` (`position_id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_employee`
--

LOCK TABLES `t_employee` WRITE;
/*!40000 ALTER TABLE `t_employee` DISABLE KEYS */;
INSERT INTO `t_employee` VALUES (1,'Serafina Salim',100000000,0,'03082220004',1),(2,'Vyorennity Joeanca',30000000,0,'03082220013',2),(6,'testtest',123123,1,'03081220000',3),(10,'Sera Test',6786786,1,'00000000999',2),(11,'test',1000000,1,'09090909090',3),(12,'vyovyo',123123,1,'01234567890',3),(13,'Anson',20000000,0,'03081220001',2),(15,'Benz',50000000,0,'12345678901',2);
/*!40000 ALTER TABLE `t_employee` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_position`
--

DROP TABLE IF EXISTS `t_position`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `t_position` (
  `position_id` int NOT NULL AUTO_INCREMENT,
  `position_name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `deleted` tinyint(1) NOT NULL DEFAULT '0',
  `salary_max` decimal(10,0) NOT NULL DEFAULT '0',
  `salary_min` decimal(10,0) NOT NULL DEFAULT '0',
  PRIMARY KEY (`position_id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_position`
--

LOCK TABLES `t_position` WRITE;
/*!40000 ALTER TABLE `t_position` DISABLE KEYS */;
INSERT INTO `t_position` VALUES (1,'Executive',0,500000000,60000000),(2,'Manager',0,59999999,10000000),(3,'Staff',0,9999999,3500000),(24,'ABCDEF',1,222222,111111);
/*!40000 ALTER TABLE `t_position` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-12-14 19:55:29
