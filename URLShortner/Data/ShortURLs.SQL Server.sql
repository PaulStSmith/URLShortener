/*
 * Paulo Santos
 * 12.Oct.2023
 *
 *  ___  ___  _      ___                      
 * / __|/ _ \| |    / __| ___ _ ___ _____ _ _ 
 * \__ \ (_) | |__  \__ \/ -_) '_\ V / -_) '_|
 * |___/\__\_\____| |___/\___|_|  \_/\___|_|  
 *                                            
 *  ___ _            _   _   _ ___ _       
 * / __| |_  ___ _ _| |_| | | | _ \ |   ___
 * \__ \ ' \/ _ \ '_|  _| |_| |   / |__(_-<
 * |___/_||_\___/_|  \__|\___/|_|_\____/__/
 *                                         
 * Stores the short URLs and their expansion.
 */
CREATE TABLE ShortURLs (
    id          INT IDENTITY(1,1) 
                PRIMARY KEY,            -- Primary key
    hits        INT,                    -- Number of hits
    [url]       VARCHAR(255),           -- The long URL
    shortUrl    VARCHAR(16) UNIQUE,     -- Alternate key
    dateCreated DATETIME 
                DEFAULT 
                CURRENT_TIMESTAMP,      -- Date when the record was created with default value
);

