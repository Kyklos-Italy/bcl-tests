using System;
using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;

namespace Kyklos.Kernel.Data.Test.Entities
{
    [EntityObjectInfo(TableName = "JOB_TIMES")]
    public class JobTime : IBaseEntity
    {
        [EntityPropertyInfo(ColumnName = "JobID", DbType = PropertyDbType.Long, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 10)]
        public virtual long JobId { get; set; }

        [EntityPropertyInfo(ColumnName = "MemberID", DbType = PropertyDbType.Long, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 20)]
        public virtual long MemberId { get; set; }

        [EntityPropertyInfo(ColumnName = "DateOfWork", DbType = PropertyDbType.DateTime, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 30)]
        public virtual DateTime DateOfWork { get; set; }

        [EntityPropertyInfo(ColumnName = "AmountTimeToInvoice", DbType = PropertyDbType.Decimal, IsNullable = true, ColumnOrder = 40)]
        public virtual decimal? AmountTimeToInvoice { get; set; }

        [EntityPropertyInfo(ColumnName = "FreeAmountTime", DbType = PropertyDbType.Decimal, IsNullable = true, ColumnOrder = 50)]
        public virtual decimal? FreeAmountTime { get; set; }

        [EntityPropertyInfo(ColumnName = "TimeNote", DbType = PropertyDbType.Clob, IsNullable = true, ColumnOrder = 60)]
        public virtual string TimeNote { get; set; }

        [EntityPropertyInfo(ColumnName = "ReasonID", DbType = PropertyDbType.Integer, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 70)]
        public virtual int ReasonId { get; set; }

        [EntityPropertyInfo(ColumnName = "Hours", DbType = PropertyDbType.Decimal, IsNullable = true, ColumnOrder = 80)]
        public virtual decimal? Hours { get; set; }
    }
}