using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using University_Academic.Enums;

namespace University_Academic.Entities
{
    public class Grade
    {
        public int GradeId {  get; set; }
        public int EnrollmentId { get; set; }
        public decimal Midterm { get; set; }
        public decimal Practical { get; set; }
        public decimal Final { get; set; }
        [NotMapped]  public decimal Total => Midterm + Practical + Final;
        [NotMapped]
        public LetterGrade LetterGrade => Total switch
        {
            >= 90.00m => LetterGrade.A,
            >= 85.00m => LetterGrade.BPlus,
            >= 80.00m => LetterGrade.B,
            >= 75.00m => LetterGrade.CPlus,
            >= 70.00m => LetterGrade.C,
            >= 65.00m => LetterGrade.DPlus,
            >= 60.00m => LetterGrade.D,
            _ => LetterGrade.F

        };
        [NotMapped]
        public decimal GradePoint => Total switch
        {
            >= 90.00m => 4.00m,
            >= 85.00m => 3.50m,
            >= 80.00m => 3.00m,
            >= 75.00m => 2.50m,
            >= 70.00m => 2.00m,
            >= 65.00m => 1.50m,
            >= 60.00m => 1.00m,
            _ => 0.00m

        };
        public Enrollment Enrollment { get; set; } = null!;
    }
}
