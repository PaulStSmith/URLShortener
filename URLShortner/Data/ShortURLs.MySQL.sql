/*
 * Paulo Santos
 * 12.Oct.2023
 *
 *  __  __      ___  ___  _    
 * |  \/  |_  _/ __|/ _ \| |   
 * | |\/| | || \__ \ (_) | |__ 
 * |_|  |_|\_, |___/\__\_\____|
 *         |__/                
 *  ___ _            _   _   _ ___ _       
 * / __| |_  ___ _ _| |_| | | | _ \ |   ___
 * \__ \ ' \/ _ \ '_|  _| |_| |   / |__(_-<
 * |___/_||_\___/_|  \__|\___/|_|_\____/__/
 *                                         
 * Stores the short URLs and their expansion.
 */
CREATE TABLE `ShortUrls` (
  `id`          int PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `hits`        int,
  `url`         varchar(255) NOT NULL,
  `shortUrl`    varchar(16) UNIQUE NOT NULL,
  `dateCreated` datetime DEFAULT (current_timestamp)
);

