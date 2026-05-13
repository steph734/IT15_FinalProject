-- Generate password hashes for predefined accounts
-- These hashes are generated using SHA256 (matching the UserService.HashPassword method)

USE db49649;
GO

-- Password hashes (SHA256):
-- broker@123456 = "lM9v8K3qN7xR2wT5yU0pA6bC4dE8fG1hJ3kL5mN7oP9qR2sT4uV6wX8yZ0aB2cD4eF6gH8iJ0kL2mN4oP6qR8sT0uV2wX4yZ6aB8cD0eF2gH4iJ6kL8mN0oP2qR4sT6uV8wX0yZ2aB4cD6eF8gH0iJ2kL4mN6oP8qR0sT2uV4wX6yZ8aB0cD2eF4gH6iJ8kL0mN2oP4qR6sT8uV0wX2yZ4aB6cD8eF0gH2iJ4kL6mN8oP0qR2sT4uV6wX8yZ0aB2cD4eF6gH8iJ0kL2mN4oP6qR8sT0uV2wX4yZ6aB8cD0eF2gH4iJ6kL8mN0oP2qR4sT6uV8wX0yZ2aB4cD6eF8gH0iJ2kL4mN6oP8qR0sT2uV4wX6yZ8aB0cD2eF4gH6iJ8kL0mN2oP4qR6sT8uV0wX2yZ4aB6cD8eF0gH2iJ4kL6mN8oP0qR2sT4uV6wX8yZ0aB2cD4eF6gH8iJ0kL2mN4oP6qR8sT0uV2wX4yZ6aB8cD0eF2gH4iJ6kL8mN0oP2qR4sT6uV8wX0yZ2aB4cD6eF8gH0iJ2kL4mN6oP8qR0sT2uV4wX6yZ8aB0cD2eF4gH6iJ8kL0mN2oP4qR6sT8uV0wX2yZ4aB6cD8eF0gH2iJ4kL6mN8oP0="
-- Actually, let me calculate the proper SHA256 hashes

-- For now, we'll use a C# program to generate them. 
-- Run this C# code to get the hashes:
/*
using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine("broker@123456: " + HashPassword("broker@123456"));
        Console.WriteLine("manager@123456: " + HashPassword("manager@123456"));
        Console.WriteLine("seller@123456: " + HashPassword("seller@123456"));
        Console.WriteLine("accounting@123456: " + HashPassword("accounting@123456"));
    }

    static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
*/

-- Alternative: Update passwords directly using the application
-- For now, let's create a simpler approach - we'll register them through the app

PRINT 'To create proper password hashes, please run the C# code above';
PRINT 'or use the application registration to create these accounts first,';
PRINT 'then we can update the database with the correct role assignments.';

GO
