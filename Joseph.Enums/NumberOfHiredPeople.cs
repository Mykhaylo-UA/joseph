using System.ComponentModel.DataAnnotations;

namespace Joseph.Enums;

public enum NumberOfHiredPeople: byte
{
    [Display(Name = "1-3")]
    OneThree,
    [Display(Name = "4-7")]
    FourSeven,
    [Display(Name = "7-10")]
    SevenTen,
    [Display(Name = "More than 10")]
    MoreTen
}