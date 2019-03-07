using System;
using System.Collections.Generic;
using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;

namespace Kyklos.Kernel.Data.Test.Entities
{
    [EntityObjectInfo(TableName = "DAYS")]
    [EntityUniqueConstraint(ConstraintName = "UNIQUE_DAY", Properties = new string[] { nameof(DayNumber) })]
    public class Day : IBaseEntity
    {
        [EntityPropertyInfo(ColumnName = "DAY_ID", DbType = PropertyDbType.String, IsNullable = false, IsPrimaryKey = true, MaxLength = 50)]
        public string DayId { get; set; }

        [EntityPropertyInfo(ColumnName = "DAY_NUMBER", DbType = PropertyDbType.Integer, IsNullable = false)]
        public int DayNumber { get; set; }

        [EntityPropertyInfo(ColumnName = "DAY_DATE", DbType = PropertyDbType.DateTime, IsNullable = false)]
        public DateTime DayDate { get; set; }

        public override bool Equals(object obj)
        {
            var day = obj as Day;
            return day != null &&
                   DayId == day.DayId &&
                   DayNumber == day.DayNumber &&
                   DayDate == day.DayDate;
        }

        public override int GetHashCode()
        {
            var hashCode = -1832492239;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DayId);
            hashCode = hashCode * -1521134295 + DayNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + DayDate.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Day day1, Day day2)
        {
            return EqualityComparer<Day>.Default.Equals(day1, day2);
        }

        public static bool operator !=(Day day1, Day day2)
        {
            return !(day1 == day2);
        }
    }
}
