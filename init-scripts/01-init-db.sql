-- Database initialization script for MyDoctorApp
-- This script runs when PostgreSQL container starts for the first time

-- Set up timezone
SET timezone = 'UTC';

-- Create extensions if needed
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Grant necessary permissions
GRANT ALL PRIVILEGES ON DATABASE "MyDoctorDb" TO postgres;

-- Optional: Create additional users or schemas here
-- CREATE USER mydoctorapp_user WITH PASSWORD 'app_password';
-- GRANT CONNECT ON DATABASE "MyDoctorDb" TO mydoctorapp_user;

-- Note: EF Core migrations will handle table creation
COMMENT ON DATABASE "MyDoctorDb" IS 'MyDoctorApp Database - Medical Practice Management System'; 