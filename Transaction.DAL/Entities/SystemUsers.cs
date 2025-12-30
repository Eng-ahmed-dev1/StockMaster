using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TransactionsTask.Models; // 👈 عشان Transactions

namespace Transaction.DAL
{
    public class SystemUsers : IdentityUser
    {
        [MaxLength(200)]
        public string? FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(500)]
        public string? ProfilePictureUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }

        // Navigation Property
        public ICollection<Transactions> Transactions { get; set; } = new HashSet<Transactions>(); // 👈
    }

    public enum Gender
    {
        Male,
        Female,
        Other,
        PreferNotToSay
    }
}