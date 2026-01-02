using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TransactionsTask.Models;

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

        public CountryType? Country { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }

        public ICollection<Transactions> CreatedTransactions { get; set; } = new HashSet<Transactions>();
        public ICollection<Transactions> SuppliedTransactions { get; set; } = new HashSet<Transactions>();
    }


    public enum Gender
    {
        Male,
        Female,
        Other,
        PreferNotToSay
    }
    public enum CountryType
    {
        // Africa
        Algeria,
        Angola,
        Benin,
        Botswana,
        BurkinaFaso,
        Burundi,
        Cameroon,
        CapeVerde,
        CentralAfricanRepublic,
        Chad,
        Comoros,
        CongoBrazzaville,
        CongoKinshasa,
        Djibouti,
        Egypt,
        EquatorialGuinea,
        Eritrea,
        Eswatini,
        Ethiopia,
        Gabon,
        Gambia,
        Ghana,
        Guinea,
        GuineaBissau,
        IvoryCoast,
        Kenya,
        Lesotho,
        Liberia,
        Libya,
        Madagascar,
        Malawi,
        Mali,
        Mauritania,
        Mauritius,
        Morocco,
        Mozambique,
        Namibia,
        Niger,
        Nigeria,
        Rwanda,
        SaoTomeAndPrincipe,
        Senegal,
        Seychelles,
        SierraLeone,
        Somalia,
        SouthAfrica,
        SouthSudan,
        Sudan,
        Tanzania,
        Togo,
        Tunisia,
        Uganda,
        Zambia,
        Zimbabwe,

        // Asia
        Afghanistan,
        Armenia,
        Azerbaijan,
        Bahrain,
        Bangladesh,
        Bhutan,
        Brunei,
        Cambodia,
        China,
        Georgia,
        India,
        Indonesia,
        Iran,
        Iraq,
        Israel,
        Japan,
        Jordan,
        Kazakhstan,
        Kuwait,
        Kyrgyzstan,
        Laos,
        Lebanon,
        Malaysia,
        Maldives,
        Mongolia,
        Myanmar,
        Nepal,
        NorthKorea,
        Oman,
        Pakistan,
        Palestine,
        Philippines,
        Qatar,
        SaudiArabia,
        Singapore,
        SouthKorea,
        SriLanka,
        Syria,
        Taiwan,
        Tajikistan,
        Thailand,
        TimorLeste,
        Turkey,
        Turkmenistan,
        UnitedArabEmirates,
        Uzbekistan,
        Vietnam,
        Yemen,

        // Europe
        Albania,
        Andorra,
        Austria,
        Belarus,
        Belgium,
        BosniaAndHerzegovina,
        Bulgaria,
        Croatia,
        Cyprus,
        Czechia,
        Denmark,
        Estonia,
        Finland,
        France,
        Germany,
        Greece,
        Hungary,
        Iceland,
        Ireland,
        Italy,
        Kosovo,
        Latvia,
        Liechtenstein,
        Lithuania,
        Luxembourg,
        Malta,
        Moldova,
        Monaco,
        Montenegro,
        Netherlands,
        NorthMacedonia,
        Norway,
        Poland,
        Portugal,
        Romania,
        Russia,
        SanMarino,
        Serbia,
        Slovakia,
        Slovenia,
        Spain,
        Sweden,
        Switzerland,
        Ukraine,
        UnitedKingdom,
        VaticanCity,

        // North America
        AntiguaAndBarbuda,
        Bahamas,
        Barbados,
        Belize,
        Canada,
        CostaRica,
        Cuba,
        Dominica,
        DominicanRepublic,
        ElSalvador,
        Grenada,
        Guatemala,
        Haiti,
        Honduras,
        Jamaica,
        Mexico,
        Nicaragua,
        Panama,
        SaintKittsAndNevis,
        SaintLucia,
        SaintVincentAndTheGrenadines,
        TrinidadAndTobago,
        UnitedStates,

        // South America
        Argentina,
        Bolivia,
        Brazil,
        Chile,
        Colombia,
        Ecuador,
        Guyana,
        Paraguay,
        Peru,
        Suriname,
        Uruguay,
        Venezuela,

        // Oceania
        Australia,
        Fiji,
        Kiribati,
        MarshallIslands,
        Micronesia,
        Nauru,
        NewZealand,
        Palau,
        PapuaNewGuinea,
        Samoa,
        SolomonIslands,
        Tonga,
        Tuvalu,
        Vanuatu
    }
}