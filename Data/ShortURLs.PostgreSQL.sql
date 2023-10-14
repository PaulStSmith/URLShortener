/*
 * Paulo Santos
 * 12.Oct.2023
 *
 *  ___        _                ___  ___  _    
 * | _ \___ __| |_ __ _ _ _ ___/ __|/ _ \| |   
 * |  _/ _ (_-<  _/ _` | '_/ -_)__ \ (_) | |__ 
 * |_| \___/__/\__\__, |_| \___|___/\__\_\____|
 *                |___/                        
 *  ___ _            _   _   _ ___ _       
 * / __| |_  ___ _ _| |_| | | | _ \ |   ___
 * \__ \ ' \/ _ \ '_|  _| |_| |   / |__(_-<
 * |___/_||_\___/_|  \__|\___/|_|_\____/__/
 *                                         
 * Stores the short URLs and their expansion.
 */
CREATE TABLE "ShortUrls" (
  "id"              INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY NOT NULL,
  "hits"            int,
  "url"             varchar(255) NOT NULL,
  "shortUrl"        varchar(16) UNIQUE NOT NULL,
  "dateCreated"     timestamp DEFAULT (current_timestamp)
);