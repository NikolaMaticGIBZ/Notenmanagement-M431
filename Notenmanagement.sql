CREATE DATABASE NotenManagement;
USE NotenManagement;
 
-- Table for users
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
 
    twofactor_code VARCHAR(6) NULL,
    twofactor_expires DATETIME NULL,
    twofactor_verified BIT NOT NULL DEFAULT 0,
 
    role VARCHAR(50) NOT NULL DEFAULT 'teacher'
);
 
-- Table for rektors
CREATE TABLE rektor (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL
);
 
-- Table for grades
CREATE TABLE grades (
    id INT AUTO_INCREMENT PRIMARY KEY,
 
    course_name VARCHAR(255) NOT NULL,
    module_name VARCHAR(255) NOT NULL,
    student_name VARCHAR(255) NOT NULL,
    grade_value DECIMAL(5,2) NOT NULL,
 
    status VARCHAR(20) NOT NULL DEFAULT 'pending',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    decision_note VARCHAR(1000) NULL,
    comment VARCHAR(1000) NULL,
 
    teacher_id INT NOT NULL,
    rektor_id INT NOT NULL,
    prorektor_id INT NULL,
 
    CONSTRAINT fk_grades_teacher
        FOREIGN KEY (teacher_id) REFERENCES users(id) ON DELETE CASCADE,
 
    CONSTRAINT fk_grades_rektor
        FOREIGN KEY (rektor_id) REFERENCES rektor(id) ON DELETE RESTRICT,
 
    CONSTRAINT fk_grades_prorektor
        FOREIGN KEY (prorektor_id) REFERENCES users(id) ON DELETE SET NULL
);

-- Blockchain-like ledger for grade changes
CREATE TABLE grade_ledger (
    id INT AUTO_INCREMENT PRIMARY KEY,
    grade_id INT NOT NULL,
    action VARCHAR(50) NOT NULL,
    snapshot_json VARCHAR(4000) NOT NULL,
    payload_hash VARCHAR(128) NOT NULL,
    previous_hash VARCHAR(128) NOT NULL,
    block_hash VARCHAR(128) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    actor_user_id INT NOT NULL,
    actor_role VARCHAR(50) NOT NULL
);
 
-- Initial rektor data
INSERT INTO rektor (id, name) VALUES
(1, 'Regula Tobler'),
(2, 'Werner Odermatt'),
(3, 'Patrick Zeiger'),
(4, 'Alex Kobel');
