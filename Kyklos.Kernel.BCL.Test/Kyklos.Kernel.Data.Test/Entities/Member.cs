using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;

namespace Kyklos.Kernel.Data.Test.Entities
{
    [EntityObjectInfo(TableName = "Members")]
    public class Member : IBaseEntity
    {
        [EntityPropertyInfo(ColumnName = "MemberID", DbType = PropertyDbType.Long, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 10)]
        public virtual long MemberId { get; set; }

        [EntityPropertyInfo(ColumnName = "MemberName", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 50, ColumnOrder = 20)]
        public virtual string MemberName { get; set; }

        [EntityPropertyInfo(ColumnName = "MemberDescription", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 255, ColumnOrder = 30)]
        public virtual string MemberDescription { get; set; }

        [EntityPropertyInfo(ColumnName = "MemberShortCode", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 3, ColumnOrder = 40)]
        public virtual string MemberShortCode { get; set; }

        [EntityPropertyInfo(ColumnName = "IsMemberActive", DbType = PropertyDbType.Boolean, IsNullable = false, ColumnOrder = 50)]
        public virtual bool IsMemberActive { get; set; }

        [EntityPropertyInfo(ColumnName = "ProfileID", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 60)]
        public virtual long ProfileId { get; set; }

        [EntityPropertyInfo(ColumnName = "CompanyID", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 70)]
        public virtual long CompanyId { get; set; }

        [EntityPropertyInfo(ColumnName = "MemberNote", DbType = PropertyDbType.Clob, IsNullable = true, ColumnOrder = 80)]
        public virtual string MemberNote { get; set; }

        [EntityPropertyInfo(ColumnName = "CanBeOwner", DbType = PropertyDbType.Boolean, IsNullable = false, ColumnOrder = 90)]
        public virtual bool CanBeOwner { get; set; }

        [EntityPropertyInfo(ColumnName = "Password", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 50, ColumnOrder = 100)]
        public virtual string Password { get; set; }

        [EntityPropertyInfo(ColumnName = "AnalyticCost", DbType = PropertyDbType.Decimal, IsNullable = true, ColumnOrder = 110)]
        public virtual decimal? AnalyticCost { get; set; }

        [EntityPropertyInfo(ColumnName = "ToBeTraced", DbType = PropertyDbType.Boolean, IsNullable = false, ColumnOrder = 120)]
        public virtual bool ToBeTraced { get; set; }

        [EntityPropertyInfo(ColumnName = "DailyHours", DbType = PropertyDbType.Decimal, IsNullable = true, ColumnOrder = 130)]
        public virtual decimal? DailyHours { get; set; }

        [EntityPropertyInfo(ColumnName = "ReconcileValuationTypeID", DbType = PropertyDbType.Integer, IsNullable = true, ColumnOrder = 140)]
        public virtual int? ReconcileValuationTypeId { get; set; }

        public override bool Equals(object obj)
        {
            var member = obj as Member;
            return member != null &&
                   MemberId == member.MemberId &&
                   MemberName == member.MemberName;
        }

    }
}